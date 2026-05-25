using System;

namespace NewsFramework.Services.Content.Http
{
    public interface IContentHttpClient
    {
        void Send(ContentHttpRequest request, Action<ContentHttpResponse> onComplete);
    }
}
