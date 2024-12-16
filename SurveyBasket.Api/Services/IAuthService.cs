namespace SurveyBasket.Api.Services;

public interface IAuthService
{
    Task<AuthResponce?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<AuthResponce?> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    Task<bool> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
}
