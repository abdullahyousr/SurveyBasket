using SurveyBasket.Api.Contracts.Roles;

namespace SurveyBasket.Api.Services;

public interface IRoleService
{
    Task<IEnumerable<RoleResponce>> GetAllAsync(bool includeDisabled = false, CancellationToken cancellationToken = default);
    Task<Result<RoleDetailResponse>> GetAsync(string id);
    Task<Result<RoleDetailResponse>> AddRole(RoleRequest request);
    Task<Result> UpdateRole(string id, RoleRequest request);
    Task<Result> ToggleRoleStatusAsync(string id, CancellationToken cancellationToken = default);
}
