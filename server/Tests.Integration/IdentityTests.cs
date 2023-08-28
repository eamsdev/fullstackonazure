using System.Net;
using Application.Features.Identity;
using Bogus;
using FluentAssertions;
using Tests.Integration.Extensions;
using Xunit;

namespace Tests.Integration;

public class IdentityTests : IClassFixture<TestContainer>
{
    private readonly TestContainer _container;

    public IdentityTests(TestContainer container)
    {
        _container = container;
    }
    
    [Fact]
    public async Task CanRegisterUser()
    {
        // Given
        var client = _container.CreateClient();
        
        // When
        var response = await RegisterAndLoginUser(client);

        // Then
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task CannotLogoutIfNotLoggedIn()
    {
        // Given
        var client = _container.CreateClient();
        
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
        var client = _container.CreateClient();
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
        var client = _container.CreateClient();
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
        var client = _container.CreateClient();
        
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
        var client = _container.CreateClient();
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
        var client = _container.CreateClient();
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
        var registerRequest = new Faker<RegisterUser.Command>()
            .StrictMode(true)
            .RuleFor(x => x.Username, y => y.Internet.UserName())
            .RuleFor(x => x.Password, _ => "Password_1234!!")
            .Generate(1).Single();
        
        return await client.PostRouteAsJsonAsync("identity/register", registerRequest);
    }
    
    private static async Task<RegisterUser.Command> RegisterAndGetUser(HttpClient client)
    {
        var registerRequest = new Faker<RegisterUser.Command>()
            .StrictMode(true)
            .RuleFor(x => x.Username, y => y.Internet.UserName())
            .RuleFor(x => x.Password, _ => "Password_1234!!")
            .Generate(1).Single();
        
        await client.PostRouteAsJsonAsync("identity/register", registerRequest);
        return registerRequest;
    }
}