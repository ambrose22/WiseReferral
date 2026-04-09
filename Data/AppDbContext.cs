using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReferralTracker.Models;

namespace ReferralTracker.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Referral> Referrals => Set<Referral>();
    public DbSet<ReferralComment> ReferralComments => Set<ReferralComment>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Referral>(entity =>
        {
            entity.HasOne(r => r.ReferredByUser)
                .WithMany()
                .HasForeignKey(r => r.ReferredByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.AssignedToUser)
                .WithMany()
                .HasForeignKey(r => r.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ReferralComment>(entity =>
        {
            entity.HasOne(c => c.Referral)
                .WithMany(r => r.Comments)
                .HasForeignKey(c => c.ReferralId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(c => c.ReferralId);
        });
    }
}
