using MediatR;
using RL.Backend.Models;

namespace RL.Backend.Commands.Assignments;

public class GetAssignmentsQuery : IRequest<ApiResponse<List<AssignmentDto>>>
{
    public int PlanId { get; set; }
}

public class AssignmentDto
{
    public int ProcedureId { get; set; }
    public int UserId { get; set; }
}
