using System;

namespace NewsFramework.Services.Content.Http
{
    [Serializable]
    public sealed class ContentHttpResponse
    {
        public bool success;
        public long statusCode;
        public string body;
        public string error;
        public ContentHttpErrorKind errorKind;
        public bool retryable;
        public int attemptCount = 1;
    }
}
