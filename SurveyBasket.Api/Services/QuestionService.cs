using SurveyBasket.Api.Contracts.Answers;
using SurveyBasket.Api.Contracts.Questions;

namespace SurveyBasket.Api.Services;

public class QuestionService(ApplicationDbContext context) : IQuestionService
{
    private readonly ApplicationDbContext _context = context;
    
    public async Task<Result<IEnumerable<QuestionResponce>>> GetAllAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var pollISExists = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken);

        if (!pollISExists)
            return Result.Failure<IEnumerable<QuestionResponce>>(PollErrors.PollNotFound);

        var questions = await _context.Questions
                .Where(x => x.PollId == pollId)
                .Include(x => x.Answers)
                //.Select(q => new QuestionResponce(
                //    q.Id,
                //    q.Content,
                //    q.Answers.Select(a => new AnswerResponce(a.Id,a.Content))
                //))
                .ProjectToType<QuestionResponce>()
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<QuestionResponce>>(questions);
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

        return Result.Success();
    }

    public async Task<Result> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default)
    {
        var question = await _context.Questions.SingleOrDefaultAsync(x => x.Id == id && x.PollId == pollId, cancellationToken);
        if (question is null)
            return Result.Failure(QuestionErrors.QuestionNotFound);

        question.IsActive = !question.IsActive;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

}
