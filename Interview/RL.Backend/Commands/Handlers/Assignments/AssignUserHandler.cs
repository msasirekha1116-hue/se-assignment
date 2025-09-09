using MediatR;
using RL.Backend.Models;
using RL.Backend.Commands.Assignments;
using RL.Data;
using RL.Data.DataModels;
using RL.Backend.Exceptions;

namespace RL.Backend.Commands.Handlers.Assignments;

public class AssignUserHandler : IRequestHandler<AssignUserCommand, ApiResponse<Unit>>
{
    private readonly RLContext _context;

    public AssignUserHandler(RLContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<Unit>> Handle(AssignUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.PlanId < 1 || request.ProcedureId < 1 || request.UserId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid PlanId, ProcedureId, or UserId"));

            var plan = await _context.Plans.FindAsync(request.PlanId);
            var procedure = await _context.Procedures.FindAsync(request.ProcedureId);
            var user = await _context.Users.FindAsync(request.UserId);

            if (plan == null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"Plan {request.PlanId} not found"));
            if (procedure == null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"Procedure {request.ProcedureId} not found"));
            if (user == null)
                return ApiResponse<Unit>.Fail(new NotFoundException($"User {request.UserId} not found"));

            var exists = _context.PlanProcedureUserAssignments.Any(x =>
                x.PlanId == request.PlanId &&
                x.ProcedureId == request.ProcedureId &&
                x.UserId == request.UserId);

            if (exists)
                return ApiResponse<Unit>.Succeed(Unit.Value);

            var assignment = new PlanProcedureUserAssignment
            {
                PlanId = request.PlanId,
                ProcedureId = request.ProcedureId,
                UserId = request.UserId,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow
            };

            _context.PlanProcedureUserAssignments.Add(assignment);
            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<Unit>.Succeed(Unit.Value);
        }
        catch (Exception ex)
        {
            return ApiResponse<Unit>.Fail(ex);
        }
    }
}
