using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SurveyBasket.Api.Controllers;

[ApiVersion(1, Deprecated = true)]
[ApiVersion(2)]
[Route("api/polls/{pollId}/[controller]")]
[ApiController]
[HasPermisssion(Permissions.Results)]
public class ResultsController(IResultService resultService) : ControllerBase
{
    private readonly IResultService _resultService = resultService;

    [HttpGet]
    [Route("row-data")]
    public async Task<IActionResult> PollVotes([FromRoute] int pollId, CancellationToken cancellationToken)
    {
        var result = await _resultService.GetPollVotesAsync(pollId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }


    [HttpGet]
    [Route("votes-per-day")]
    public async Task<IActionResult> VotesPerDay([FromRoute] int pollId, CancellationToken cancellationToken)
    {
        var result = await _resultService.GetVotesPerDayAsync(pollId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet]
    [Route("votes-per-question")]
    public async Task<IActionResult> VotesPerQuestion([FromRoute] int pollId, CancellationToken cancellationToken)
    {
        var result = await _resultService.GetVotesPerQuestionAsync(pollId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}
