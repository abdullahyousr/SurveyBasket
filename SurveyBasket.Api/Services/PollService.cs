using Hangfire;

namespace SurveyBasket.Api.Services;

public class PollService(ApplicationDbContext context,
    INotificationService notificationService) : IPollService
{
    private readonly ApplicationDbContext _context = context;
    private readonly INotificationService _notificationService = notificationService;

    public async Task<IEnumerable<PollResponce>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Polls
                    .AsNoTracking()
                    .ProjectToType<PollResponce>()
                    .ToListAsync(cancellationToken);

    public async Task<IEnumerable<PollResponce>> GetCurrentAsyncV1(CancellationToken cancellationToken = default)
        => await QueryCurrentPublishedPolls()
                    .ProjectToType<PollResponce>()
                    .ToListAsync(cancellationToken);

    public async Task<IEnumerable<PollResponceV2>> GetCurrentAsyncV2(CancellationToken cancellationToken = default)
        => await QueryCurrentPublishedPolls()
                    .ProjectToType<PollResponceV2>()
                    .ToListAsync(cancellationToken);

    private IQueryable<Poll> QueryCurrentPublishedPolls()
    {
        return _context.Polls
                            .Where(p => p.IsPublished && p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && p.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow))
                            .AsNoTracking();
    }

    public async Task<Result<PollResponce>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _context.Polls.FindAsync(id, cancellationToken);
        return poll is not null
             ? Result.Success(poll.Adapt<PollResponce>())
             : Result.Failure<PollResponce>(PollErrors.PollNotFound);
    }

    public async Task<Result<PollResponce>> AddAsync(PollRequest request, CancellationToken cancellationToken = default)
    {
        var isExistingTitle = await _context.Polls.AnyAsync(p => p.Title == request.Title, cancellationToken);
        if (isExistingTitle)
            return Result.Failure<PollResponce>(PollErrors.DuplicatedPollTitle);

        var newPoll = request.Adapt<Poll>();

        await _context.Polls.AddAsync(newPoll, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var responce = newPoll.Adapt<PollResponce>();

        return Result.Success(responce);
    }

    public async Task<Result> UpdateAsync(int id, PollRequest request, CancellationToken cancellationToken = default)
    {
        var currentPoll = await _context.Polls.FindAsync(id, cancellationToken);

        if (currentPoll is null)
            return Result.Failure(PollErrors.PollNotFound);

        var isExistingTitle = await _context.Polls.AnyAsync(p => p.Title == request.Title && p.Id != id, cancellationToken);
        if (isExistingTitle)
            return Result.Failure<PollResponce>(PollErrors.DuplicatedPollTitle);
        
        currentPoll = request.Adapt(currentPoll);
        
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _context.Polls.FindAsync(id, cancellationToken);

        if (poll is null)
            return Result.Failure(PollErrors.PollNotFound);

        _context.Polls.Remove(poll);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> TogglePusblishStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _context.Polls.FindAsync(id, cancellationToken);
        if (poll is null)
            return Result.Failure(PollErrors.PollNotFound);

        poll.IsPublished = !poll.IsPublished;
        await _context.SaveChangesAsync(cancellationToken);

        if (poll.IsPublished && poll.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
        {
            BackgroundJob.Enqueue(() => _notificationService.SendNewPollsNotification(poll.Id));
        }

        return Result.Success();
    }


}
