using DigitalWallet.Application.DTOs.Transfer;
using DigitalWallet.Application.Validators;

namespace DigitalWallet.UnitTests.Validators
{
    public class CreateTransferRequestValidatorTests
    {
        private readonly CreateTransferRequestValidator _validator = new();

        [Fact]
        public void ValidRequest_ShouldPass()
        {
            var request = new CreateTransferRequest("receiver@test.com", 100, "key-123");

            var result = _validator.Validate(request);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void EmptyReceiverEmail_ShouldFail()
        {
            var request = new CreateTransferRequest("", 100, "key-123");

            var result = _validator.Validate(request);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "ReceiverEmail");
        }

        [Fact]
        public void InvalidReceiverEmail_ShouldFail()
        {
            var request = new CreateTransferRequest("not-email", 100, "key-123");

            var result = _validator.Validate(request);

            Assert.False(result.IsValid);
        }

        [Fact]
        public void ZeroAmount_ShouldFail()
        {
            var request = new CreateTransferRequest("a@test.com", 0, "key-123");

            var result = _validator.Validate(request);

            Assert.False(result.IsValid);
        }

        [Fact]
        public void EmptyIdempotencyKey_ShouldFail()
        {
            var request = new CreateTransferRequest("a@test.com", 100, "");

            var result = _validator.Validate(request);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "IdempotencyKey");
        }
    }
}
