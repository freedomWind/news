using System;

namespace NewsFramework.Services.Content.Cache
{
    public sealed class SystemContentClock : IContentClock
    {
        public long UnixSeconds => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}
