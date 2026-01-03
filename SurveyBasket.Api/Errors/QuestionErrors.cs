namespace SurveyBasket.Api.Errors;

public record QuestionErrors
{
    public static readonly Error QuestionNotFound = new Error("Question.NotFound", "No question was found with the given ID", StatusCodes.Status404NotFound);
    public static readonly Error DuplicatedQuestionContent = new Error("Question.DuplicatedContent", "Another Question with the same conent exists", StatusCodes.Status409Conflict);
}
