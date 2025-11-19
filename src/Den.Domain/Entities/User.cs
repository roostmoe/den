namespace Den.Domain.Entities;

public class User
{
  public required Guid Id { get; set; }
  public required string Username { get; set; }
  public required string DisplayName { get; set; }
  public required string Email { get; set; }
  public required string PasswordHash { get; set; }
  public required UserRole Role { get; set; }

  public ICollection<Session> Sessions { get; } = [];
  public ICollection<Budget> Budgets { get; } = [];
}

public enum UserRole
{
    /// <summary>
    /// A user with permission to view the contents of the system, but not edit.
    /// </summary>
    VIEWER,

    /// <summary>
    /// A user with 'standard' permissions within the service, allowing them
    /// read/write permissions on all resources.
    /// </summary>
    EDITOR,

    /// <summary>
    /// A user with complete administrative access, granting extra abilities to
    /// manage other users with and see system diagnostic information.
    /// </summary>
    ADMIN,
}