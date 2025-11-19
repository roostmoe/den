namespace Den.Application.Budgets;

public record UpdateBudgetRequest(
    string? DisplayName,
    string? Description,
    string? Period,
    int? Total,
    string? Currency
);
