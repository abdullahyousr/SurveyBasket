namespace SurveyBasket.Api.Contracts.Results;

public record VotesPerDayResponce(
    DateOnly Date,
    int NumberOfVotes
);
