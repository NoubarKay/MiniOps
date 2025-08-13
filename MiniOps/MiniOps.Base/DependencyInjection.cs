using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MiniOps.Base.Extensions.Controller;
using MiniOps.Base.Hubs;
using MiniOps.Base.Middleware;
using MiniOps.Base.Stores;

namespace MiniOps.Base.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddMiniOps(this IServiceCollection services)
    {
        services.AddSignalR();
        services.AddControllers().AddApplicationPart(typeof(MetricsController).Assembly);
        services.AddSingleton<MetricStore>();
        return services;
    }

    public static IApplicationBuilder UseMiniOps(this IApplicationBuilder app)
    {
        app.UseMiddleware<RequestMetricsMiddleware>();

        return app;
    }
}