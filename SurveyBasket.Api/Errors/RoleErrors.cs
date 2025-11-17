namespace SurveyBasket.Api.Errors;

public static class RoleErrors
{
    public static readonly Error RoleNotFound = new Error("Role.RoleNotFound", "Role is not found", StatusCodes.Status404NotFound);
    public static readonly Error DuplicatedRole = new Error("Role.DuplicatedRole", "Role with the same name is already registered", StatusCodes.Status409Conflict);
    public static readonly Error InvalidPermissions = new Error("Role.InvalidPermissions", "Invalid Permissions", StatusCodes.Status400BadRequest);
    //public static readonly Error InvalidRefrestToken = new Error("User.InvalidRefreshToken", "Invalid refresh Token", StatusCodes.Status401Unauthorized);
    //public static readonly Error EmailNotConfirmed = new Error("User.EmailNotConfirmed", "Email is not confirmed", StatusCodes.Status401Unauthorized);
    //public static readonly Error InvalidCode = new Error("User.InvalidCode", "Invalid Code", StatusCodes.Status401Unauthorized);
    //public static readonly Error DuplicatedConfirmation = new Error("User.DuplicatedConfirmation", "Email is already confirmed", StatusCodes.Status401Unauthorized);

}
