
using Microsoft.AspNetCore.Identity;
using SurveyBasket.Api.Authentication;
using System.Security.Cryptography;

namespace SurveyBasket.Api.Services;

public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly int _refreshTokenExpiryDays = 14;

    public async Task<AuthResponce?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        //Check user
        var user = await _userManager.FindByEmailAsync(email);
        if(user is null)
            return null;

        //Check Password
        var isValidPassword = await _userManager.CheckPasswordAsync(user, password);
        if(!isValidPassword)
            return null;

        //Generate JWT Token
        var (token, expiresIn) = _jwtProvider.GenerateToken(user);

        //Generate RefreshToken Token
        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        await _userManager.UpdateAsync(user);

        return new AuthResponce(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn, refreshToken, refreshTokenExpiration);
    }

    public async Task<AuthResponce?> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if(userId is null)
            return null;

        var user = await _userManager.FindByIdAsync(userId);
        
        if(user is null) 
            return null;

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);
        
        if(userRefreshToken is null) 
            return null;

        userRefreshToken.RevokedOn = DateTime.UtcNow;
        //Generate JWT Token
        var (newtoken, expiresIn) = _jwtProvider.GenerateToken(user);

        //Generate RefreshToken Token
        var newRefreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        await _userManager.UpdateAsync(user);

        return new AuthResponce(user.Id, user.Email, user.FirstName, user.LastName, newtoken, expiresIn, newRefreshToken, refreshTokenExpiration);


    }

    public async Task<bool> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId is null)
            return false;

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return false;

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

        if (userRefreshToken is null)
            return false;

        userRefreshToken.RevokedOn = DateTime.UtcNow;
        
        await _userManager.UpdateAsync(user);
        return true;
    }

    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

}
