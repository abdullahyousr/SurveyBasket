namespace SurveyBasket.Api.Contracts.Roles;

public class RoleRequestValidator : AbstractValidator<RoleRequest>
{
    public RoleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(3, 200);

        RuleFor(x => x.Permissions)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Permissions)
            .Must(permissions => permissions.Distinct().Count() == permissions.Count())
            .WithMessage("Permissions must be unique.")
            .When(x => x.Permissions is not null);
    }
}
