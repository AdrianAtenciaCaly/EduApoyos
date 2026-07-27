using EduApoyos.Application.Features.SupportRequests.Queries;
using EduApoyos.Domain.Entities;
using EduApoyos.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace EduApoyos.Application.Tests
{
    public class GetSupportRequestByIdQueryHandlerTests
    {
        private readonly Mock<ISupportRequestRepository> _req = new();
        private readonly Mock<IStudentRepository> _stu = new();
        private readonly GetSupportRequestByIdQueryHandler _handler;

        public GetSupportRequestByIdQueryHandlerTests()
            => _handler = new GetSupportRequestByIdQueryHandler(_req.Object, _stu.Object);

        [Fact]
        public async Task Advisor_CanViewAny()
        {
            var studentId = Guid.NewGuid();
            var student = TestHelpers.CreateStudentWithUser(studentId);
            var id = Guid.NewGuid();
            var entity = TestHelpers.CreateSupportRequest(studentId);
            TestHelpers.SetPrivateProperty(entity, nameof(SupportRequest.Id), id);
            TestHelpers.SetPrivateProperty(entity, nameof(SupportRequest.Student), student);
            _req.Setup(x => x.GetByIdWithDetailsAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

            var result = await _handler.Handle(
                new GetSupportRequestByIdQuery(id, Guid.NewGuid(), "Advisor"), CancellationToken.None);

            result.Id.Should().Be(id);
        }

        [Fact]
        public async Task Student_OtherRequest_ThrowsUnauthorized()
        {
            var owner = Guid.NewGuid();
            var other = Guid.NewGuid();
            var studentId = Guid.NewGuid();
            var student = TestHelpers.CreateStudentWithUser(studentId, owner);
            var id = Guid.NewGuid();
            var entity = TestHelpers.CreateSupportRequest(studentId);
            TestHelpers.SetPrivateProperty(entity, nameof(SupportRequest.Id), id);
            TestHelpers.SetPrivateProperty(entity, nameof(SupportRequest.Student), student);

            _req.Setup(x => x.GetByIdWithDetailsAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
            _stu.Setup(x => x.GetByUserIdAsync(other, It.IsAny<CancellationToken>()))
                .ReturnsAsync(TestHelpers.CreateStudentWithUser(Guid.NewGuid(), other));

            var act = () => _handler.Handle(
                new GetSupportRequestByIdQuery(id, other, "Student"), CancellationToken.None);

            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }
    }
}
