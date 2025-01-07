namespace SurveyBasket.Api.Contracts.Results;

public record VotesPerAnswerResponce(
    string Answer,
    int Count
);