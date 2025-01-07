
using SurveyBasket.Api.Contracts.Questions;

namespace SurveyBasket.Api.Services;

public class ResultService(ApplicationDbContext context) : IResultService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result<PollVotesResponce>> GetPollVotesAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var pollVotes = await _context.Polls
            .Where(x => x.Id == pollId)
            .Select(x => new PollVotesResponce(
                x.Title,
                x.Votes.Select(v => new VoteResponce(
                    $"{v.User.FirstName} {v.User.LastName}",
                    v.SubmittedOn,
                    v.VoteAnswers.Select(answer => new QuestionAnswerResponce(
                        answer.Question.Content,
                        answer.Answer.Content
                        ))
                    ))
                ))
            .SingleOrDefaultAsync(cancellationToken);
        return pollVotes is null 
              ? Result.Failure<PollVotesResponce>(PollErrors.PollNotFound)
              : Result.Success(pollVotes);
    }

    public async Task<Result<IEnumerable<VotesPerDayResponce>>> GetVotesPerDayAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var pollISExists = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken);

        if (!pollISExists)
            return Result.Failure<IEnumerable<VotesPerDayResponce>>(PollErrors.PollNotFound);

        var votesPerDay = await _context.Votes
            .Where(x => x.PollId == pollId)
            .GroupBy(x => new {Date = DateOnly.FromDateTime(x.SubmittedOn)})
            .Select(g => new VotesPerDayResponce(
                g.Key.Date,
                g.Count()
             ))
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<VotesPerDayResponce>>(votesPerDay);
    }

    public async Task<Result<IEnumerable<VotesPerQuestionResponce>>> GetVotesPerQuestionAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var pollISExists = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken);

        if (!pollISExists)
            return Result.Failure<IEnumerable<VotesPerQuestionResponce>>(PollErrors.PollNotFound);

        var votesPerQuestion = await _context.VoteAnswers
            .Where(x => x.Vote.PollId == pollId)
            .Select(x => new VotesPerQuestionResponce(
                x.Question.Content,
                x.Question.VoteAnswers
                    .GroupBy(x => new { AnswerId = x.AnswerId, AnswerContent = x.Answer.Content })
                    .Select(g => new VotesPerAnswerResponce(
                        g.Key.AnswerContent,
                        g.Count()
                    ))
            ))
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<VotesPerQuestionResponce>>(votesPerQuestion);
    }
}
