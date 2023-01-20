namespace Answer.King.Api.Common.HealthChecks;

public interface IStopwatch
{
    public long ElapsedMilliseconds { get; }

    public void Start();

#pragma warning disable CA1716
    public void Stop();
#pragma warning restore CA1716
}
