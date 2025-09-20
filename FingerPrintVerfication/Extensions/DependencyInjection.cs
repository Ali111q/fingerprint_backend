using FingerPrintVerfication.Data;
using FingerPrintVerfication.Interfaces;
using FingerPrintVerfication.Repositories;
using Takeel.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace FingerPrintVerfication.Extensions;

static public class DependencyInjection
{
    public static void AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<DataContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IPersonRepository, PersonRepository>();

        // Services
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<PersonService>();
        services.AddScoped<WebSocketService>();
    }
}