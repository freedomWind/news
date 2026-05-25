namespace NewsFramework.Media.Api.Utilities;

public static class MediaClock
{
    public static long UnixSeconds()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}
