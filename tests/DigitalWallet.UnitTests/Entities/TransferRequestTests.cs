using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.Enums;

namespace DigitalWallet.UnitTests.Entities
{
    public class TransferRequestTests
    {
        [Fact]
        public void Constructor_ShouldSetStatusToPending()
        {
            var transfer = new TransferRequest(Guid.NewGuid(), Guid.NewGuid(), 100, "key-123");

            Assert.Equal(TransactionStatus.Pending, transfer.Status);
            Assert.Null(transfer.CompletedAt);
        }

        [Fact]
        public void MarkCompleted_ShouldUpdateStatusAndTimestamp()
        {
            var transfer = new TransferRequest(Guid.NewGuid(), Guid.NewGuid(), 100, "key-123");

            transfer.MarkCompleted();

            Assert.Equal(TransactionStatus.Completed, transfer.Status);
            Assert.NotNull(transfer.CompletedAt);
        }

        [Fact]
        public void MarkFailed_ShouldUpdateStatusAndTimestamp()
        {
            var transfer = new TransferRequest(Guid.NewGuid(), Guid.NewGuid(), 100, "key-123");

            transfer.MarkFailed();

            Assert.Equal(TransactionStatus.Failed, transfer.Status);
            Assert.NotNull(transfer.CompletedAt);
        }

        [Fact]
        public void Constructor_ShouldSetIdempotencyKey()
        {
            var transfer = new TransferRequest(Guid.NewGuid(), Guid.NewGuid(), 200, "unique-key");

            Assert.Equal("unique-key", transfer.IdempotencyKey);
            Assert.Equal(200, transfer.Amount);
        }
    }
}
