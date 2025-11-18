using Den.Application.Budgets;
using Den.Domain.Entities;
using Den.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Den.Infrastructure.Budgets;

public class BudgetService(DenDbContext context) : IBudgetService
{
    public async Task<BudgetResponse?> GetByIdAsync(Guid id)
    {
        var budget = await context.Budgets
            .FirstOrDefaultAsync(b => b.Id == id);

        return budget is null ? null : MapToResponse(budget);
    }

    public async Task<IEnumerable<BudgetResponse>> GetAllAsync()
    {
        var budgets = await context.Budgets
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        return budgets.Select(MapToResponse);
    }

    public async Task<BudgetResponse> CreateAsync(CreateBudgetRequest request, Guid userId)
    {
        var owner = await context.Users.FindAsync(userId);
        if (owner is null)
        {
            throw new InvalidOperationException("user not found");
        }

        var budget = new Budget
        {
            Id = Guid.NewGuid(),
            DisplayName = request.DisplayName,
            Description = request.Description,
            Period = Enum.Parse<BudgetPeriod>(request.Period, ignoreCase: true),
            Total = request.Total,
            Currency = Enum.Parse<BudgetCurrency>(request.Currency, ignoreCase: true),
            OwnerId = userId,
            Owner = owner,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Budgets.Add(budget);
        await context.SaveChangesAsync();

        return MapToResponse(budget);
    }

    public async Task<BudgetResponse?> UpdateAsync(Guid id, UpdateBudgetRequest request, Guid userId)
    {
        var budget = await context.Budgets
            .FirstOrDefaultAsync(b => b.Id == id && b.OwnerId == userId);

        if (budget is null)
        {
            return null;
        }

        if (request.DisplayName is not null)
        {
            budget.DisplayName = request.DisplayName;
        }

        if (request.Description is not null)
        {
            budget.Description = request.Description;
        }

        if (request.Period is not null)
        {
            budget.Period = Enum.Parse<BudgetPeriod>(request.Period, ignoreCase: true);
        }

        if (request.Total is not null)
        {
            budget.Total = request.Total.Value;
        }

        if (request.Currency is not null)
        {
            budget.Currency = Enum.Parse<BudgetCurrency>(request.Currency, ignoreCase: true);
        }

        budget.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        return MapToResponse(budget);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var budget = await context.Budgets
            .FirstOrDefaultAsync(b => b.Id == id && b.OwnerId == userId);

        if (budget is null)
        {
            return false;
        }

        context.Budgets.Remove(budget);
        await context.SaveChangesAsync();

        return true;
    }

    private static BudgetResponse MapToResponse(Budget budget)
    {
        return new BudgetResponse(
            Id: budget.Id.ToString(),
            DisplayName: budget.DisplayName,
            Description: budget.Description,
            Period: budget.Period.ToString(),
            Total: budget.Total,
            Currency: budget.Currency.ToString(),
            OwnerId: budget.OwnerId.ToString(),
            CreatedAt: budget.CreatedAt,
            UpdatedAt: budget.UpdatedAt
        );
    }
}
