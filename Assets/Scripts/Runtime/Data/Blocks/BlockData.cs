using System;
using NewsFramework.Data.Media;
using NewsFramework.Data.Replay;

namespace NewsFramework.Data.Blocks
{
    [Serializable]
    public sealed class BlockData
    {
        public string id;
        public string type;
        public string rendererKey;
        public string prefabKey;
        public string fallbackType;
        public float marginTop = -1f;
        public float marginBottom = -1f;
        public BlockActionData action = BlockActionData.None();

        public string badge;
        public string title;
        public string subtitle;
        public string source;
        public string boardTitle;
        public string fen;
        public string text;
        public string time;
        public string url;
        public string posterUrl;
        public string streamUrl;
        public string caption;
        public float aspectRatio = 1.6f;
        public float height = 12f;
        public float durationSeconds;
        public MediaAssetData media;
        public ReplayData replay;
    }
}
