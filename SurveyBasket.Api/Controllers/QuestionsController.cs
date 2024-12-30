using SurveyBasket.Api.Contracts.Questions;


namespace SurveyBasket.Api.Controllers;
[Route("api/polls/{pollId}/[controller]")]
[ApiController]
[Authorize]
public class QuestionsController(IQuestionService questionService) : ControllerBase
{
    private readonly IQuestionService _questionService = questionService;

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetAll([FromRoute]int pollId, CancellationToken cancellationToken)
    {
        var questionsResult = await _questionService.GetAllAsync(pollId, cancellationToken);
        return questionsResult.IsSuccess
            ? Ok(questionsResult.Value)
            : questionsResult.ToProblem();
    }

    [HttpGet]
    [Route("{questionId}")]
    public async Task<IActionResult> Get([FromRoute] int pollId, [FromRoute]int questionId, CancellationToken cancellationToken)
    {
        var questionResult = await _questionService.GetAsync(pollId, questionId, cancellationToken);
        return questionResult.IsSuccess
            ? Ok(questionResult.Value)
            : questionResult.ToProblem();
    }

    [HttpPost]
    [Route("")]
    public async Task<IActionResult> Add([FromRoute]int pollId, [FromBody] QuestionRequest questionRequest, CancellationToken cancellationToken)
    { 
        var questionResult = await _questionService.AddAsync(pollId, questionRequest, cancellationToken);

        return questionResult.IsSuccess
            ? CreatedAtAction(nameof(Get), new {pollId, questionId = questionResult.Value.Id }, questionResult.Value)
            : questionResult.ToProblem();
    }


    [HttpPut]
    [Route("{requestId}")]
    public async Task<IActionResult> Update([FromRoute] int pollId,[FromRoute] int requestId, [FromBody] QuestionRequest questionRequest, CancellationToken cancellationToken)
    {
        var questionResult = await _questionService.UpdateAsync(pollId, requestId, questionRequest, cancellationToken);

        return questionResult.IsSuccess
            ? NoContent()
            : questionResult.ToProblem();
    }

    [HttpPut]
    [Route("{id}/ToggleStatus")]
    public async Task<IActionResult> ToggleStatus([FromRoute] int pollId, [FromRoute] int id, CancellationToken cancellationToken)
    {
        var questionResult = await _questionService.ToggleStatusAsync(pollId, id, cancellationToken);
         
        return questionResult.IsSuccess
            ? NoContent()
            : questionResult.ToProblem();
    }


}
