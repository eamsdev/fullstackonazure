using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        return services
            .AddApiDoc()
            .AddProblemDetails() // TODO: Configure
            .AddCors(ConfigureCors)
            .AddControllerInternal()
            .AddHttpContextAccessor()
            .AddApplicationInsightsTelemetry();
    }
    
    private static IServiceCollection AddControllerInternal(this IServiceCollection services)
    {

        services.AddControllers()
            .AddJsonOptions(x => x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);
        
        return services;
    }

    private static void ConfigureCors(CorsOptions opt)
    {
        opt.AddPolicy("CorsPolicy", policy =>
        {
            policy
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithExposedHeaders(
                    "WWW-Authenticate") // expose header so client can understand when to log user out
                .WithOrigins("http://localhost:8080"); // required when access resource from a different domain
        });
    }

    private static IServiceCollection AddApiDoc(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .ConfigureSwaggerGen(options =>
            {
                options.CustomSchemaIds(s => s.FullName?.Replace("+", "."));
            });
        return services;
    }
}