using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Identity;

public static class LogoutUser
{
    public class Command : IRequest
    { }

    public class Handler : IRequestHandler<Command>
    {
        private readonly SignInManager<User> _signInManager;

        public Handler(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }
        
        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            await _signInManager.SignOutAsync();
        }
    }
}