using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using UserManagement.Application.Interfaces.Repositories;
using UserManagement.Application.Interfaces.Services;

namespace UserManagement.API.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<IUserRepository> UserRepositoryMock { get; } = new();
    public Mock<IIdentityProvider> IdentityProviderMock { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        // Add test-specific JWT configuration
        builder.ConfigureAppConfiguration((context, conf) =>
        {
            conf.AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string>("Jwt:Key", "A_VERY_LONG_AND_SECRET_KEY_FOR_TESTING_!@#$%"),
                new KeyValuePair<string, string>("Jwt:Issuer", "test-issuer"),
                new KeyValuePair<string, string>("Jwt:Audience", "test-audience")
            });
        });

        builder.ConfigureServices(services =>
        {
            // Find and remove original services
            var userRepositoryDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IUserRepository));
            if (userRepositoryDescriptor != null)
            {
                services.Remove(userRepositoryDescriptor);
            }

            var identityProviderDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IIdentityProvider));
            if (identityProviderDescriptor != null)
            {
                services.Remove(identityProviderDescriptor);
            }

            // Add mocks
            services.AddSingleton(UserRepositoryMock.Object);
            services.AddSingleton(IdentityProviderMock.Object);
        });
    }
}

