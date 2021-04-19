using Application.Contracts.Repositories;
using Application.DTOs.Response;
using Core.Entities;
using Infrastructure.Authentication;
using Infrastructure.Config;
using Infrastructure.Contracts;
using InfrastructureTests.Mocks;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace InfrastructureTests
{
    public class JWTokenManagerTests
    {
        private readonly JWTokenManager _sut;
        private readonly IEncrypter _encrypterMock = new EncrypterMock();
        private readonly Mock<IUserRepo> _repoMock = new Mock<IUserRepo>();
        public JWTokenManagerTests()
        {
            var config = new TokenConfig
            {
                EncryptionKey = "This is testing encryption key, please have fun!"
            };

            _sut = new JWTokenManager(config, _repoMock.Object, _encrypterMock);
        }

        [Fact]
        public async Task Authorize_ShouldReturnNull_WhenPasswordDontMatch()
        {
            //Arrange
            string userName = "Oleg";
            string password = "password123";
            _repoMock.Setup(x => x.FindUserByUsername(userName)).ReturnsAsync(new User
            {
                PasswordHash = "notPassword123"
            });
            //Act
            var actual = await _sut.Authorize(userName, password, CancellationToken.None);
            //Assert
            Assert.Null(actual);
        }

        [Fact]
        public async Task Authorize_ShouldReturnToken_WhenPasswordMatch()
        {
            //Arrange
            string userName = "Oleg";
            string password = "password123";
            _repoMock.Setup(x => x.FindUserByUsername(userName)).ReturnsAsync(new User
            {
                PasswordHash = password,
                Username = userName,
                UserId = Guid.NewGuid()
            });
            _repoMock.Setup(x => x.SetRefreshTokenFor(It.IsAny<Guid>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            //Act
            JWTokenModel actual = await _sut.Authorize(userName, password, CancellationToken.None);
            //Assert
            Assert.NotNull(actual);
            AssertRefreshTokenBeenSet();
        }

        private void AssertRefreshTokenBeenSet()
        {
            _repoMock.Verify(x => x.SetRefreshTokenFor(It.IsAny<Guid>(), It.IsAny<string>()), Times.Once());
        }
    }
}
