using LifeSync.API.Features.Authentication.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace LifeSync.API.Features.Authentication;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var loginResult = await _authService.LoginAsync(request);

        if (!loginResult.IsSuccess)
        {
            return Unauthorized();
        }

        return Ok(loginResult.Data);
    }
}
