namespace SurveyBasket.Api.Contracts.Authentication;

public record AuthRequest(
    string Email,
    string Password
);
