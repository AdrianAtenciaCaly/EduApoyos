using EduApoyos.Application.DTOs.SupportRequests;
using EduApoyos.Application.Features.SupportRequests.Commands;
using EduApoyos.Domain.Entities;
using EduApoyos.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace EduApoyos.Application.Tests
{
    public class CreateSupportRequestCommandHandlerTests
    {
        private readonly Mock<ISupportRequestRepository> _requestRepo = new();
        private readonly Mock<IStudentRepository> _studentRepo = new();
        private readonly Mock<IUnitOfWork> _uow = new();
        private readonly CreateSupportRequestCommandHandler _handler;

        public CreateSupportRequestCommandHandlerTests()
        {
            _handler = new CreateSupportRequestCommandHandler(
                _requestRepo.Object, _studentRepo.Object, _uow.Object);
        }

        [Fact]
        public async Task Handle_ValidRequest_CreatesPending_AndSaves()
        {
            var studentId = Guid.NewGuid();
            _studentRepo.Setup(x => x.GetByIdWithUserAsync(studentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(TestHelpers.CreateStudentWithUser(studentId));

            var result = await _handler.Handle(new CreateSupportRequestCommand(
                new CreateSupportRequestRequest(studentId, "Scholarship", 1_500_000m, "Need support"),
                Guid.NewGuid(), "Advisor"), CancellationToken.None);

            result.Status.Should().Be("Pending");
            result.Type.Should().Be("Scholarship");
            _requestRepo.Verify(x => x.AddAsync(It.IsAny<SupportRequest>(), It.IsAny<CancellationToken>()), Times.Once);
            _uow.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_StudentNotFound_Throws()
        {
            var id = Guid.NewGuid();
            _studentRepo.Setup(x => x.GetByIdWithUserAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Student?)null);

            var act = () => _handler.Handle(new CreateSupportRequestCommand(
                new CreateSupportRequestRequest(id, "Credit", 100m, "x"), Guid.NewGuid(), "Advisor"),
                CancellationToken.None);

            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task Handle_InvalidType_Throws()
        {
            var studentId = Guid.NewGuid();
            _studentRepo.Setup(x => x.GetByIdWithUserAsync(studentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(TestHelpers.CreateStudentWithUser(studentId));

            var act = () => _handler.Handle(new CreateSupportRequestCommand(
                new CreateSupportRequestRequest(studentId, "BadType", 100m, "x"), Guid.NewGuid(), "Advisor"),
                CancellationToken.None);

            await act.Should().ThrowAsync<ArgumentException>();
        }
    }
}
