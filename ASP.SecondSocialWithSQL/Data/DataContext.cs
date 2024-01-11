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
}