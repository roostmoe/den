namespace Den.Application.Budgets;

public interface IBudgetService
{
    Task<BudgetResponse?> GetByIdAsync(Guid id);
    Task<IEnumerable<BudgetResponse>> GetAllAsync();
    Task<BudgetResponse> CreateAsync(CreateBudgetRequest request, Guid userId);
    Task<BudgetResponse?> UpdateAsync(Guid id, UpdateBudgetRequest request, Guid userId);
    Task<bool> DeleteAsync(Guid id, Guid userId);
}
