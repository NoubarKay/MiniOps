namespace MiniOps.Base.Models;

public record RequestMetric(
    DateTime Timestamp,
    long DurationMs,
    int StatusCode,
    string Path);