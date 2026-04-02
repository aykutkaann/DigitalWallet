using DigitalWallet.Application.DTOs.Transfer;
using FluentValidation;

namespace DigitalWallet.Application.Validators
{
    public class CreateTransferRequestValidator : AbstractValidator<CreateTransferRequest>
    {
        public CreateTransferRequestValidator()
        {
            RuleFor(x => x.ReceiverEmail)
                .NotEmpty().EmailAddress();

            RuleFor(x => x.Amount)
                .GreaterThan(0);

            RuleFor(x => x.IdempotencyKey)
                .NotEmpty();
        }
    }
}
