using SurveyBasket.Api.Contracts.Questions;

namespace SurveyBasket.Api.Services;

public interface IQuestionService
{
    Task<Result<IEnumerable<QuestionResponce>>> GetAllAsync(int pollId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<QuestionResponce>>> GetAvailableAsync(int pollId, string userId, CancellationToken cancellationToken = default);
    Task<Result<QuestionResponce>> GetAsync(int pollId, int id, CancellationToken cancellationToken = default);
    Task<Result<QuestionResponce>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(int pollId, int id, QuestionRequest request, CancellationToken cancellationToken = default);
    Task<Result> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default);
}
