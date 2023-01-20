namespace Answer.King.Api.Common.HealthChecks;

public interface IStopwatch
{
    public long GetTimestamp();

    public TimeSpan GetElapsedTime(long startingTimestamp);
}
