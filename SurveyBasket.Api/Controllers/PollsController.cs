

using Asp.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace SurveyBasket.Api.Controllers;

[ApiVersion(1,Deprecated = true)]
[ApiVersion(2)]
[Route("api/[controller]")]
//[Route("api/v{v:apiVersion}/[controller]")]
[ApiController]
public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService _pollService = pollService;

    [HttpGet]
    [Route("")]
    [HasPermisssion(Permissions.GetPolls)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var pollsResponce = await _pollService.GetAllAsync(cancellationToken);

        return Ok(pollsResponce);
    }

    [HttpGet]
    [MapToApiVersion(1)]
    [Route("current")]
    [Authorize(Roles = DefaultRoles.Member.Name)]
    [EnableRateLimiting(RateLimiters.Concurrency)]
    [SwaggerIgnore]
    public async Task<IActionResult> GetCurrentV1(CancellationToken cancellationToken)
    {
        var pollsResponce = await _pollService.GetCurrentAsyncV1(cancellationToken);

        return Ok(pollsResponce);
    }

    [HttpGet]
    [MapToApiVersion(2)]
    [Route("current")]
    [Authorize(Roles = DefaultRoles.Member.Name)]
    [EnableRateLimiting(RateLimiters.Concurrency)]
    public async Task<IActionResult> GetCurrentV2(CancellationToken cancellationToken)
    {
        var pollsResponce = await _pollService.GetCurrentAsyncV2(cancellationToken);

        return Ok(pollsResponce);
    }

    [HttpGet]
    [Route("{id}")]
    [HasPermisssion(Permissions.GetPolls)]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        var pollResult = await _pollService.GetAsync(id, cancellationToken);

        return pollResult.IsSuccess
            ? Ok(pollResult.Value)
            : pollResult.ToProblem();
    }

    [HttpPost]
    [Route("")]
    [HasPermisssion(Permissions.AddPolls)]
    public async Task<IActionResult> Add([FromBody] PollRequest request, CancellationToken cancellationToken)
    {
        var pollResult = await _pollService.AddAsync(request, cancellationToken);

        return pollResult.IsSuccess
            ? CreatedAtAction(nameof(Get), new { pollResult.Value.Id }, pollResult.Value)
            : pollResult.ToProblem();
    }

    [HttpPut]
    [Route("{id}")]
    [HasPermisssion(Permissions.UpdatePolls)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request, CancellationToken cancellationToken)
    {
        var pollResult = await _pollService.UpdateAsync(id, request, cancellationToken);

        return pollResult.IsSuccess 
            ? NoContent()
            : pollResult.ToProblem();
    }

    [HttpDelete]
    [Route("{id}")]
    [HasPermisssion(Permissions.DeletePolls)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var pollResult = await _pollService.DeleteAsync(id, cancellationToken);

        return pollResult.IsSuccess 
            ? NoContent()
            : pollResult.ToProblem();
    }


    [HttpPut]
    [Route("{id}/toggle-publish")]
    [HasPermisssion(Permissions.UpdatePolls)]
    public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellationToken)
    {
        var pollResult = await _pollService.TogglePusblishStatusAsync(id, cancellationToken);

        return pollResult.IsSuccess 
            ? NoContent()
            : pollResult.ToProblem();
    }
}
