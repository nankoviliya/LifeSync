using LifeSync.API.Features.Authentication.Models;
using LifeSync.API.Features.Authentication.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace LifeSync.API.Features.Authentication;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService authService;

    public AuthController(IAuthService authService)
    {
        this.authService = authService;
    }

    [HttpPost("login")]
    [EndpointSummary("Authenticates a user and returns a JWT token.")]
    [EndpointDescription("Pass the user credentials to obtain a valid JWT token if authentication is successful.")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var loginResult = await authService.LoginAsync(request);

        if (!loginResult.IsSuccess)
        {
            return Unauthorized();
        }

        return Ok(loginResult.Data);
    }

    [HttpPost("register")]
    [EndpointSummary("Registers a new user.")]
    [EndpointDescription("Creates a new user account with the provided registration details.")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] Models.RegisterRequest request)
    {
        var result = await authService.RegisterAsync(request);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result.Message);
    }
}
