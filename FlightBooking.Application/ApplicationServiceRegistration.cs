using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FlightBooking.Application.Behaviors;
using FluentValidation;
using MediatR;

namespace FlightBooking.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        services.AddValidatorsFromAssembly(assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}