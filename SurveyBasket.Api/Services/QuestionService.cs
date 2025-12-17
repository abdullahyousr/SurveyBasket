using SurveyBasket.Api.Contracts.Answers;
using SurveyBasket.Api.Contracts.Common;
using SurveyBasket.Api.Contracts.Questions;
using System.Linq.Dynamic.Core;

namespace SurveyBasket.Api.Services;

public class QuestionService(ApplicationDbContext context, ICasheService cashService, ILogger<QuestionService> logger) : IQuestionService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ICasheService _cashService = cashService;
    private readonly ILogger<QuestionService> _logger = logger;

    private const string _cashePrefix = "availableQuestions";

    public async Task<Result<PaginatedList<QuestionResponce>>> GetAllAsync(int pollId, RequestFilters requestFilter, CancellationToken cancellationToken = default)
    {
        var pollISExists = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken);

        if (!pollISExists)
            return Result.Failure<PaginatedList<QuestionResponce>>(PollErrors.PollNotFound);

        var query = _context.Questions
                .Where(x => x.PollId == pollId && (string.IsNullOrEmpty(requestFilter.SearchValue) || x.Content.Contains(requestFilter.SearchValue)));
                    if (!string.IsNullOrEmpty(requestFilter.sortColumn))
                    {
                        query = query.OrderBy($"{requestFilter.sortColumn} {requestFilter.sortDirection}");

                    }
                    var source = query
                            .Include(x => x.Answers)
                            //.Select(q => new QuestionResponce(
                            //    q.Id,
                            //    q.Content,
                            //    q.Answers.Select(a => new AnswerResponce(a.Id,a.Content))
                            //))
                            .ProjectToType<QuestionResponce>()
                            .AsNoTracking();

        var questions = await PaginatedList<QuestionResponce>.CreateAsync(source, requestFilter.PageNumber, requestFilter.PageSize, cancellationToken);

        return Result.Success(questions);
    }

    public async Task<Result<IEnumerable<QuestionResponce>>> GetAvailableAsync(int pollId, string userId, CancellationToken cancellationToken = default)
    {
        var hasVote = await _context.Votes
                   .AnyAsync(v => v.PollId == pollId && v.UserId == userId, cancellationToken);

        if (hasVote)
            return Result.Failure<IEnumerable<QuestionResponce>>(VoteErrors.DuplicatedVote);

        var pollIsExists = await _context.Polls.AnyAsync(p => p.Id == pollId && p.IsPublished && p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && p.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);

        if (!pollIsExists)
            return Result.Failure<IEnumerable<QuestionResponce>>(PollErrors.PollNotFound);

        var casheKey = $"{_cashePrefix}:{pollId}";
        var cashedQuestions = await _cashService.GetAsync<IEnumerable<QuestionResponce>>(casheKey, cancellationToken);

        IEnumerable<QuestionResponce> questions = []; 
        if (cashedQuestions is null)
        {
            _logger.LogInformation("Questions not found in cashe, loading from db...");
            questions = await _context.Questions
                .Where(q => q.IsActive && q.PollId == pollId)
                .Include(q => q.Answers)
                .Select(q => new QuestionResponce(
                    q.Id,
                    q.Content,
                    q.Answers.Where(a => a.IsActive).Select(a => new AnswerResponce(a.Id, a.Content))
                ))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            await _cashService.SetAsync(casheKey, questions, cancellationToken);
        }
        else
        {
            _logger.LogInformation("Questions found in cashe, loading from cashe...");
            questions = cashedQuestions;
        }

        return Result.Success(questions);
    }

    public async Task<Result<QuestionResponce>> GetAsync(int pollId, int id, CancellationToken cancellationToken = default)
    {
        var question = await _context.Questions
            .Where(x => x.Id == id && x.PollId == pollId)
            .Include(x => x.Answers)
            .ProjectToType<QuestionResponce>()
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

        if(question is null)
            return Result.Failure<QuestionResponce>(QuestionErrors.QuestionNotFound);

        return Result.Success(question);
    }

    public async Task<Result<QuestionResponce>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken = default)
    {
        var pollISExists = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken);

        if (!pollISExists)
            return Result.Failure<QuestionResponce>(PollErrors.PollNotFound);

        var questionISExists = await _context.Questions.AnyAsync(x => x.Content == request.Content && x.PollId == pollId, cancellationToken);

        if (questionISExists)
            return Result.Failure<QuestionResponce>(QuestionErrors.DuplicatedQuestionContent);

        var question = request.Adapt<Question>();
        question.PollId = pollId;

        await _context.AddAsync(question,cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await _cashService.RemoveAsync($"{_cashePrefix}:{pollId}", cancellationToken);

        return Result.Success(question.Adapt<QuestionResponce>());
    }

    public async Task<Result> UpdateAsync(int pollId, int id, QuestionRequest request, CancellationToken cancellationToken = default)
    {
        var questionIsExist = await _context.Questions.AnyAsync(x => x.PollId == pollId 
                    && x.Id != id
                    && x.Content == request.Content, 
                    cancellationToken);

        if (questionIsExist)
            return Result.Failure(QuestionErrors.DuplicatedQuestionContent);

        var question = await _context.Questions
                .Include(x => x.Answers)
                .SingleOrDefaultAsync(x => x.PollId == pollId && x.Id == id, cancellationToken);

        if(question is null)
            return Result.Failure(QuestionErrors.QuestionNotFound);

        question.Content = request.Content;
        
        var currentAnswers = _context.Answers.Select(x => x.Content).ToList();

        var newAnswers = request.Answers.Except(currentAnswers).ToList();

        newAnswers.ForEach(answer =>
            question.Answers.Add(new Answer { Content = answer})
            );

        question.Answers.ToList().ForEach(answer =>
        {
            answer.IsActive = request.Answers.Contains(answer.Content);
        });

        await _context.SaveChangesAsync(cancellationToken);

        await _cashService.RemoveAsync($"{_cashePrefix}:{pollId}", cancellationToken);

        return Result.Success();
    }

    public async Task<Result> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default)
    {
        var question = await _context.Questions.SingleOrDefaultAsync(x => x.Id == id && x.PollId == pollId, cancellationToken);
        if (question is null)
            return Result.Failure(QuestionErrors.QuestionNotFound);

        question.IsActive = !question.IsActive;
        await _context.SaveChangesAsync(cancellationToken);

        await _cashService.RemoveAsync($"{_cashePrefix}:{pollId}", cancellationToken);

        return Result.Success();
    }

}
