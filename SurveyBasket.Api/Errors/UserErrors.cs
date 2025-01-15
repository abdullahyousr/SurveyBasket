namespace SurveyBasket.Api.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredentials = new Error("User.InvalidCredentials", "Invalid email/password", StatusCodes.Status401Unauthorized);
    public static readonly Error InvalidJwtToken = new Error("User.InvalidJwtToken", "Invalid Jwt token", StatusCodes.Status401Unauthorized);
    public static readonly Error InvalidRefrestToken = new Error("User.InvalidRefreshToken", "Invalid refresh Token", StatusCodes.Status401Unauthorized);
    public static readonly Error DuplicatedEmail = new Error("User.DuplicatedEmail", "User with the same email is already registered", StatusCodes.Status409Conflict);
    public static readonly Error EmailNotConfirmed = new Error("User.EmailNotConfirmed", "Email is not confirmed", StatusCodes.Status401Unauthorized);
    public static readonly Error InvalidCode = new Error("User.InvalidCode", "Invalid Code", StatusCodes.Status401Unauthorized);
    public static readonly Error DuplicatedConfirmation = new Error("User.DuplicatedConfirmation", "Email is already confirmed", StatusCodes.Status401Unauthorized);

}
