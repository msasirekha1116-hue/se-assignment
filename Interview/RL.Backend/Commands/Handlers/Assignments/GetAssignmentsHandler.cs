using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Commands.Assignments;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;

namespace RL.Backend.Commands.Handlers.Assignments;

public class GetAssignmentsHandler : IRequestHandler<GetAssignmentsQuery, ApiResponse<List<AssignmentDto>>>
{
    private readonly RLContext _context;

    public GetAssignmentsHandler(RLContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<List<AssignmentDto>>> Handle(GetAssignmentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.PlanId < 1)
                return ApiResponse<List<AssignmentDto>>.Fail(new BadRequestException("Invalid PlanId"));

            var assignments = await _context.PlanProcedureUserAssignments
                .Where(x => x.PlanId == request.PlanId)
                .Select(x => new AssignmentDto
                {
                    ProcedureId = x.ProcedureId,
                    UserId = x.UserId
                })
                .ToListAsync(cancellationToken);

            return ApiResponse<List<AssignmentDto>>.Succeed(assignments);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<AssignmentDto>>.Fail(ex);
        }
    }
}
