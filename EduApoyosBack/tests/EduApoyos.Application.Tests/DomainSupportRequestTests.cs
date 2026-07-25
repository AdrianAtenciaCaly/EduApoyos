using EduApoyos.Domain.Entities;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyos.Application.Tests
{
    public class DomainSupportRequestTests
    {
        [Fact]
        public void ChangeStatus_AddsHistory()
        {
            var r = new SupportRequest(Guid.NewGuid(), SupportType.Scholarship, 1000m, "t");
            var advisor = Guid.NewGuid();
            r.ChangeStatus(RequestStatus.UnderReview, advisor, "go");

            r.Status.Should().Be(RequestStatus.UnderReview);
            r.StatusHistories.Should().HaveCount(1);
        }

        [Fact]
        public void InvalidAmount_Throws()
        {
            var act = () => new SupportRequest(Guid.NewGuid(), SupportType.Credit, 0m, "t");
            act.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
