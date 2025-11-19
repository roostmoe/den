namespace Den.Application.Budgets;

public record CreateBudgetRequest(
    string DisplayName,
    string Description,
    string Period,
    int Total,
    string Currency
);
