using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Commands.Assignments;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;

namespace RL.Backend.Commands.Handlers.Assignments;

public class ClearUsersHandler : IRequestHandler<ClearUsersCommand, ApiResponse<Unit>>
{
    private readonly RLContext _context;

    public ClearUsersHandler(RLContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<Unit>> Handle(ClearUsersCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.PlanId < 1 || request.ProcedureId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid PlanId or ProcedureId"));

            var users = await _context.PlanProcedureUserAssignments
                .Where(x => x.PlanId == request.PlanId && x.ProcedureId == request.ProcedureId)
                .ToListAsync(cancellationToken);

            if (!users.Any())
                return ApiResponse<Unit>.Succeed(Unit.Value);

            _context.PlanProcedureUserAssignments.RemoveRange(users);
            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Unit>.Succeed(Unit.Value);
        }
        catch (Exception ex)
        {
            return ApiResponse<Unit>.Fail(ex);
        }
    }
}
