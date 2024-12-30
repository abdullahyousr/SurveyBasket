using SurveyBasket.Api.Contracts.Answers;

namespace SurveyBasket.Api.Contracts.Questions;

public record QuestionResponce(
    int Id,
    string Content,
    IEnumerable<AnswerResponce> Answers
);