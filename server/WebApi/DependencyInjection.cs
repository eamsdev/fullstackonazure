using System.Security.Claims;
using System.Text.Json.Serialization;
using Application.Common;
using Application.Common.Exceptions;
using Domain.Common;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;

namespace WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        return services
            .AddApiDoc()
            .AddAuth()
            .AddUserContext()
            .AddProblemDetails()
            .AddCors(Configure)
            .AddControllerInternal()
            .AddHttpContextAccessor()
            .AddApplicationInsightsTelemetry();
    }

    public static IApplicationBuilder ConfigureExceptionHandler(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                context.Response.ContentType = "application/problem+json";
                if (context.RequestServices.GetService<IProblemDetailsService>() is { } problemDetailsService)
                {
                    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var ex = exceptionHandlerFeature?.Error;

                    if (ex is null)
                        return;
                    
                    (string Title, string Detail, int StatusCode) details = ex switch
                    {
                        InvalidDomainOperationException or InvalidOperationException=>
                        (
                            ex.GetType().Name,
                            ex.Message,
                            context.Response.StatusCode = StatusCodes.Status400BadRequest
                        ),
                        NotFoundException =>
                        (
                            ex.GetType().Name,
                            ex.Message,
                            context.Response.StatusCode = StatusCodes.Status404NotFound
                        ),
                        UnauthorizedException =>
                        (
                            ex.GetType().Name,
                            ex.Message,
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized
                        ),
                        _ =>
                        (
                            ex.GetType().Name,
                            ex.Message,
                            context.Response.StatusCode = StatusCodes.Status500InternalServerError
                        )
                    };
                    
                    var problem = new ProblemDetailsContext
                    {
                        HttpContext = context,
                        ProblemDetails =
                        {
                            Title = details.Title,
                            Detail = details.Detail,
                            Status = details.StatusCode
                        }
                    };
                    
                    if (env.IsDevelopment())
                        problem.ProblemDetails.Extensions.Add(
                            "exception", exceptionHandlerFeature?.Error.ToString());
                    
                    
                    await problemDetailsService.WriteAsync(problem);
                }
            });
        });
        
        return app;
    }

    private static IServiceCollection AddAuth(this IServiceCollection services)
    {
        services
            .AddAuthorization()
            .AddAuthentication(IdentityConstants.ApplicationScheme)
            // AddIdentityCore(...) doesnt add these by default
            .AddCookie(IdentityConstants.ApplicationScheme, ConfigureCookie)
            .AddCookie(IdentityConstants.ExternalScheme, ConfigureCookie)
            .AddCookie(IdentityConstants.TwoFactorUserIdScheme);
        
        services.AddIdentityCore<User>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager<SignInManager<User>>()
            .AddDefaultTokenProviders();

        return services;
    }

    private static IServiceCollection AddControllerInternal(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(x => x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);
        
        return services;
    }

    private static void Configure(CorsOptions opt)
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
            .AddSwaggerDocument();
            // .ConfigureSwaggerGen(options =>
            // {
            //     options.CustomSchemaIds(s => s.FullName?.Replace("+", "."));
            // });
        return services;
    }
    
    private static IServiceCollection AddUserContext(this IServiceCollection services)
    {
        services.AddScoped(sp =>
        {
            var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
            var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return new CurrentUserContext(userId);
        });
        
        return services;
    }
    
    private static void ConfigureCookie(CookieAuthenticationOptions options)
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Events.OnRedirectToLogin = c =>
        {
            c.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.FromResult<object>(null!);
        };
    }
}