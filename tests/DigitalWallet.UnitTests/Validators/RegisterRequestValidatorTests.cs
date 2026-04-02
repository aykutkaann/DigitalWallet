using DigitalWallet.Application.DTOs.Auth;
using DigitalWallet.Application.Validators;

namespace DigitalWallet.UnitTests.Validators
{
    public class RegisterRequestValidatorTests
    {
        private readonly RegisterRequestValidator _validator = new();

        [Fact]
        public void ValidRequest_ShouldPass()
        {
            var request = new RegisterRequest
            {
                FullName = "Aykut",
                Email = "aykut@test.com",
                Password = "Test1234!",
                Currency = "TRY"
            };

            var result = _validator.Validate(request);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void EmptyEmail_ShouldFail()
        {
            var request = new RegisterRequest
            {
                FullName = "Aykut",
                Email = "",
                Password = "Test1234!",
                Currency = "TRY"
            };

            var result = _validator.Validate(request);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Email");
        }

        [Fact]
        public void InvalidEmail_ShouldFail()
        {
            var request = new RegisterRequest
            {
                FullName = "Aykut",
                Email = "not-an-email",
                Password = "Test1234!",
                Currency = "TRY"
            };

            var result = _validator.Validate(request);

            Assert.False(result.IsValid);
        }

        [Fact]
        public void ShortPassword_ShouldFail()
        {
            var request = new RegisterRequest
            {
                FullName = "Aykut",
                Email = "aykut@test.com",
                Password = "12345",
                Currency = "TRY"
            };

            var result = _validator.Validate(request);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Password");
        }

        [Fact]
        public void ShortFullName_ShouldFail()
        {
            var request = new RegisterRequest
            {
                FullName = "A",
                Email = "aykut@test.com",
                Password = "Test1234!",
                Currency = "TRY"
            };

            var result = _validator.Validate(request);

            Assert.False(result.IsValid);
        }

        [Fact]
        public void CurrencyTooLong_ShouldFail()
        {
            var request = new RegisterRequest
            {
                FullName = "Aykut",
                Email = "aykut@test.com",
                Password = "Test1234!",
                Currency = "TRYY"
            };

            var result = _validator.Validate(request);

            Assert.False(result.IsValid);
        }
    }
}
