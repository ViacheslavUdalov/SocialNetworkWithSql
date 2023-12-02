using ASP.SecondSocialWithSQL.Data;
using ASP.SecondSocialWithSQL.Interfaces;
using ASP.SecondSocialWithSQL.Services;
using Microsoft.EntityFrameworkCore;

namespace ASP.SecondSocialWithSQL.Extenstions;

public static class ApplicationServiceExtention
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddDbContext<DataContext>(options =>
        {
            options.UseSqlite(config.GetConnectionString("DefaultConnection"));
        });
        return services;
    }
}