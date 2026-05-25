using System;

namespace NewsFramework.Data.Media
{
    [Serializable]
    public sealed class MediaAssetData
    {
        public string mediaId;
        public string type;
        public string url;
        public string posterUrl;
        public string streamUrl;
        public string mimeType;
        public string version;
        public string hash;
        public int width;
        public int height;
        public float aspectRatio;
        public float durationSeconds;
    }
}
