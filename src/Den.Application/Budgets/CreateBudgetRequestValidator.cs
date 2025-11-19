using Den.Domain.Entities;
using FluentValidation;

namespace Den.Application.Budgets;

public class CreateBudgetRequestValidator : AbstractValidator<CreateBudgetRequest>
{
    public CreateBudgetRequestValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("display name is required")
            .MaximumLength(100).WithMessage("display name must not exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("description must not exceed 500 characters");

        RuleFor(x => x.Period)
            .NotEmpty().WithMessage("period is required")
            .Must(BeValidPeriod).WithMessage("period must be one of: Weekly, Monthly, Quarterly, Yearly");

        RuleFor(x => x.Total)
            .GreaterThan(0).WithMessage("total must be greater than 0");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("currency is required")
            .Must(BeValidCurrency).WithMessage("currency must be one of: GBP, EUR, USD, COP");
    }

    private static bool BeValidPeriod(string period)
    {
        return Enum.TryParse<BudgetPeriod>(period, ignoreCase: true, out _);
    }

    private static bool BeValidCurrency(string currency)
    {
        return Enum.TryParse<BudgetCurrency>(currency, ignoreCase: true, out _);
    }
}
