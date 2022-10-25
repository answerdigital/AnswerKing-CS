using Answer.King.Api.RequestModels;
using FluentValidation;

namespace Answer.King.Api.Validators;

public class MakePaymentValidator : AbstractValidator<MakePayment>
{
    public MakePaymentValidator()
    {
        this.RuleFor(c => c.Amount)
            .GreaterThanOrEqualTo(0.00);

        this.RuleFor(c => c.OrderId)
            .NotEmpty();
    }
}
