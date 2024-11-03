using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using PersonalFinances.API.Features.Authentication.Helpers;
using PersonalFinances.API.Features.Authentication.Models;
using PersonalFinances.API.Models;
using PersonalFinances.API.Persistence;
using PersonalFinances.API.Shared;

namespace PersonalFinances.API.Features.Authentication.Services;

public class AuthService : IAuthService
{
    private readonly JwtTokenGenerator _jwtTokenGenerator;
    private readonly UserManager<User> _userManager;
    
    public AuthService( 
        JwtTokenGenerator jwtTokenGenerator, 
        UserManager<User> userManager)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _userManager = userManager;
    }
    
    public async Task<TokenResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        
        if (user == null || ! await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return null; // Invalid credentials
        }
        
        return _jwtTokenGenerator.GenerateJwtToken(user);
    }
}