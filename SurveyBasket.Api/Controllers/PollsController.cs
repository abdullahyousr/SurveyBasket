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
        return Ok(_pollService.GetAll());
    }

    [HttpGet]
    [Route("{id}")]
    public IActionResult Get(int id)
    {
        var poll = _pollService.Get(id);
        return poll is null ? BadRequest() : Ok(poll);
    }

    [HttpPost]
    [Route("")]
    public IActionResult Add(Poll request)
    {
        Poll newPoll = _pollService.Add(request);

        return CreatedAtAction(nameof(Get), new { Id = newPoll.Id }, newPoll);
    }

    [HttpPut]
    [Route("{id}")]
    public IActionResult Update(int id, Poll request)
    {
        var isUpdated = _pollService.Update(id, request);

        if(!isUpdated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete]
    [Route("{id}")]
    public IActionResult Delete(int id)
    {
        var isDeleted = _pollService.Delete(id);

        if(!isDeleted)
            return NotFound();
        return NoContent();
    }
}
