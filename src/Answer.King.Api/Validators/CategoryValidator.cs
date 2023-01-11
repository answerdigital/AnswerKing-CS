﻿using Answer.King.Api.RequestModels;
using FluentValidation;

namespace Answer.King.Api.Validators;

public class CategoryValidator : AbstractValidator<Category>
{
    public CategoryValidator()
    {
        this.RuleFor(c => c.Name)
            .NotNullOrWhiteSpace();

        this.RuleFor(c => c.Description)
            .NotNullOrWhiteSpace();

        this.RuleFor(p => p.Products)
            .ForEach(p =>
                p.NotNull()
                    .GreaterThanOrEqualTo(0));
    }
}
