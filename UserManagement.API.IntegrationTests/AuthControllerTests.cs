using System;
using System.Net;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using FluentAssertions;
using Moq;
using UserManagement.Application.DTOs.Auth;
using UserManagement.Domain.Entities;
using Xunit;

namespace UserManagement.API.IntegrationTests;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _factory.UserRepositoryMock.Reset();
        _factory.IdentityProviderMock.Reset();
    }

    private string GenerateTestJwt(string userId)
    {
        var claims = new[] { new Claim("user_id", userId) };
        var token = new JwtSecurityToken(claims: claims);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    private string GenerateTestAuthToken(string userId, string userType)
    {
        var configuration = _factory.Services.GetRequiredService<IConfiguration>();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Role, userType)
        };
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [Fact]
    public async Task RegisterPersonal_WithValidData_ReturnsOk()
    {
        var testUserId = "test-user-id-123";
        var registerDto = new RegisterPersonalDto
        {
            Email = "test@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            Nombres = "Test",
            ApellidoPaterno = "User",
            UserName = "test.user",
            CI = "1234567",
            FechaNacimiento = new DateTime(1990, 1, 1)
        };
        _factory.IdentityProviderMock.Setup(x => x.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(testUserId);
        _factory.UserRepositoryMock.Setup(x => x.GetByUserNameAsync(It.IsAny<string>())).ReturnsAsync((User)null);
        
        var response = await _client.PostAsJsonAsync("/api/Auth/register/personal", registerDto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _factory.UserRepositoryMock.Verify(x => x.AddAsync(It.Is<User>(u => u.Id == testUserId)), Times.Once);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkAndToken()
    {
        var testUserId = "test-user-id-123";
        var loginDto = new LoginDto { Email = "test@example.com", Password = "Password123!" };
        var user = new User { Id = testUserId, Email = "test@example.com", TipoUsuario = "Personal", Estado = "Activo", ModulosHabilitados = new List<string> { "Social" } };
        _factory.IdentityProviderMock.Setup(x => x.SignInAsync(loginDto.Email, loginDto.Password)).ReturnsAsync(("dummy-firebase-token", testUserId, "dummy-refresh-token"));
        _factory.UserRepositoryMock.Setup(x => x.GetByIdAsync(testUserId)).ReturnsAsync(user);

        var response = await _client.PostAsJsonAsync("/api/Auth/login", loginDto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        authResponse.Should().NotBeNull();
        authResponse.UserId.Should().Be(testUserId);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        var loginDto = new LoginDto { Email = "invalid@example.com", Password = "WrongPassword!" };
        _factory.IdentityProviderMock.Setup(x => x.SignInAsync(loginDto.Email, loginDto.Password)).ThrowsAsync(new UnauthorizedAccessException("Credenciales inválidas"));
        
        var response = await _client.PostAsJsonAsync("/api/Auth/login", loginDto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RegisterCompany_WithValidData_ReturnsOk()
    {
        var testCompanyId = "test-company-id-456";
        var registerDto = new RegisterCompanyDto
        {
            EmailEmpresa = "company@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            RazonSocial = "Test Inc.",
            Nit = "1234567890",
            Sucursales = new List<SucursalDto> { new SucursalDto { Nombre = "Oficina Central", Direccion = "Av. Principal 123", Departamento = "La Paz" } }
        };
        _factory.IdentityProviderMock.Setup(x => x.CreateUserAsync(registerDto.EmailEmpresa, registerDto.Password, registerDto.RazonSocial)).ReturnsAsync(testCompanyId);

        var response = await _client.PostAsJsonAsync("/api/Auth/register/company", registerDto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _factory.UserRepositoryMock.Verify(x => x.AddAsync(It.Is<User>(u => u.Id == testCompanyId)), Times.Once);
        var jsonString = await response.Content.ReadAsStringAsync();
        using var jsonDocument = JsonDocument.Parse(jsonString);
        Assert.Equal("Empresa registrada. Pendiente de validación.", jsonDocument.RootElement.GetProperty("message").GetString());
    }
    
    [Fact]
    public async Task RefreshToken_WithValidToken_ReturnsNewTokens()
    {
        var testUserId = "test-user-id-123";
        var refreshTokenDto = new RefreshTokenRequestDto { RefreshToken = "old-valid-refresh-token" };
        var fakeNewFirebaseToken = GenerateTestJwt(testUserId);
        _factory.IdentityProviderMock.Setup(x => x.RefreshTokenAsync(refreshTokenDto.RefreshToken)).ReturnsAsync((fakeNewFirebaseToken, "new-refresh-token"));
        _factory.UserRepositoryMock.Setup(x => x.GetByIdAsync(testUserId)).ReturnsAsync(new User { Id = testUserId, Email = "test@example.com", TipoUsuario = "Personal", Estado = "Activo" });

        var response = await _client.PostAsJsonAsync("/api/Auth/refresh-token", refreshTokenDto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        authResponse.Should().NotBeNull();
        authResponse.RefreshToken.Should().Be("new-refresh-token");
    }

    [Fact]
    public async Task RefreshToken_WithInvalidToken_ReturnsUnauthorized()
    {
        var refreshTokenDto = new RefreshTokenRequestDto { RefreshToken = "invalid-token" };
        _factory.IdentityProviderMock.Setup(x => x.RefreshTokenAsync(refreshTokenDto.RefreshToken)).ThrowsAsync(new UnauthorizedAccessException("Invalid refresh token."));
        
        var response = await _client.PostAsJsonAsync("/api/Auth/refresh-token", refreshTokenDto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ChangePassword_WithValidData_ReturnsOk()
    {
        var testUserId = "test-user-id-123";
        var userEmail = "test@example.com";
        var token = GenerateTestAuthToken(testUserId, "Personal");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var changePasswordDto = new ChangePasswordDto { CurrentPassword = "current-password", NewPassword = "new-password-123", ConfirmNewPassword = "new-password-123" };
        
        _factory.UserRepositoryMock.Setup(x => x.GetByIdAsync(testUserId)).ReturnsAsync(new User { Id = testUserId, Email = userEmail });
        _factory.IdentityProviderMock.Setup(x => x.SignInAsync(userEmail, changePasswordDto.CurrentPassword)).ReturnsAsync(("", "", ""));
        _factory.IdentityProviderMock.Setup(x => x.UpdatePasswordAsync(testUserId, changePasswordDto.NewPassword)).Returns(Task.CompletedTask);

        var response = await _client.PostAsJsonAsync("/api/Auth/change-password", changePasswordDto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _factory.IdentityProviderMock.Verify(x => x.UpdatePasswordAsync(testUserId, changePasswordDto.NewPassword), Times.Once);
    }

    [Fact]
    public async Task ChangePassword_WithWrongCurrentPassword_ReturnsUnauthorized()
    {
        var testUserId = "test-user-id-123";
        var userEmail = "test@example.com";
        var token = GenerateTestAuthToken(testUserId, "Personal");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var changePasswordDto = new ChangePasswordDto { CurrentPassword = "wrong-password", NewPassword = "a-valid-new-password" , ConfirmNewPassword = "a-valid-new-password"};

        _factory.UserRepositoryMock.Setup(x => x.GetByIdAsync(testUserId)).ReturnsAsync(new User { Id = testUserId, Email = userEmail });
        _factory.IdentityProviderMock.Setup(x => x.SignInAsync(userEmail, changePasswordDto.CurrentPassword)).ThrowsAsync(new UnauthorizedAccessException());

        var response = await _client.PostAsJsonAsync("/api/Auth/change-password", changePasswordDto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ChangePassword_WithMismatchedNewPasswords_ReturnsBadRequest()
    {
        var testUserId = "test-user-id-123";
        var token = GenerateTestAuthToken(testUserId, "Personal");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var changePasswordDto = new ChangePasswordDto { CurrentPassword = "current-password", NewPassword = "new-password-A", ConfirmNewPassword = "new-password-B" };

        var response = await _client.PostAsJsonAsync("/api/Auth/change-password", changePasswordDto);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
