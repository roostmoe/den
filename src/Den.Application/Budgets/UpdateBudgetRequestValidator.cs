using Den.Domain.Entities;
using FluentValidation;

namespace Den.Application.Budgets;

public class UpdateBudgetRequestValidator : AbstractValidator<UpdateBudgetRequest>
{
    public UpdateBudgetRequestValidator()
    {
        RuleFor(x => x.DisplayName)
            .MaximumLength(100).WithMessage("display name must not exceed 100 characters")
            .When(x => x.DisplayName is not null);

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("description must not exceed 500 characters")
            .When(x => x.Description is not null);

        RuleFor(x => x.Period)
            .Must(BeValidPeriod).WithMessage("period must be one of: Weekly, Monthly, Quarterly, Yearly")
            .When(x => x.Period is not null);

        RuleFor(x => x.Total)
            .GreaterThan(0).WithMessage("total must be greater than 0")
            .When(x => x.Total is not null);

        RuleFor(x => x.Currency)
            .Must(BeValidCurrency).WithMessage("currency must be one of: GBP, EUR, USD, COP")
            .When(x => x.Currency is not null);
    }

    private static bool BeValidPeriod(string? period)
    {
        return period is null || Enum.TryParse<BudgetPeriod>(period, ignoreCase: true, out _);
    }

    private static bool BeValidCurrency(string? currency)
    {
        return currency is null || Enum.TryParse<BudgetCurrency>(currency, ignoreCase: true, out _);
    }
}
