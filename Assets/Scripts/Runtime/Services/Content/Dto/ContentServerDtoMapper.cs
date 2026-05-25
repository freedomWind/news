using System;
using NewsFramework.Data.Blocks;
using NewsFramework.Services.Content.Cache;

namespace NewsFramework.Services.Content.Dto
{
    public sealed class ContentServerDtoMapper
    {
        private readonly ContentCachePolicy cachePolicy;
        private readonly IContentClock clock;

        public ContentServerDtoMapper(ContentCachePolicy cachePolicy, IContentClock clock)
        {
            this.cachePolicy = cachePolicy ?? new ContentCachePolicy();
            this.clock = clock ?? new SystemContentClock();
        }

        public bool TryMap(ContentServerEnvelopeDto envelope, ContentRequest request, out ContentCacheEntry entry, out string error)
        {
            entry = null;
            error = string.Empty;

            if (envelope == null)
            {
                error = "Content response is empty.";
                return false;
            }

            if (!envelope.IsSuccess)
            {
                error = string.IsNullOrEmpty(envelope.message)
                    ? "Content response code is " + envelope.code
                    : envelope.message;
                return false;
            }

            if (envelope.data == null)
            {
                error = "Content response data is empty.";
                return false;
            }

            var page = MapPage(envelope.data, request);
            if (page == null || string.IsNullOrEmpty(page.pageId))
            {
                error = "Content page is empty.";
                return false;
            }

            var now = clock.UnixSeconds;
            var pageType = !string.IsNullOrEmpty(page.pageType) ? page.pageType : ResolvePageType(request);
            var ttlSeconds = envelope.data.expiresInSeconds > 0
                ? envelope.data.expiresInSeconds
                : cachePolicy.ResolveTtlSeconds(pageType);

            entry = new ContentCacheEntry
            {
                page = page,
                metadata = new ContentCacheMetadata
                {
                    pageId = page.pageId,
                    pageType = pageType,
                    version = envelope.data.version,
                    fetchedAtUnixSeconds = now,
                    expiresAtUnixSeconds = ttlSeconds > 0 ? now + ttlSeconds : 0,
                    source = "http"
                }
            };

            return true;
        }

        private static PageData MapPage(ContentServerPageDto dto, ContentRequest request)
        {
            if (dto == null)
            {
                return null;
            }

            var page = new PageData
            {
                pageId = !string.IsNullOrEmpty(dto.pageId) ? dto.pageId : request?.pageId,
                pageType = !string.IsNullOrEmpty(dto.pageType) ? dto.pageType : ResolvePageType(request),
                title = dto.title
            };

            if (dto.blocks == null)
            {
                return page;
            }

            for (var i = 0; i < dto.blocks.Count; i++)
            {
                var block = MapBlock(dto.blocks[i]);
                if (block != null)
                {
                    page.blocks.Add(block);
                }
            }

            return page;
        }

        public static BlockData MapBlock(ContentServerBlockDto dto)
        {
            if (dto == null)
            {
                return null;
            }

            var id = FirstNonEmpty(dto.id, dto.blockId);
            var type = FirstNonEmpty(dto.type, dto.blockType);

            return new BlockData
            {
                id = id,
                type = type,
                marginTop = dto.marginTop,
                marginBottom = dto.marginBottom,
                action = MapAction(dto),
                badge = dto.badge,
                title = dto.title,
                subtitle = dto.subtitle,
                source = dto.source,
                boardTitle = dto.boardTitle,
                fen = dto.fen,
                text = dto.text,
                time = dto.time,
                url = dto.url,
                posterUrl = dto.posterUrl,
                streamUrl = dto.streamUrl,
                caption = dto.caption,
                aspectRatio = dto.aspectRatio > 0f ? dto.aspectRatio : 1.6f,
                height = dto.height > 0f ? dto.height : 12f,
                durationSeconds = dto.durationSeconds,
                media = dto.media,
                replay = dto.replay
            };
        }

        private static BlockActionData MapAction(ContentServerBlockDto blockDto)
        {
            if (blockDto == null)
            {
                return BlockActionData.None();
            }

            var dto = blockDto.action;
            if (dto == null || string.IsNullOrEmpty(dto.type))
            {
                return MapImplicitAction(blockDto);
            }

            var action = new BlockActionData
            {
                type = dto.type,
                target = string.IsNullOrEmpty(dto.target) ? blockDto.articleId : dto.target
            };

            if (dto.parameters != null)
            {
                foreach (var pair in dto.parameters)
                {
                    if (string.IsNullOrEmpty(pair.Key))
                    {
                        continue;
                    }

                    action.parameters[pair.Key] = pair.Value;
                }
            }

            AddArticleParameterIfNeeded(action, blockDto.articleId);

            return action;
        }

        private static BlockActionData MapImplicitAction(ContentServerBlockDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.articleId))
            {
                return BlockActionData.None();
            }

            var blockType = FirstNonEmpty(dto.type, dto.blockType);
            if (blockType != "article_card" && blockType != "news_item")
            {
                return BlockActionData.None();
            }

            var action = new BlockActionData
            {
                type = "open_article",
                target = string.IsNullOrEmpty(dto.actionUrl) ? dto.articleId : dto.actionUrl
            };
            AddArticleParameterIfNeeded(action, dto.articleId);
            return action;
        }

        private static void AddArticleParameterIfNeeded(BlockActionData action, string articleId)
        {
            if (action == null || string.IsNullOrEmpty(articleId))
            {
                return;
            }

            if (action.parameters == null)
            {
                action.parameters = new System.Collections.Generic.Dictionary<string, string>();
            }

            if (!action.parameters.ContainsKey("articleId"))
            {
                action.parameters["articleId"] = articleId;
            }
        }

        private static string FirstNonEmpty(string first, string second)
        {
            return !string.IsNullOrEmpty(first) ? first : second;
        }

        private static string ResolvePageType(ContentRequest request)
        {
            if (request != null && !string.IsNullOrEmpty(request.pageType))
            {
                return request.pageType;
            }

            if (request != null && request.contentKind == ContentKinds.Article)
            {
                return ContentPageTypes.Article;
            }

            if (request != null && request.contentKind == ContentKinds.Replay)
            {
                return ContentPageTypes.Replay;
            }

            return ContentPageTypes.Page;
        }
    }
}
