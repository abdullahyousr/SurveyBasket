namespace SurveyBasket.Api.Services;

public interface IPollService
{
    Task<IEnumerable<PollResponce>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<PollResponce>> GetCurrentAsyncV1(CancellationToken cancellationToken = default);
    Task<IEnumerable<PollResponceV2>> GetCurrentAsyncV2(CancellationToken cancellationToken = default);
    
    Task<Result<PollResponce>> GetAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<PollResponce>> AddAsync(PollRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(int id, PollRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
    
    Task<Result> TogglePusblishStatusAsync(int id, CancellationToken cancellationToken = default);
}
