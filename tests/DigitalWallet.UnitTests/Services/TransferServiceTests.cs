using DigitalWallet.Application.DTOs.Transfer;
using DigitalWallet.Application.Interfaces;
using DigitalWallet.Application.Services;
using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.Enums;
using DigitalWallet.Domain.Interfaces;
using DigitalWallet.Application.Events;
using MassTransit;
using NSubstitute;

namespace DigitalWallet.UnitTests.Services
{
    public class TransferServiceTests
    {
        private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
        private readonly IWalletRepository _walletRepo = Substitute.For<IWalletRepository>();
        private readonly IUserRepository _userRepo = Substitute.For<IUserRepository>();
        private readonly ITransferRequestRepository _transferRepo = Substitute.For<ITransferRequestRepository>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
        private readonly IPublishEndpoint _publishEndpoint = Substitute.For<IPublishEndpoint>();
        private readonly TransferService _sut;

        public TransferServiceTests()
        {
            _sut = new TransferService(_currentUser, _walletRepo, _userRepo, _transferRepo, _unitOfWork, _publishEndpoint);
        }

        private (User sender, Wallet senderWallet, User receiver, Wallet receiverWallet) SetupUsers()
        {
            var sender = new User("Sender", "sender@test.com", "hash", UserRole.User);
            var senderWallet = new Wallet(sender.Id, "TRY");
            senderWallet.Credit(500);

            var receiver = new User("Receiver", "receiver@test.com", "hash", UserRole.User);
            var receiverWallet = new Wallet(receiver.Id, "TRY");

            _currentUser.UserId.Returns(sender.Id);
            _walletRepo.GetByUserIdAsync(sender.Id, Arg.Any<CancellationToken>()).Returns(senderWallet);
            _userRepo.GetByEmailAsync("receiver@test.com", Arg.Any<CancellationToken>()).Returns(receiver);
            _walletRepo.GetByUserIdAsync(receiver.Id, Arg.Any<CancellationToken>()).Returns(receiverWallet);
            _transferRepo.GetByIdempotencyKeyAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((TransferRequest?)null);

            return (sender, senderWallet, receiver, receiverWallet);
        }

        [Fact]
        public async Task CreateTransferAsync_ValidRequest_ShouldReturnPending()
        {
            SetupUsers();
            var request = new CreateTransferRequest("receiver@test.com", 200, "key-1");

            var result = await _sut.CreateTransferAsync(request, CancellationToken.None);

            Assert.Equal("Pending", result.Status);
            Assert.Equal(200, result.Amount);
            await _transferRepo.Received(1).AddAsync(Arg.Any<TransferRequest>(), Arg.Any<CancellationToken>());
            await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
            await _publishEndpoint.Received(1).Publish(Arg.Any<TransferRequestedEvent>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task CreateTransferAsync_InsufficientBalance_ShouldThrow()
        {
            SetupUsers();
            var request = new CreateTransferRequest("receiver@test.com", 1000, "key-2");

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.CreateTransferAsync(request, CancellationToken.None));
        }

        [Fact]
        public async Task CreateTransferAsync_ReceiverNotFound_ShouldThrow()
        {
            var sender = new User("Sender", "sender@test.com", "hash", UserRole.User);
            var senderWallet = new Wallet(sender.Id, "TRY");
            senderWallet.Credit(500);

            _currentUser.UserId.Returns(sender.Id);
            _walletRepo.GetByUserIdAsync(sender.Id, Arg.Any<CancellationToken>()).Returns(senderWallet);
            _userRepo.GetByEmailAsync("nobody@test.com", Arg.Any<CancellationToken>()).Returns((User?)null);

            var request = new CreateTransferRequest("nobody@test.com", 100, "key-3");

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.CreateTransferAsync(request, CancellationToken.None));
        }

        [Fact]
        public async Task CreateTransferAsync_SendToSelf_ShouldThrow()
        {
            var sender = new User("Sender", "sender@test.com", "hash", UserRole.User);
            var senderWallet = new Wallet(sender.Id, "TRY");
            senderWallet.Credit(500);

            _currentUser.UserId.Returns(sender.Id);
            _walletRepo.GetByUserIdAsync(sender.Id, Arg.Any<CancellationToken>()).Returns(senderWallet);
            _userRepo.GetByEmailAsync("sender@test.com", Arg.Any<CancellationToken>()).Returns(sender);

            var request = new CreateTransferRequest("sender@test.com", 100, "key-4");

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.CreateTransferAsync(request, CancellationToken.None));
        }

        [Fact]
        public async Task CreateTransferAsync_DuplicateIdempotencyKey_ShouldReturnExisting()
        {
            SetupUsers();
            var existingTransfer = new TransferRequest(Guid.NewGuid(), Guid.NewGuid(), 200, "duplicate-key");

            _transferRepo.GetByIdempotencyKeyAsync("duplicate-key", Arg.Any<CancellationToken>()).Returns(existingTransfer);

            var request = new CreateTransferRequest("receiver@test.com", 200, "duplicate-key");
            var result = await _sut.CreateTransferAsync(request, CancellationToken.None);

            Assert.Equal("Pending", result.Status);
            Assert.Equal(existingTransfer.Id, result.TransferRequestId);
            // Should NOT save or publish — just return existing
            await _transferRepo.DidNotReceive().AddAsync(Arg.Any<TransferRequest>(), Arg.Any<CancellationToken>());
            await _publishEndpoint.DidNotReceive().Publish(Arg.Any<TransferRequestedEvent>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetTransferByIdAsync_NotFound_ShouldThrow()
        {
            _transferRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((TransferRequest?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.GetTransferByIdAsync(Guid.NewGuid(), CancellationToken.None));
        }
    }
}
