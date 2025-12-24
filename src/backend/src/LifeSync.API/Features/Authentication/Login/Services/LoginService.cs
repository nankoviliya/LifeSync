using LifeSync.API.Features.Authentication.Helpers;
using LifeSync.API.Features.Authentication.Login.Models;
using LifeSync.API.Models.ApplicationUser;
using LifeSync.API.Models.RefreshTokens;
using LifeSync.API.Persistence;
using LifeSync.API.Shared.Services;
using LifeSync.Common.Results;
using Microsoft.AspNetCore.Identity;

namespace LifeSync.API.Features.Authentication.Login.Services;

public class LoginService : BaseService, ILoginService
{
    private readonly UserManager<User> _userManager;
    private readonly JwtTokenGenerator _jwtTokenGenerator;
    private readonly ApplicationDbContext _context;

    public LoginService(
        UserManager<User> userManager,
        JwtTokenGenerator jwtTokenGenerator,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _context = context;
    }

    public async Task<DataResult<LoginResponse>> LoginAsync(LoginRequest request)
    {
        // Validate credentials
        User? user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Failure<LoginResponse>("Invalid email or password.");
        }

        // Generate access token with platform-specific expiration
        TokenResponse accessToken = await _jwtTokenGenerator.GenerateJwtTokenAsync(user, request.DeviceType);

        // Generate refresh token
        string refreshToken = _jwtTokenGenerator.GenerateRefreshToken();
        string tokenHash = _jwtTokenGenerator.HashRefreshToken(refreshToken);

        // Calculate platform-specific refresh token expiration
        TimeSpan refreshLifetime = JwtTokenGenerator.GetRefreshTokenLifetime(request.DeviceType);
        DateTime refreshExpiry = DateTime.UtcNow.Add(refreshLifetime);

        // Store refresh token in database
        RefreshToken refreshTokenEntity = RefreshToken.Create(
            user.Id,
            tokenHash,
            refreshExpiry,
            request.DeviceType);

        await _context.RefreshTokens.AddAsync(refreshTokenEntity);
        await _context.SaveChangesAsync();

        // Build and return response
        LoginResponse loginResponse = new()
        {
            AccessToken = accessToken.Token,
            AccessTokenExpiry = accessToken.Expiry,
            RefreshToken = refreshToken,
            RefreshTokenExpiry = refreshExpiry,
            Message = "Login successful"
        };

        return Success(loginResponse);
    }
}