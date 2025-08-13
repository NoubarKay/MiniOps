using System.Collections.Concurrent;
using MiniOps.Base.Models;
using Microsoft.AspNetCore.SignalR;
using MiniOps.Base.Hubs;

namespace MiniOps.Base.Stores;

public class MetricStore
{
    private static readonly ConcurrentQueue<RequestMetric> _queue = new();
    private readonly TimeSpan _retention = TimeSpan.FromHours(1);
    private readonly IHubContext<MetricsHub> _hubContext;

    public MetricStore(IHubContext<MetricsHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task AddAsync(RequestMetric metric)
    {
        _queue.Enqueue(metric);
        CleanupOld();

        await _hubContext.Clients.All.SendAsync("ReceiveMetric", metric);
    }

    private void CleanupOld()
    {
        var cutoff = DateTime.UtcNow - _retention;

        while (_queue.TryPeek(out var oldest) && oldest.Timestamp < cutoff)
        {
            _queue.TryDequeue(out _);
        }
    }

    public IReadOnlyCollection<RequestMetric> GetSnapshot() => _queue.ToArray();
}