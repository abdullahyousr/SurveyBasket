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
        var polls = await _pollService.GetAllAsync(cancellationToken);

        var response = polls.Adapt<IEnumerable<PollResponce>>();
        return Ok(response);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        var poll = await _pollService.GetAsync(id, cancellationToken);

        if (poll is null)
            return NotFound();

        var responce = poll.Adapt<PollResponce>();
        return Ok(responce);
    }

    [HttpPost]
    [Route("")]
    public async Task<IActionResult> Add([FromBody] PollRequest request, CancellationToken cancellationToken)
    {
        var poll = request.Adapt<Poll>();
        Poll newPoll = await _pollService.AddAsync(poll, cancellationToken);

        return CreatedAtAction(nameof(Get), new { Id = newPoll.Id }, newPoll.Adapt<PollResponce>());
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest request, CancellationToken cancellationToken)
    {
        var poll = request.Adapt<Poll>();
        var isUpdated = await _pollService.UpdateAsync(id, poll, cancellationToken);

        if (!isUpdated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var isDeleted = await _pollService.DeleteAsync(id, cancellationToken);

        if (!isDeleted)
            return NotFound();
        return NoContent();
    }


    [HttpPut]
    [Route("{id}/togglePublish")]
    public async Task<IActionResult> TogglePublish([FromRoute] int id, CancellationToken cancellationToken)
    {
        var isUpdated = await _pollService.TogglePusblishStatusAsync(id, cancellationToken);

        if (!isUpdated)
            return NotFound();
        return NoContent();
    }
}
