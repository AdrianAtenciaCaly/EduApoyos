using EduApoyos.Application.Features.SupportRequests.Queries;
using EduApoyos.Domain.Entities;
using EduApoyos.Domain.Interfaces;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyos.Application.Tests
{
    public class GetSupportRequestsByStudentQueryHandlerTests
    {
        [Fact]
        public async Task OwnStudent_ReturnsList()
        {
            var userId = Guid.NewGuid();
            var studentId = Guid.NewGuid();
            var student = TestHelpers.CreateStudentWithUser(studentId, userId);
            var entity = TestHelpers.CreateSupportRequest(studentId);
            TestHelpers.SetPrivateProperty(entity, nameof(SupportRequest.Student), student);

            var reqRepo = new Mock<ISupportRequestRepository>();
            var stuRepo = new Mock<IStudentRepository>();
            stuRepo.Setup(x => x.GetByIdWithUserAsync(studentId, It.IsAny<CancellationToken>())).ReturnsAsync(student);
            reqRepo.Setup(x => x.GetByStudentIdAsync(studentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<SupportRequest> { entity });

            var handler = new GetSupportRequestsByStudentQueryHandler(reqRepo.Object, stuRepo.Object);
            var result = await handler.Handle(new GetSupportRequestsByStudentQuery(studentId, userId), CancellationToken.None);

            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task OtherStudent_ThrowsUnauthorized()
        {
            var studentId = Guid.NewGuid();
            var student = TestHelpers.CreateStudentWithUser(studentId, Guid.NewGuid());
            var stuRepo = new Mock<IStudentRepository>();
            stuRepo.Setup(x => x.GetByIdWithUserAsync(studentId, It.IsAny<CancellationToken>())).ReturnsAsync(student);

            var handler = new GetSupportRequestsByStudentQueryHandler(
                new Mock<ISupportRequestRepository>().Object, stuRepo.Object);

            var act = () => handler.Handle(
                new GetSupportRequestsByStudentQuery(studentId, Guid.NewGuid()), CancellationToken.None);

            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }
    }
}
