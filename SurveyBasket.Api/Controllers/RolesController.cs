using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Api.Contracts.Roles;

namespace SurveyBasket.Api.Controllers;

[ApiVersion(1, Deprecated = true)]
[ApiVersion(2)]
[Route("api/[controller]")]
[ApiController]
public class RolesController(IRoleService roleService) : ControllerBase
{
    private readonly IRoleService _roleService = roleService;

    [HttpGet("")]
    [HasPermisssion(Permissions.GetRoles)]
    public async Task<IActionResult> GetAll([FromQuery] bool includeDisabled, CancellationToken cancellationToken)
    {
        var roles = await _roleService.GetAllAsync(includeDisabled, cancellationToken);

        return Ok(roles);
    }

    [HttpGet("{id}")]
    [HasPermisssion(Permissions.GetRoles)]
    public async Task<IActionResult> Get([FromRoute] string id)
    {
        var result = await _roleService.GetAsync(id);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("")]
    [HasPermisssion(Permissions.AddRoles)]
    public async Task<IActionResult> Add([FromBody] RoleRequest request)
    {
        var result = await _roleService.AddRole(request);

        return result.IsSuccess ? CreatedAtAction(nameof(Get), new { result.Value.Id }, result.Value): result.ToProblem();
    }

    [HttpPut("{id}")]
    [HasPermisssion(Permissions.UpdateRoles)]
    public async Task<IActionResult> Update([FromRoute] string id, [FromBody] RoleRequest request)
    {
        var result = await _roleService.UpdateRole(id, request);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
    
    [HttpPut("{id}/toggle-status")]
    [HasPermisssion(Permissions.UpdateRoles)]
    public async Task<IActionResult> ToggleStatus([FromRoute] string id, CancellationToken cancellationToken)
    {
        var result = await _roleService.ToggleRoleStatusAsync(id, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
