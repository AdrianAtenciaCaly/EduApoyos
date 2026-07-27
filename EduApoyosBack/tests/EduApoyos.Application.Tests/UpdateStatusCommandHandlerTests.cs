using EduApoyos.Application.DTOs.SupportRequests;
using EduApoyos.Application.Features.SupportRequests.Commands;
using EduApoyos.Domain.Entities;
using EduApoyos.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace EduApoyos.Application.Tests
{
    public class UpdateStatusCommandHandlerTests
    {
        private readonly Mock<ISupportRequestRepository> _repo = new();
        private readonly UpdateStatusCommandHandler _handler;

        public UpdateStatusCommandHandlerTests() => _handler = new UpdateStatusCommandHandler(_repo.Object);

        [Fact]
        public async Task Handle_ValidStatus_ReturnsDto()
        {
            var id = Guid.NewGuid();
            var advisorId = Guid.NewGuid();
            var student = TestHelpers.CreateStudentWithUser();
            var entity = TestHelpers.CreateSupportRequest(student.Id);
            TestHelpers.SetPrivateProperty(entity, nameof(SupportRequest.Id), id);
            TestHelpers.SetPrivateProperty(entity, nameof(SupportRequest.Student), student);
            entity.ChangeStatus(RequestStatus.UnderReview, advisorId, "ok");

            _repo.Setup(x => x.ChangeStatusAsync(id, RequestStatus.UnderReview, advisorId, "ok", It.IsAny<CancellationToken>()))
                .ReturnsAsync(entity);

            var result = await _handler.Handle(
                new UpdateStatusCommand(id, new UpdateStatusRequest("UnderReview", "ok"), advisorId),
                CancellationToken.None);

            result.Status.Should().Be("UnderReview");
        }

        [Fact]
        public async Task Handle_InvalidStatus_Throws()
        {
            var act = () => _handler.Handle(
                new UpdateStatusCommand(Guid.NewGuid(), new UpdateStatusRequest("Nope", null), Guid.NewGuid()),
                CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>();
        }
    }
}
