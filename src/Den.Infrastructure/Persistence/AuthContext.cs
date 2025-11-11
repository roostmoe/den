using Den.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Den.Infrastructure.Persistence;

public class AuthContext(DbContextOptions<AuthContext> options) : DbContext(options)
{
  public DbSet<User> Users => Set<User>();
}