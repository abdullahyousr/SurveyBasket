namespace SurveyBasket.Api.Contracts.Questions;

public record QuestionRequest(
    string Content,
    List<String> Answers
);
