using Application.Common;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Identity;

public static class GetCurrentUser
{
    public class Query : IRequest<Result>
    { }

    public class Result
    {
        public string? Username { get; set; }
    }
    
    public class Handler : IRequestHandler<Query, Result>
    {
        private readonly UserManager<User> _userManager;
        private readonly CurrentUserContext _currentUserContext;

        public Handler(
            UserManager<User> userManager,
            CurrentUserContext currentUserContext)
        {
            _userManager = userManager;
            _currentUserContext = currentUserContext;
        }
        
        public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
        {
            if (_currentUserContext.UserId is null)
                throw new Exception();

            var user = await _userManager.FindByIdAsync(_currentUserContext.UserId);
            if (user is null)
                throw new Exception();
            
            return new Result
            {
                Username = user.UserName
            };
        }
    }
}