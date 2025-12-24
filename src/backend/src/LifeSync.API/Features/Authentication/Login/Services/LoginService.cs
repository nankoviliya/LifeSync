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

    public async Task<DataResult<LoginResponse>> LoginAsync(LoginRequest request,
        CancellationToken cancellationToken)
    {
        User? user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Failure<LoginResponse>("Invalid email or password.");
        }

        TokenResponse accessToken = await _jwtTokenGenerator.GenerateAccessTokenAsync(user, request.DeviceType);

        TokenResponse refreshToken = JwtTokenGenerator.GenerateRefreshToken(request.DeviceType);
        string tokenHash = JwtTokenGenerator.HashRefreshToken(refreshToken.Token);

        RefreshToken refreshTokenEntity = RefreshToken.Create(
            user.Id,
            tokenHash,
            refreshToken.Expiry,
            request.DeviceType);

        await _context.RefreshTokens.AddAsync(refreshTokenEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        LoginResponse loginResponse = new()
        {
            AccessToken = accessToken.Token,
            AccessTokenExpiry = accessToken.Expiry,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiry = refreshToken.Expiry,
            Message = "Login successful"
        };

        return Success(loginResponse);
    }
}