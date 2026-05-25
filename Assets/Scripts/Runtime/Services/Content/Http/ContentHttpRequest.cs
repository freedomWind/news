using System;
using System.Collections.Generic;

namespace NewsFramework.Services.Content.Http
{
    [Serializable]
    public sealed class ContentHttpRequest
    {
        public string method = ContentHttpMethods.Get;
        public string url;
        public string body;
        public int timeoutSeconds = 10;
        public List<ContentHttpHeader> headers = new List<ContentHttpHeader>();
    }
}
