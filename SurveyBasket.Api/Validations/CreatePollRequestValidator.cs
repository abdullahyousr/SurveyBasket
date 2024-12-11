﻿
namespace SurveyBasket.Api.Validations;

public class CreatePollRequestValidator : AbstractValidator<CreatePollRequest>
{
    public CreatePollRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .Length(3,100);

        RuleFor(x => x.Description)
            .NotEmpty()
            .Length(3, 100);
    }
}
