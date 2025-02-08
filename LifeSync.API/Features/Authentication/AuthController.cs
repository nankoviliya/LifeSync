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
