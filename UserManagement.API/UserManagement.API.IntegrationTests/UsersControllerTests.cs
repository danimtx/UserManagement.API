using System;
using System.Net;
using System.Net.Http.Json;
using System.Net.Http.Headers;
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
using UserManagement.Application.DTOs.User.Profile;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Entities.ValueObjects;
using UserManagement.Domain.Enums;
using Xunit;

namespace UserManagement.API.IntegrationTests;

public class UsersControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public UsersControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _factory.UserRepositoryMock.Reset();
        _factory.IdentityProviderMock.Reset();
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
    public async Task GetMyProfile_WhenAuthenticated_ReturnsOkAndProfile()
    {
        // Arrange
        var testUserId = "profile-user-1";
        var userCi = "1234567";
        var authToken = GenerateTestAuthToken(testUserId, "Personal");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

        var user = new User
        {
            Id = testUserId,
            Email = "profile@example.com",
            TipoUsuario = UserType.Personal.ToString(),
            Estado = UserStatus.Activo.ToString(),
            Biografia = "Software Developer",
            DatosPersonales = new PersonalProfile
            {
                Nombres = "Profile",
                ApellidoPaterno = "User",
                CI = userCi
            }
        };

        _factory.UserRepositoryMock.Setup(x => x.GetByIdAsync(testUserId)).ReturnsAsync(user);

        // Act
        var response = await _client.GetAsync("/api/Users/profile/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var profileDto = await response.Content.ReadFromJsonAsync<UserProfileDto>();
        
        profileDto.Should().NotBeNull();
        profileDto.Id.Should().Be(testUserId);
        profileDto.Biografia.Should().Be("Software Developer");
        profileDto.DatosPersonales.Should().NotBeNull();
        profileDto.DatosPersonales.CI.Should().NotBe(userCi); // Check that it is masked
        profileDto.DatosPersonales.CI.Should().EndWith(userCi.Substring(userCi.Length - 3));
    }
}
