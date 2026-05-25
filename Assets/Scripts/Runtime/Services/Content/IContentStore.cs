using NewsFramework.Data.Blocks;
using NewsFramework.Services.Content.Cache;

namespace NewsFramework.Services.Content
{
    public interface IContentStore
    {
        bool TryGetEntry(string pageId, out ContentCacheEntry entry);
        void SetEntry(ContentCacheEntry entry);
        bool TryGetPage(string pageId, out PageData page);
        void SetPage(PageData page);
        void RemovePage(string pageId);
        void Clear();
    }
}
