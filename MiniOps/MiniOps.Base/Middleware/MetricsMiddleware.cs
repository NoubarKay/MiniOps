using MiniOps.Base.Models;
using MiniOps.Base.Stores;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace MiniOps.Base.Middleware;

public class RequestMetricsMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, MetricStore metricStore)
    {
        var stopwatch = Stopwatch.StartNew();
        int statusCode = 200;

        try
        {
            await next(context);
            statusCode = context.Response.StatusCode;
        }
        catch
        {
            // Unhandled exceptions → mark as 500
            statusCode = 500;
            throw; // rethrow so the pipeline still sees the error
        }
        finally
        {
            stopwatch.Stop();

            if (!context.Request.Path.Value.Contains("/miniops"))
            {
                

            var metric = new RequestMetric(
                DateTime.UtcNow,
                stopwatch.ElapsedMilliseconds,
                statusCode,
                context.Request.Path);

            await metricStore.AddAsync(metric);
            }
        }
    }
}