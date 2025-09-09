using Microsoft.AspNetCore.Mvc;
using MediatR;
using RL.Backend.Commands.Assignments;
using RL.Backend.Commands.Handlers.Assignments;
using RL.Backend.Models;

[ApiController]
[Route("api/[controller]")]
public class AssignmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AssignmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignUser([FromBody] AssignUserCommand command)
    {
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> RemoveUser([FromBody] RemoveUserCommand command)
    {
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> ClearUsers([FromBody] ClearUsersCommand command)
    {
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpGet("{planId:int}")]
    public async Task<IActionResult> GetAssignments(int planId)
    {
        var result = await _mediator.Send(new GetAssignmentsQuery { PlanId = planId });
        return result.ToActionResult();
    }


}
