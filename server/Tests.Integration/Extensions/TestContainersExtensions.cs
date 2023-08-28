using Application.Features.Identity;
using Bogus;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Integration.Extensions;

public static class TestContainersExtensions
{
    public static async Task<IEnumerable<User>> GenerateRandomUsers(this TestContainer container, int count = 1)
    {
        var users = new List<User>();
        foreach (var _ in Enumerable.Range(0, count))
        {
            users.Add(await container.RegisterUser());
        }
        return users;
    }
    
    private static async Task<User> RegisterUser(this TestContainer container)
    {
        var registerRequest = new Faker<RegisterUser.Command>()
            .StrictMode(true)
            .RuleFor(x => x.Username, y => y.Internet.UserName())
            .RuleFor(x => x.Password, _ => "Password_1234!!")
            .Generate(1).Single();

        var user = new User
        {
            UserName = registerRequest.Username
        };
        
        var createResult = await container.ServiceProvider
            .GetRequiredService<UserManager<User>>()
            .CreateAsync(user, registerRequest.Password);

        if (!createResult.Succeeded)
            throw new Exception("Test data generation failed!");

        return user;
    }
}