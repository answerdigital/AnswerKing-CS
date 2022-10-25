using Answer.King.Api.RequestModels;
using FluentValidation;

namespace Answer.King.Api.Validators;

public class CategoryValidator : AbstractValidator<CategoryDto>
{
    public CategoryValidator()
    {
        this.RuleFor(c => c.Name)
            .NotNullOrWhiteSpace();

        this.RuleFor(c => c.Description)
            .NotNullOrWhiteSpace();
    }
}
