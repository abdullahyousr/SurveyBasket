using Asp.Versioning;
using SurveyBasket.Api.Contracts.Users;

namespace SurveyBasket.Api.Controllers;


[ApiVersion(1, Deprecated = true)]
[ApiVersion(2)]
[Route("api/[controller]")]
[ApiController]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet("")]
    [HasPermisssion(Permissions.GetUsers)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _userService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id}")]
    [HasPermisssion(Permissions.GetUsers)]
    public async Task<IActionResult> Get([FromRoute]string id)
    {
        var result = await _userService.GetAsync(id);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("")]
    [HasPermisssion(Permissions.AddUsers)]
    public async Task<IActionResult> Add([FromBody] CreateUserRequest userRequest, CancellationToken cancellationToken)
    {
        var result = await _userService.AddAsync(userRequest,cancellationToken);

        return result.IsSuccess ? CreatedAtAction(nameof(Get), new { result.Value.Id }, result.Value) : result.ToProblem();
    }

    [HttpPut("{id}")]
    [HasPermisssion(Permissions.UpdateUsers)]
    public async Task<IActionResult> Update([FromRoute] string id,[FromBody] UpdateUserRequest userRequest, CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateAsync(id, userRequest, cancellationToken);

        return result.IsSuccess 
            ?  NoContent() 
            : result.ToProblem();
    }

    [HttpPut("{id}/toggle-status")]
    [HasPermisssion(Permissions.UpdateUsers)]
    public async Task<IActionResult> Toggle([FromRoute] string id)
    {
        var result = await _userService.ToggleUserStatus(id);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }


    [HttpPut("{id}/unlock-login")]
    public async Task<IActionResult> Unlock([FromRoute] string id)
    {
        var result = await _userService.UnlocKUser(id);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}
