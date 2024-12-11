namespace SurveyBasket.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService _pollService = pollService;

    [HttpGet]
    [Route("")]
    public IActionResult GetAll()
    {
        var polls = _pollService.GetAll();

        var response = polls.Adapt<IEnumerable<PollResponce>>();
        return Ok(response);
    }

    [HttpGet]
    [Route("{id}")]
    public IActionResult Get([FromRoute] int id)
    {
        var poll = _pollService.Get(id);

        if(poll is null)
            return NotFound();

        var responce = poll.Adapt<PollResponce>();
        return Ok(responce);
    }

    [HttpPost]
    [Route("")]
    public IActionResult Add([FromBody] CreatePollRequest request)
    {
        var poll = request.Adapt<Poll>();
        Poll newPoll = _pollService.Add(poll);

        return CreatedAtAction(nameof(Get), new { Id = newPoll.Id }, newPoll);
    }

    [HttpPut]
    [Route("{id}")]
    public IActionResult Update([FromRoute] int id, [FromBody] CreatePollRequest request)
    {
        var poll = request.Adapt<Poll>();
        var isUpdated = _pollService.Update(id, poll);

        if(!isUpdated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete]
    [Route("{id}")]
    public IActionResult Delete([FromRoute] int id)
    {
        var isDeleted = _pollService.Delete(id);

        if(!isDeleted)
            return NotFound();
        return NoContent();
    }
}
