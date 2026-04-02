using DigitalWallet.Domain.Entities;

namespace DigitalWallet.UnitTests.Entities
{
    public class WalletTests
    {
        [Fact]
        public void Constructor_ShouldCreateWallet_WithZeroBalance()
        {
            var wallet = new Wallet(Guid.NewGuid(), "TRY");

            Assert.Equal(0, wallet.Balance);
            Assert.Equal("TRY", wallet.Currency);
        }

        [Fact]
        public void Credit_ShouldIncreaseBalance()
        {
            var wallet = new Wallet(Guid.NewGuid(), "TRY");

            wallet.Credit(100);

            Assert.Equal(100, wallet.Balance);
        }

        [Fact]
        public void Credit_WithZeroAmount_ShouldThrow()
        {
            var wallet = new Wallet(Guid.NewGuid(), "TRY");

            Assert.Throws<ArgumentException>(() => wallet.Credit(0));
        }

        [Fact]
        public void Credit_WithNegativeAmount_ShouldThrow()
        {
            var wallet = new Wallet(Guid.NewGuid(), "TRY");

            Assert.Throws<ArgumentException>(() => wallet.Credit(-50));
        }

        [Fact]
        public void Debit_ShouldDecreaseBalance()
        {
            var wallet = new Wallet(Guid.NewGuid(), "TRY");
            wallet.Credit(200);

            wallet.Debit(50);

            Assert.Equal(150, wallet.Balance);
        }

        [Fact]
        public void Debit_WithInsufficientBalance_ShouldThrow()
        {
            var wallet = new Wallet(Guid.NewGuid(), "TRY");
            wallet.Credit(50);

            Assert.Throws<InvalidOperationException>(() => wallet.Debit(100));
        }

        [Fact]
        public void Debit_WithZeroAmount_ShouldThrow()
        {
            var wallet = new Wallet(Guid.NewGuid(), "TRY");

            Assert.Throws<ArgumentException>(() => wallet.Debit(0));
        }

        [Fact]
        public void Credit_ShouldUpdateUpdatedAt()
        {
            var wallet = new Wallet(Guid.NewGuid(), "TRY");
            Assert.Null(wallet.UpdatedAt);

            wallet.Credit(100);

            Assert.NotNull(wallet.UpdatedAt);
        }
    }
}
