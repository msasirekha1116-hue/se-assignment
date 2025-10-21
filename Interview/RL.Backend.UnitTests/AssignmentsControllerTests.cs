
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Commands;
using RL.Backend.Commands.Assignments;
using RL.Backend.Commands.Handlers.Assignments;
using RL.Backend.Exceptions;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.UnitTests
{
    [TestClass]
    public class AssignmentsTests
    {
        private RLContext _context = null!;

        [TestInitialize]
        public void Setup()
        {
            _context = DbContextHelper.CreateContext();
        }

        [TestCleanup]
        public void Teardown()
        {
            _context.Dispose();
        }

        #region AssignUser
        [TestMethod]
        public async Task AssignUser_PlanProcedureAndUserExist_SuccessfullyAssignsUser()
        {
            // Arrange
            var handler = new AssignUserHandler(_context);
            var plan = new Plan { PlanId = 1 };
            var procedure = new Procedure { ProcedureId = 1, ProcedureTitle = "Test" };
            var user = new User { UserId = 1, Name = "Test User" };
            var planProcedure = new PlanProcedure { PlanId = 1, ProcedureId = 1 };
            _context.Plans.Add(plan);
            _context.Procedures.Add(procedure);
            _context.Users.Add(user);
            _context.PlanProcedures.Add(planProcedure);
            await _context.SaveChangesAsync();
            var command = new AssignUserCommand { PlanId = 1, ProcedureId = 1, UserId = 1 };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            var assignment = await _context.PlanProcedureUserAssignments.FirstOrDefaultAsync(ppu =>
                ppu.PlanId == command.PlanId && ppu.ProcedureId == command.ProcedureId && ppu.UserId == command.UserId);
            assignment.Should().NotBeNull();
        }

        [TestMethod]
        public async Task AssignUser_PlanProcedureNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var handler = new AssignUserHandler(_context);
            var command = new AssignUserCommand { PlanId = 99, ProcedureId = 99, UserId = 1 };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Exception.Should().BeOfType<NotFoundException>();
        }
        #endregion

        #region RemoveUser
        [TestMethod]
        public async Task RemoveUser_AssignmentExists_SuccessfullyRemovesUser()
        {
            // Arrange
            var handler = new RemoveUserHandler(_context);
            var assignment = new PlanProcedureUserAssignment { PlanId = 1, ProcedureId = 1, UserId = 1 };
            _context.PlanProcedureUserAssignments.Add(assignment);
            await _context.SaveChangesAsync();
            var command = new RemoveUserCommand { PlanId = 1, ProcedureId = 1, UserId = 1 };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            var deletedAssignment = await _context.PlanProcedureUserAssignments
                .FirstOrDefaultAsync(a => a.PlanId == 1 && a.ProcedureId == 1 && a.UserId == 1);
            deletedAssignment.Should().BeNull();
        }

        [TestMethod]
        public async Task RemoveUser_AssignmentNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var handler = new RemoveUserHandler(_context);
            var command = new RemoveUserCommand { PlanId = 99, ProcedureId = 99, UserId = 99 };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Exception.Should().BeOfType<NotFoundException>();
        }
        #endregion

        #region ClearUsers
        [TestMethod]
        public async Task ClearUsers_AssignmentsExist_SuccessfullyClearsUsers()
        {
            // Arrange
            var handler = new ClearUsersHandler(_context);
            _context.PlanProcedureUserAssignments.AddRange(
                new PlanProcedureUserAssignment { PlanId = 1, ProcedureId = 1, UserId = 1 },
                new PlanProcedureUserAssignment { PlanId = 1, ProcedureId = 1, UserId = 2 }
            );
            await _context.SaveChangesAsync();
            var command = new ClearUsersCommand { PlanId = 1, ProcedureId = 1 };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            var assignments = await _context.PlanProcedureUserAssignments.Where(ppu => ppu.PlanId == 1 && ppu.ProcedureId == 1).ToListAsync();
            assignments.Should().BeEmpty();
        }

        [TestMethod]
        public async Task ClearUsers_NoAssignmentsExist_ReturnsSuccess()
        {
            // Arrange
            var handler = new ClearUsersHandler(_context);
            var command = new ClearUsersCommand { PlanId = 1, ProcedureId = 1 };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
        }

        [TestMethod]
        public async Task ClearUsers_InvalidPlanId_ReturnsBadRequest()
        {
            // Arrange
            var handler = new ClearUsersHandler(_context);
            var command = new ClearUsersCommand { PlanId = -1, ProcedureId = 1 };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Exception.Should().BeOfType<BadRequestException>();
        }
        #endregion

        #region GetAssignments
        [TestMethod]
        public async Task GetAssignments_GivenValidPlanId_ReturnsAssignments()
        {
            // Arrange
            var handler = new GetAssignmentsHandler(_context);
            var planId = 1;
            _context.PlanProcedureUserAssignments.AddRange(
                new PlanProcedureUserAssignment { PlanId = planId, ProcedureId = 1, UserId = 101 },
                new PlanProcedureUserAssignment { PlanId = planId, ProcedureId = 2, UserId = 102 }
            );
            await _context.SaveChangesAsync();
            var query = new GetAssignmentsQuery { PlanId = planId };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().HaveCount(2);
            result.Value.Should().Contain(a => a.UserId == 101 && a.ProcedureId == 1);
            result.Value.Should().Contain(a => a.UserId == 102 && a.ProcedureId == 2);
        }

        [TestMethod]
        public async Task GetAssignments_NoAssignmentsForPlan_ReturnsEmptyList()
        {
            // Arrange
            var handler = new GetAssignmentsHandler(_context);
            var planId = 1;
            _context.Plans.Add(new Plan { PlanId = planId }); // Plan exists but has no assignments
            await _context.SaveChangesAsync();
            var query = new GetAssignmentsQuery { PlanId = planId };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Should().BeEmpty();
        }
        #endregion
    }
}