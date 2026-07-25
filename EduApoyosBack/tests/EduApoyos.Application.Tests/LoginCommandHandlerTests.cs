using EduApoyos.Application.DTOs.Auth;
using EduApoyos.Application.Features.Auth.Commands;
using EduApoyos.Application.Interfaces;
using EduApoyos.Domain.Entities;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduApoyos.Application.Tests
{
    public class LoginCommandHandlerTests
    {
        private readonly Mock<IIdentityService> _identity = new();
        private readonly Mock<IJwtTokenService> _jwt = new();
        private readonly LoginCommandHandler _handler;

        public LoginCommandHandlerTests()
            => _handler = new LoginCommandHandler(_identity.Object, _jwt.Object);

        [Fact]
        public async Task Handle_ValidCredentials_ReturnsToken()
        {
            var user = new User("Carlos", "a@test.com", "hash", UserRole.Advisor);
            user.SetId(Guid.NewGuid());
            _identity.Setup(x => x.ValidateCredentialsAsync("a@test.com", "pass", It.IsAny<CancellationToken>()))
                .ReturnsAsync((true, user));
            _jwt.Setup(x => x.GenerateToken(user)).Returns("jwt");
            _jwt.Setup(x => x.GetExpiration()).Returns(DateTime.UtcNow.AddHours(8));

            var result = await _handler.Handle(
                new LoginCommand(new LoginRequest("a@test.com", "pass")), CancellationToken.None);

            result.Token.Should().Be("jwt");
            result.Role.Should().Be("Advisor");
        }

        [Fact]
        public async Task Handle_Invalid_ThrowsUnauthorized()
        {
            _identity.Setup(x => x.ValidateCredentialsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((false, (User?)null));

            var act = () => _handler.Handle(
                new LoginCommand(new LoginRequest("x@y.com", "bad")), CancellationToken.None);

            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }
    }
}
