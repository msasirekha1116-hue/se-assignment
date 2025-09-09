using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Commands.Assignments;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;

namespace RL.Backend.Commands.Handlers.Assignments;

public class RemoveUserHandler : IRequestHandler<RemoveUserCommand, ApiResponse<Unit>>
{
    private readonly RLContext _context;

    public RemoveUserHandler(RLContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<Unit>> Handle(RemoveUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.PlanId < 1 || request.ProcedureId < 1 || request.UserId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid input values"));

            var assignment = await _context.PlanProcedureUserAssignments.FirstOrDefaultAsync(x =>
                x.PlanId == request.PlanId &&
                x.ProcedureId == request.ProcedureId &&
                x.UserId == request.UserId, cancellationToken);

            if (assignment == null)
                return ApiResponse<Unit>.Fail(new NotFoundException("Assignment not found"));

            _context.PlanProcedureUserAssignments.Remove(assignment);
            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Unit>.Succeed(Unit.Value);
        }
        catch (Exception ex)
        {
            return ApiResponse<Unit>.Fail(ex);
        }
    }
}
