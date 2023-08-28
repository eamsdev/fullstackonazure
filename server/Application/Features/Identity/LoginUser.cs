using System.Security.Authentication;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Identity;

public static class LoginUser
{
    public class Command : IRequest
    {
        public required string Username { get; set; } = string.Empty;

        public required string Password { get; set; } = string.Empty;
    }

    public class Handler : IRequestHandler<Command>
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public Handler(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        
        public async Task Handle(Command command, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(command.Username);
            if (user is null)
            {
                throw new AuthenticationException();
            }

            await _signInManager.PasswordSignInAsync(user, command.Password, true, false);
        }
    }
}