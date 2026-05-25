using System;
using System.Collections.Generic;
using NewsFramework.Data.Media;
using NewsFramework.Data.Replay;

namespace NewsFramework.Services.Content.Dto
{
    [Serializable]
    public sealed class ContentServerEnvelopeDto
    {
        public bool? success;
        public int code;
        public string message;
        public long serverTime;
        public ContentServerPageDto data;

        public bool IsSuccess => success.HasValue ? success.Value : code == 0;
    }

    [Serializable]
    public sealed class ContentServerPageDto
    {
        public string feedId;
        public string pageId;
        public string pageType;
        public string title;
        public string feedVersion;
        public string version;
        public string cursor;
        public string nextCursor;
        public bool hasMore;
        public int estimatedTotal;
        public long expiresInSeconds;
        public List<ContentServerBlockDto> blocks = new List<ContentServerBlockDto>();
    }

    [Serializable]
    public sealed class ContentServerBlockDto
    {
        public string id;
        public string type;
        public string blockId;
        public string blockType;
        public string articleId;
        public string actionUrl;
        public float marginTop = -1f;
        public float marginBottom = -1f;
        public ContentServerActionDto action;

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

    [Serializable]
    public sealed class ContentServerActionDto
    {
        public string type;
        public string target;
        public Dictionary<string, string> parameters = new Dictionary<string, string>();
    }
}
