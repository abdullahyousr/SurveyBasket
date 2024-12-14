namespace SurveyBasket.Api.Services;

public interface IAuthService
{
    Task<AuthResponce?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);
}
