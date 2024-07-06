namespace Azf.Shared.Time;

public interface IClock
{
    DateTime UtcNow { get; }

    DateTime UtcToday { get; }
}

public class Clock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;

    public DateTime UtcToday => DateTime.UtcNow.Date;
}