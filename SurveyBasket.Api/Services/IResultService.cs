
namespace SurveyBasket.Api.Services;

public interface IResultService
{
    Task<Result<PollVotesResponce>> GetPollVotesAsync(int pollId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<VotesPerDayResponce>>> GetVotesPerDayAsync(int pollId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<VotesPerQuestionResponce>>> GetVotesPerQuestionAsync(int pollId, CancellationToken cancellationToken = default);
}
