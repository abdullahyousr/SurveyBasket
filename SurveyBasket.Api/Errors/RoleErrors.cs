namespace SurveyBasket.Api.Errors;

public record RoleErrors
{
    public static readonly Error RoleNotFound = new Error("Role.RoleNotFound", "Role is not found", StatusCodes.Status404NotFound);
    public static readonly Error DuplicatedRole = new Error("Role.DuplicatedRole", "Role with the same name is already registered", StatusCodes.Status409Conflict);
    public static readonly Error InvalidPermissions = new Error("Role.InvalidPermissions", "Invalid Permissions", StatusCodes.Status400BadRequest);
}
