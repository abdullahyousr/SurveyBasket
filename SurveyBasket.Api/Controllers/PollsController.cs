using Microsoft.AspNetCore.Authorization;

namespace SurveyBasket.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService _pollService = pollService;

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var pollsResponce = await _pollService.GetAllAsync(cancellationToken);

        return Ok(pollsResponce);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        var pollResult = await _pollService.GetAsync(id, cancellationToken);

        return pollResult.IsSuccess
            ? Ok(pollResult.Value)
            : pollResult.ToProblem();
    }

    [HttpPost]
    [Route("")]
    public async Task<IActionResult> Add([FromBody] PollRequest request, CancellationToken cancellationToken)
    {
        var pollResult = await _pollService.AddAsync(request, cancellationToken);

        return pollResult.IsSuccess
            ? CreatedAtAction(nameof(Get), new { pollResult.Value.Id }, pollResult.Value)
            : pollResult.ToProblem();
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request, CancellationToken cancellationToken)
    {
        var pollResult = await _pollService.UpdateAsync(id, request, cancellationToken);

        return pollResult.IsSuccess 
            ? NoContent()
            : pollResult.ToProblem();
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var pollResult = await _pollService.DeleteAsync(id, cancellationToken);

        return pollResult.IsSuccess 
            ? NoContent()
            : pollResult.ToProblem();
    }


    [HttpPut]
    [Route("{id}/togglePublish")]
    public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellationToken)
    {
        var pollResult = await _pollService.TogglePusblishStatusAsync(id, cancellationToken);

        return pollResult.IsSuccess 
            ? NoContent()
            : pollResult.ToProblem();
    }
}
