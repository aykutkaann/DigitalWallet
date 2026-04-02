using DigitalWallet.Application.DTOs.Wallet;
using DigitalWallet.Application.Validators;

namespace DigitalWallet.UnitTests.Validators
{
    public class DepositRequestValidatorTests
    {
        private readonly DepositRequestValidator _validator = new();

        [Fact]
        public void ValidAmount_ShouldPass()
        {
            var request = new DepositRequest { Amount = 100 };

            var result = _validator.Validate(request);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void ZeroAmount_ShouldFail()
        {
            var request = new DepositRequest { Amount = 0 };

            var result = _validator.Validate(request);

            Assert.False(result.IsValid);
        }

        [Fact]
        public void NegativeAmount_ShouldFail()
        {
            var request = new DepositRequest { Amount = -50 };

            var result = _validator.Validate(request);

            Assert.False(result.IsValid);
        }
    }
}
