namespace Den.Application.Budgets;

public record BudgetResponse(
    string Id,
    string DisplayName,
    string Description,
    string Period,
    int Total,
    string Currency,
    string OwnerId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);