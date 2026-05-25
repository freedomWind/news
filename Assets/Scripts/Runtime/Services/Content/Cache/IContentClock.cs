namespace NewsFramework.Services.Content.Cache
{
    public interface IContentClock
    {
        long UnixSeconds { get; }
    }
}
