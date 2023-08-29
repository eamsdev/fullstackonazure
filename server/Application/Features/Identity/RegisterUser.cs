using Application.Common.Exceptions;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Identity;

public static class RegisterUser
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
        
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                UserName = request.Username
            };
            
            var createResult = await _userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
                throw new UnauthorizedException();
            
            var signInResult = await _signInManager.PasswordSignInAsync(user, request.Password, true, false);
            if (!signInResult.Succeeded)
                throw new UnauthorizedException();
        }
    }
}