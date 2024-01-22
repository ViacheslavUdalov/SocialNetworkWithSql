using ASP.SecondSocialWithSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ASP.SecondSocialWithSQL.Data;

public class DataContext : DbContext
{
    // base(options) обязательно для установки миграции(Migrations)
    public  DataContext(DbContextOptions options) : base(options)
    {
     
    }
    public DbSet<AppUser> Users { get; set; }
    public DbSet<UserLike> Likes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<UserLike>()
            .HasKey(k => new { k.SourceUserId, k.LikedUserId });

        builder.Entity<UserLike>()
            .HasOne(s => s.SourceUser)
            .WithMany(l => l.LikedUsers)
            .HasForeignKey(s => s.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Entity<UserLike>()
            .HasOne(s => s.LikedUser)
            .WithMany(l => l.LikedByUsers)
            .HasForeignKey(s => s.LikedUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}