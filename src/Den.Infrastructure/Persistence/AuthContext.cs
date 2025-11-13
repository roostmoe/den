using Den.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Den.Infrastructure.Persistence;

public class AuthContext(DbContextOptions<AuthContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<SecurityKey> SecurityKeys => Set<SecurityKey>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SecurityKey>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.KeyId).IsUnique();
            entity.HasIndex(e => new {
                e.IsActive,
                e.IsRevoked,
                e.ExpiresAt,
                e.Usage // allow fetching signature and encryption keys separately
            });

            entity.Property(e => e.KeyId).HasMaxLength(64).IsRequired();
            entity.Property(e => e.EncryptedPrivateKey).IsRequired();

            entity
                .Property(e => e.HashAlgorithm)
                .HasConversion(new EnumToStringConverter<SecurityKey.SecurityKeyHashAlgorithm>())
                .HasMaxLength(16);

            entity
                .Property(e => e.Usage)
                .HasConversion(new EnumToStringConverter<SecurityKey.SecurityKeyUsage>())
                .HasMaxLength(16);

            entity
                .Property(e => e.KeyType)
                .HasConversion(new EnumToStringConverter<SecurityKey.SecurityKeyType>())
                .HasMaxLength(16)
                .IsRequired();
        });
    }
}