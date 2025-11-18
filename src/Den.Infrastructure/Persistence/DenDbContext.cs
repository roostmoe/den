using Den.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Den.Infrastructure.Persistence;

public class DenDbContext(DbContextOptions<DenDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Budget> Budgets => Set<Budget>();
    public DbSet<BudgetPoint> BudgetPoints => Set<BudgetPoint>();
    public DbSet<BudgetSource> BudgetSources => Set<BudgetSource>();
}