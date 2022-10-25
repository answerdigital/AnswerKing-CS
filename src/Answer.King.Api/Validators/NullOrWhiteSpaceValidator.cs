using FluentValidation;

namespace Answer.King.Api.Validators;

public static class NullOrWhiteSpaceValidator
{
    public static IRuleBuilderOptions<T, string> NotNullOrWhiteSpace<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.Must(x => !string.IsNullOrWhiteSpace(x))
            .WithMessage("{PropertyName} cannot be null, empty or whitespace.");
    }
}
