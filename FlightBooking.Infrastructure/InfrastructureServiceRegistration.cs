using FlightBooking.Application.Interfaces;
using FlightBooking.Infrastructure.Services;
using FlightBooking.Infrastructure.AirSearch;
using FlightBooking.Infrastructure.Persistence;
using FlightBooking.Infrastructure.Services.Providers;
using Microsoft.EntityFrameworkCore; // <-- BUNU EKLEMEYİ UNUTMA
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlightBooking.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<FlightDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(FlightDbContext).Assembly.FullName)));


        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis") ?? "localhost:6379";
        });

        services.AddScoped<ICacheService, RedisCacheService>();

        services.AddScoped<IAirSearch>(provider => 
        {
            return new AirSearchClient(AirSearchClient.EndpointConfiguration.BasicHttpBinding_IAirSearch);
        });

        services.AddScoped<IFlightProvider, AirSearchFlightProvider>();
        
        services.AddScoped<IFlightDbContext>(provider => provider.GetRequiredService<FlightDbContext>());

        return services;
    }
}