using Answer.King.Api.RequestModels;
using FluentValidation;

namespace Answer.King.Api.Validators;

public class LineItemValidator : AbstractValidator<LineItemDto>
{
    public LineItemValidator()
    {
        this.RuleFor(li => li.Product)
            .NotNull()
            .SetValidator(new ProductIdValidator());

        this.RuleFor(li => li.Quantity)
            .GreaterThanOrEqualTo(0);
    }
}

public class ProductIdValidator : AbstractValidator<ProductId>
{
    public ProductIdValidator()
    {
        this.RuleFor(p => p.Id)
            .NotEmpty();
    }
}
