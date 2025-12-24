
using Microsoft.AspNetCore.RateLimiting;
using SurveyBasket.Api.Contracts.Votes;

namespace SurveyBasket.Api.Controllers;
[Route("api/polls/{pollId}/vote")]
[ApiController]
[Authorize(Roles = DefaultRoles.Member)]
[EnableRateLimiting("concurrency")]
public class VotesController(IQuestionService questionService, IVoteService voteService) : ControllerBase
{
    private readonly IQuestionService _questionService = questionService;
    private readonly IVoteService _voteService = voteService;

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> Start([FromRoute] int pollId, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await _questionService.GetAvailableAsync(pollId, userId!, cancellationToken);

        if(result.IsSuccess)
            return Ok(result.Value);
        return result.ToProblem();
    }
    
    [HttpPost]
    [Route("")]
    public async Task<IActionResult> Vote([FromRoute] int pollId, [FromBody] VoteRequest voteRequest, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await _voteService.AddAsync(pollId, userId!, voteRequest, cancellationToken);

        if (result.IsSuccess)
            return Created();

        return result.ToProblem();
    }
}
