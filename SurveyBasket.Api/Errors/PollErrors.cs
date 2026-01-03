namespace SurveyBasket.Api.Errors;

public record PollErrors
{
    public static readonly Error PollNotFound = new Error("Poll.NotFound", "No poll was found with the given ID", StatusCodes.Status404NotFound);
    public static readonly Error DuplicatedPollTitle = new Error("Poll.DuplicatedTitle", "Another title with the same title exists", StatusCodes.Status409Conflict);
}
