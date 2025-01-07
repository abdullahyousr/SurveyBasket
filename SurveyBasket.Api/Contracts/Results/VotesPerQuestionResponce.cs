namespace SurveyBasket.Api.Contracts.Results;

public record VotesPerQuestionResponce(
    string Question,
    IEnumerable<VotesPerAnswerResponce> SelectedAnswers
);
