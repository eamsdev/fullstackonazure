using System.Net;
using Application.Features.Identity;
using FluentAssertions;
using Tests.Integration.Extensions;
using Xunit;

namespace Tests.Integration;

public class IdentityTests
{
    [Fact]
    public async Task CanRegisterUser()
    {
        // Given
        using var testContainer = new TestContainer();
        var client = testContainer.HttpClient;
        
        // When
        var response = await RegisterAndLoginUser(client);

        // Then
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task CannotLogoutIfNotLoggedIn()
    {
        // Given
        using var testContainer = new TestContainer();
        var client = testContainer.HttpClient;
        
        // When
        var response = await client.PostRouteAsJsonAsync("identity/logout", new LogoutUser.Command());

        // Then
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task CanLogout()
    {
        // Given
        using var testContainer = new TestContainer();
        var client = testContainer.HttpClient;
        await RegisterAndLoginUser(client);
        
        // When
        var response = await Logout(client);

        // Then
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task CanGetCurrentUserIfLoggedIn()
    {
        // Given
        using var testContainer = new TestContainer();
        var client = testContainer.HttpClient;
        await RegisterAndLoginUser(client);
        
        // When
        var response = await client.GetAsync("identity/user");

        // Then
        response.IsSuccessStatusCode.Should().BeTrue();
    }
    
    [Fact]
    public async Task CannotGetCurrentUserIfNotLoggedIn()
    {
        // Given
        using var testContainer = new TestContainer();
        var client = testContainer.HttpClient;
        
        // When
        var response = await client.GetAsync("identity/user");

        // Then
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task CannotGetCurrentUserAfterLoggedOut()
    {
        // Given
        using var testContainer = new TestContainer();
        var client = testContainer.HttpClient;
        await RegisterAndLoginUser(client);
        await Logout(client);
        
        // When
        var response = await client.GetAsync("identity/user");

        // Then
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    
    [Fact]
    public async Task CanRegisterThenLogoutThenLoginThenAccessProtectedEndpoint()
    {
        // Given
        using var testContainer = new TestContainer();
        var client = testContainer.HttpClient;
        var user = await RegisterAndGetUser(client);
        await Logout(client);
        var loginInput = new LoginUser.Command
        {
            Username = user.Username,
            Password = user.Password
        };

        // When
        var response = await client.PostRouteAsJsonAsync("identity/login", loginInput);

        // Then
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    private static async Task<HttpResponseMessage> Logout(HttpClient client)
    {
        return await client.PostRouteAsJsonAsync("identity/logout", new LogoutUser.Command());
    }
    
    private static async Task<HttpResponseMessage> RegisterAndLoginUser(HttpClient client)
    {
        var registerRequest = new RegisterUser.Command
        {
            Username = "SuperSecureUsername7",
            Password = "Password_1234!!"
        };
        
        return await client.PostRouteAsJsonAsync("identity/register", registerRequest);
    }
    
    private static async Task<RegisterUser.Command> RegisterAndGetUser(HttpClient client)
    {
        var registerRequest = new RegisterUser.Command
        {
            Username = "SuperSecureUsername7",
            Password = "Password_1234!!"
        };
        
        await client.PostRouteAsJsonAsync("identity/register", registerRequest);
        return registerRequest;
    }
}