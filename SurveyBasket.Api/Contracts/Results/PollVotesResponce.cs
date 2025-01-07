namespace SurveyBasket.Api.Contracts.Results;

public record PollVotesResponce(
    string Title,
    IEnumerable<VoteResponce> VoteResponces
    );
