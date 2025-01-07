namespace SurveyBasket.Api.Contracts.Results;

public record VoteResponce(
    string VoterName,
    DateTime VoteDate,
    IEnumerable<QuestionAnswerResponce> SelectedAnswers
);
