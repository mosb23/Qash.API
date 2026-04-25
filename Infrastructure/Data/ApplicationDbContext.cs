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
    }
}