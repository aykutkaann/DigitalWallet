using DigitalWallet.Application.DTOs.Auth;
using DigitalWallet.Application.Interfaces;
using DigitalWallet.Application.Services;
using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.Enums;
using DigitalWallet.Domain.Interfaces;
using NSubstitute;

namespace DigitalWallet.UnitTests.Services
{
    public class AuthServiceTests
    {
        private readonly IUserRepository _userRepo = Substitute.For<IUserRepository>();
        private readonly IWalletRepository _walletRepo = Substitute.For<IWalletRepository>();
        private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
        private readonly IJwtTokenGenerator _jwtGenerator = Substitute.For<IJwtTokenGenerator>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly AuthService _sut;

        public AuthServiceTests()
        {
            _sut = new AuthService(_userRepo, _walletRepo, _passwordHasher, _jwtGenerator, _unitOfWork);
        }

        [Fact]
        public async Task RegisterAsync_WithNewEmail_ShouldReturnToken()
        {
            // Arrange
            var request = new RegisterRequest
            {
                FullName = "Aykut",
                Email = "aykut@test.com",
                Password = "Test1234!",
                Currency = "TRY"
            };
            _userRepo.ExistsAsync(request.Email, Arg.Any<CancellationToken>()).Returns(false);
            _passwordHasher.Hash(request.Password).Returns("hashed-password");
            _jwtGenerator.GenerateToken(Arg.Any<User>()).Returns("jwt-token");

            // Act
            var result = await _sut.RegisterAsync(request, CancellationToken.None);

            // Assert
            Assert.Equal("jwt-token", result.Token);
            Assert.Equal("aykut@test.com", result.Email);
            Assert.Equal("Aykut", result.FullName);
            await _userRepo.Received(1).AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
            await _walletRepo.Received(1).AddAsync(Arg.Any<Wallet>(), Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task RegisterAsync_WithExistingEmail_ShouldThrow()
        {
            var request = new RegisterRequest
            {
                FullName = "Aykut",
                Email = "existing@test.com",
                Password = "Test1234!",
                Currency = "TRY"
            };
            _userRepo.ExistsAsync(request.Email, Arg.Any<CancellationToken>()).Returns(true);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.RegisterAsync(request, CancellationToken.None));
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ShouldReturnToken()
        {
            var user = new User("Aykut", "aykut@test.com", "hashed-pw", UserRole.User);
            var wallet = new Wallet(user.Id, "TRY");

            _userRepo.GetByEmailAsync("aykut@test.com", Arg.Any<CancellationToken>()).Returns(user);
            _walletRepo.GetByUserIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(wallet);
            _passwordHasher.Verify("Test1234!", "hashed-pw").Returns(true);
            _jwtGenerator.GenerateToken(user).Returns("jwt-token");

            var result = await _sut.LoginAsync(
                new LoginRequest { Email = "aykut@test.com", Password = "Test1234!" },
                CancellationToken.None);

            Assert.Equal("jwt-token", result.Token);
        }

        [Fact]
        public async Task LoginAsync_WithWrongPassword_ShouldThrow()
        {
            var user = new User("Aykut", "aykut@test.com", "hashed-pw", UserRole.User);
            _userRepo.GetByEmailAsync("aykut@test.com", Arg.Any<CancellationToken>()).Returns(user);
            _passwordHasher.Verify("wrong-password", "hashed-pw").Returns(false);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _sut.LoginAsync(
                    new LoginRequest { Email = "aykut@test.com", Password = "wrong-password" },
                    CancellationToken.None));
        }

        [Fact]
        public async Task LoginAsync_WithNonExistentEmail_ShouldThrow()
        {
            _userRepo.GetByEmailAsync("nobody@test.com", Arg.Any<CancellationToken>()).Returns((User?)null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _sut.LoginAsync(
                    new LoginRequest { Email = "nobody@test.com", Password = "Test1234!" },
                    CancellationToken.None));
        }
    }
}
