using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MiniOps.Base.Stores;

namespace MiniOps.Base.Extensions.Controller;

[ApiController]
[Route("miniops/api/[controller]")]
public class MetricsController(MetricStore metrics) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var snapshot = metrics.GetSnapshot(); // returns counts, avg duration, error count
        var countLastMinute = snapshot
            .Where(m => m.Timestamp >= DateTime.UtcNow.AddMilliseconds(-700));
        
        var countLastMinuteSuccess = snapshot
            .Count(m => m.Timestamp >= DateTime.UtcNow.AddMilliseconds(-700) && m.StatusCode == 200);
        
        
        var countLastMinuteFail = snapshot
            .Count(m => m.Timestamp >= DateTime.UtcNow.AddMilliseconds(-700) && m.StatusCode != 200);

        var requestMetrics = countLastMinute.ToList();
        return Ok(
            new
            {
                countLastMinute = requestMetrics.Any() ? requestMetrics?.Count() : 0, 
                countLastMinuteSuccess, 
                countLastMinuteFail, 
                averageTime = requestMetrics.Any() ? requestMetrics?.Average(x=>x.DurationMs) : 0,
                TotalRequests = countLastMinuteSuccess + countLastMinuteFail,
                TotalSuccessRequests = countLastMinuteSuccess,
                TotalFailedRequests = countLastMinuteFail
            });
    }
}