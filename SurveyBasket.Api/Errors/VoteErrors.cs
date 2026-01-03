namespace SurveyBasket.Api.Errors;

public record VoteErrors
{
    public static readonly Error InvalidQuestions = new Error("Vote.InvalidQuestions", "InvalidQuestions", StatusCodes.Status404NotFound);
    public static readonly Error DuplicatedVote = new Error("Vote.DuplicatedVote", "This user has already voted before", StatusCodes.Status409Conflict);
}

