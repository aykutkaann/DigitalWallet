using DigitalWallet.Application.DTOs.Wallet;
using DigitalWallet.Application.Interfaces;
using DigitalWallet.Application.Services;
using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.Interfaces;
using NSubstitute;

namespace DigitalWallet.UnitTests.Services
{
    public class WalletServiceTests
    {
        private readonly IWalletRepository _walletRepo = Substitute.For<IWalletRepository>();
        private readonly ITransactionRepository _transactionRepo = Substitute.For<ITransactionRepository>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
        private readonly WalletService _sut;

        public WalletServiceTests()
        {
            _sut = new WalletService(_walletRepo, _transactionRepo, _unitOfWork, _currentUser);
        }

        [Fact]
        public async Task GetMyWalletAsync_ShouldReturnWalletResponse()
        {
            var userId = Guid.NewGuid();
            var wallet = new Wallet(userId, "TRY");
            wallet.Credit(500);

            _currentUser.UserId.Returns(userId);
            _walletRepo.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(wallet);

            var result = await _sut.GetMyWalletAsync(CancellationToken.None);

            Assert.Equal(500, result.Balance);
            Assert.Equal("TRY", result.Currency);
        }

        [Fact]
        public async Task GetMyWalletAsync_WalletNotFound_ShouldThrow()
        {
            var userId = Guid.NewGuid();
            _currentUser.UserId.Returns(userId);
            _walletRepo.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns((Wallet?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.GetMyWalletAsync(CancellationToken.None));
        }

        [Fact]
        public async Task DepositAsync_ShouldIncreaseBalance()
        {
            var userId = Guid.NewGuid();
            var wallet = new Wallet(userId, "TRY");

            _currentUser.UserId.Returns(userId);
            _walletRepo.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(wallet);

            var result = await _sut.DepositAsync(new DepositRequest { Amount = 250 }, CancellationToken.None);

            Assert.Equal(250, result.Balance);
            await _transactionRepo.Received(1).AddAsync(Arg.Any<Transaction>(), Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task DepositAsync_WalletNotFound_ShouldThrow()
        {
            var userId = Guid.NewGuid();
            _currentUser.UserId.Returns(userId);
            _walletRepo.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns((Wallet?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.DepositAsync(new DepositRequest { Amount = 100 }, CancellationToken.None));
        }
    }
}
