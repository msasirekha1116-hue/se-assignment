
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RL.Backend.Commands.Assignments;
using RL.Backend.Commands.Handlers.Assignments;
using RL.Backend.Controllers;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RL.Backend.Tests.Controllers
{
    [TestClass]
    public class AssignmentsControllerTests
    {
        private Mock<IMediator> _mediatorMock;
        private AssignmentsController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new AssignmentsController(_mediatorMock.Object);
        }

        [TestMethod]
        public async Task AssignUser_Successful_ReturnsOkResult()
        {
            // Arrange
            var command = new AssignUserCommand { PlanId = 1, UserId = 1 };
            var apiResponse = new ApiResponse<Unit> { Value = Unit.Value };
            _mediatorMock.Setup(m => m.Send(It.IsAny<AssignUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.AssignUser(command);

            // Assert
            _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once, "The AssignUserCommand should be sent.");
            result.Should().BeOfType<OkResult>();
        }

        [TestMethod]
        public async Task AssignUser_HandlerReturnsNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            var command = new AssignUserCommand { PlanId = 99, UserId = 99 };
            var apiResponse = new ApiResponse<Unit> { Exception = new NotFoundException() };
            _mediatorMock.Setup(m => m.Send(It.IsAny<AssignUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.AssignUser(command);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [TestMethod]
        public async Task RemoveUser_Successful_ReturnsOkResult()
        {
            // Arrange
            var command = new RemoveUserCommand { PlanId = 1, UserId = 1 };
            var apiResponse = new ApiResponse<Unit> { Value = Unit.Value };
            _mediatorMock.Setup(m => m.Send(It.IsAny<RemoveUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.RemoveUser(command);

            // Assert
            _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once, "The RemoveUserCommand should be sent.");
            result.Should().BeOfType<OkResult>();
        }

        [TestMethod]
        public async Task ClearUsers_Successful_ReturnsOkResult()
        {
            // Arrange
            var command = new ClearUsersCommand { PlanId = 1 };
            var apiResponse = new ApiResponse<Unit> { Value = Unit.Value };
            _mediatorMock.Setup(m => m.Send(It.IsAny<ClearUsersCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.ClearUsers(command);

            // Assert
            _mediatorMock.Verify(m => m.Send(command, It.IsAny<CancellationToken>()), Times.Once, "The ClearUsersCommand should be sent.");
            result.Should().BeOfType<OkResult>();
        }

        [TestMethod]
        public async Task ClearUsers_HandlerReturnsBadRequest_ReturnsBadRequestResult()
        {
            // Arrange
            var command = new ClearUsersCommand { PlanId = -1 };
            var apiResponse = new ApiResponse<Unit> { Exception = new BadRequestException() };
            _mediatorMock.Setup(m => m.Send(It.IsAny<ClearUsersCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.ClearUsers(command);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [TestMethod]
        public async Task GetAssignments_GivenValidPlanId_ReturnsOkWithAssignments()
        {
            // Arrange
            var planId = 1;
            var expectedAssignments = new List<AssignmentDto> { new AssignmentDto { UserId = 1 }, new AssignmentDto { UserId = 2 } };
            var apiResponse = new ApiResponse<List<AssignmentDto>> { Value = expectedAssignments };

            _mediatorMock.Setup(m => m.Send(It.Is<GetAssignmentsQuery>(q => q.PlanId == planId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.GetAssignments(planId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.Is<GetAssignmentsQuery>(q => q.PlanId == planId), It.IsAny<CancellationToken>()), Times.Once, "The GetAssignmentsQuery should be sent.");
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result.As<OkObjectResult>();
            okResult.Value.Should().BeEquivalentTo(expectedAssignments);
        }

        [TestMethod]
        public async Task GetAssignments_WhenHandlerReturnsNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            var planId = 999; // Non-existent plan
            var apiResponse = new ApiResponse<List<AssignmentDto>> { Exception = new NotFoundException() };
            _mediatorMock.Setup(m => m.Send(It.Is<GetAssignmentsQuery>(q => q.PlanId == planId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _controller.GetAssignments(planId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}