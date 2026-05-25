using System;

namespace NewsFramework.Services.Content
{
    public interface IContentService
    {
        event Action<ContentResult> PageUpdated;

        void LoadPage(ContentRequest request, Action<ContentResult> onComplete);
        void RefreshPage(ContentRequest request, Action<ContentResult> onComplete);
    }
}
