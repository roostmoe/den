namespace Den.Domain.Entities;

public class Budget
{
    public required Guid Id { get; set; }
    public required string DisplayName { get; set; }
    public required string Description { get; set; } = "";
    public required BudgetPeriod Period { get; set; }

    // Recorded where the first digit is the lowest denominator of that currency
    //
    // Examples:
    // - 427.13GBP -> 42713 (42,713 pence)
    // - 30.10EUR -> 3010 (3,010 cents)
    // - 0.41USD -> 3010 (41 cents)
    // - 6000COP -> 6000 (6000 pesos)
    public required int Total { get; set; }
    public required BudgetCurrency Currency { get; set; }

    public required Guid OwnerId { get; set; }
    public required User Owner { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<BudgetPoint> BudgetPoints { get; } = [];
    public ICollection<BudgetSource> BudgetSources { get; } = [];
}

public enum BudgetCurrency
{
    GBP,
    EUR,
    USD,
    COP
}

public enum BudgetPeriod
{
    Weekly,
    Monthly,
    Quarterly,
    Yearly
}

public class BudgetSource
{
    public required Guid Id { get; set; }
    public required string DisplayName { get; set; }
    public required BudgetProvider Provider { get; set; }

    public ICollection<Budget> Budgets { get; } = [];
    public ICollection<BudgetPoint> BudgetPoints { get; } = [];
}

public enum BudgetProvider
{
    Bank_Starling
}

public class BudgetPoint
{
    public required Guid Id { get; set; }
    public required string Reference { get; set; }
    public required int Value { get; set; }
    public DateTime Timestamp { get; set; }

    public required Guid BudgetId { get; set; }
    public required Budget Budget { get; set; } = null!;

    public required Guid BudgetSourceId { get; set; }
    public required BudgetSource BudgetSource { get; set; } = null!;
}