namespace SurveyBasket.Api.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredentials = new Error("User.InvalidCredentials", "Invalid email/password", StatusCodes.Status401Unauthorized);
    public static readonly Error InvalidToken = new Error("User.InvalidToken", "Invalid token/refreshToken", StatusCodes.Status401Unauthorized);
}
