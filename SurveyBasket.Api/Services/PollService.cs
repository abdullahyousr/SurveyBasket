namespace SurveyBasket.Api.Services;

public class PollService(ApplicationDbContext context) : IPollService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<PollResponce>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Polls
                    .AsNoTracking()
                    .ProjectToType<PollResponce>()
                    .ToListAsync(cancellationToken);

    public async Task<IEnumerable<PollResponce>> GetCurrentAsync(CancellationToken cancellationToken = default)
        => await _context.Polls
                    .Where(p => p.IsPublished &&  p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && p.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow))
                    .AsNoTracking()
                    .ProjectToType<PollResponce>()
                    .ToListAsync(cancellationToken);

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

        currentPoll.Title = request.Title;
        currentPoll.Summary = request.Summary;
        currentPoll.StartsAt = request.StartsAt;
        currentPoll.EndsAt = request.EndsAt;
        
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

        return Result.Success();
    }


}
