using ASP.SecondSocialWithSQL.Data;
using ASP.SecondSocialWithSQL.Helpers;
using ASP.SecondSocialWithSQL.Interfaces;
using ASP.SecondSocialWithSQL.Services;
using ASP.SecondSocialWithSQL.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ASP.SecondSocialWithSQL.Extenstions;

public static class ApplicationServiceExtention
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<PresenceTracker>();
        services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPhotoService, PhotoService>();
        services.AddScoped<ILikesRepository, LikesRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<LogUserActivity>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
        services.AddDbContext<DataContext>(options =>
        {
            options.UseSqlite(config.GetConnectionString("DefaultConnection"));
        });
        return services;
    }
}