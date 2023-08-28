using Application.Common;
using Application.Features.Identity;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class IdentityController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<IdentityController> _logger;
    private readonly CurrentUserContext _currentUserContext;

    public IdentityController(
        IMediator mediator,
        ILogger<IdentityController> logger,
        CurrentUserContext currentUserContext)
    {
        _logger = logger;
        _mediator = mediator;
        _currentUserContext = currentUserContext;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUser.Command command)
    {
        await _mediator.Send(command);
        return Ok();
    }
    
    [AllowAnonymous]
    [HttpGet("login-github")]
    public IResult Login()
    {
        return Results.Challenge(
            new AuthenticationProperties
            {
                RedirectUri = "http://localhost:8080/"
            }, 
            authenticationSchemes: new List<string> { "Github" });
    }
    
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUser.Command command)
    {
        await _mediator.Send(command);
        return Ok();
    }
    
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutUser.Command command)
    {
        await _mediator.Send(command);
        return Ok();
    }
    
    
    [HttpGet("user")]
    public async Task<IActionResult> GetUser()
    {
        var result = await _mediator.Send(new GetCurrentUser.Query());
        return Ok(result);
    }
}