using Microsoft.EntityFrameworkCore;
using Qash.API.Domain.Common;
using Qash.API.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Qash.API.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInfo();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        ApplyAuditInfo();
        return base.SaveChanges();
    }

    private void ApplyAuditInfo()
    {
        var entries = ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = DateTime.UtcNow;
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(x => x.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.HasIndex(x => x.Email).IsUnique();

            entity.HasIndex(x => x.PhoneNumber).IsUnique();

            entity.Property(x => x.PasswordHash)
                .IsRequired();

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Token)
                .IsRequired();

            entity.Property(x => x.ExpiresAt)
                .IsRequired();

            entity.HasOne(x => x.ApplicationUser)
                .WithMany(x => x.RefreshTokens)
                .HasForeignKey(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.ToTable("Wallets");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Currency)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(x => x.Balance)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.HasOne(x => x.ApplicationUser)
                .WithMany(x => x.Wallets)
                .HasForeignKey(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => new { x.ApplicationUserId, x.Name });

            entity.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("Transactions");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.TransactionType)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(x => x.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(x => x.Category)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(x => x.Description)
                .HasMaxLength(500);

            entity.Property(x => x.TransactionDate)
                .IsRequired();

            entity.HasOne(x => x.Wallet)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.WalletId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.ApplicationUser)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => new { x.ApplicationUserId, x.TransactionDate });
            entity.HasIndex(x => new { x.WalletId, x.TransactionDate });

            entity.HasQueryFilter(x => !x.IsDeleted);
        });
    }
}