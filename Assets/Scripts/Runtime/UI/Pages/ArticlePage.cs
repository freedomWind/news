using System;
using NewsFramework.Data.Articles;
using NewsFramework.Data.Blocks;
using NewsFramework.Services.Article;
using NewsFramework.Services.Content;
using NewsFramework.UI.Base;
using NewsFramework.UI.Blocks;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.Pages
{
    public sealed class ArticlePage : MonoBehaviour
    {
        private RectTransform contentRoot;
        private RectTransform footerRoot;
        private BlockRenderer blockRenderer;
        private TextMeshProUGUI titleLabel;
        private TextMeshProUGUI bookmarkIconLabel;
        private TextMeshProUGUI flowerButtonLabel;
        private TextMeshProUGUI flowerFeedbackLabel;
        private TextMeshProUGUI commentInputPlaceholderLabel;
        private IContentService contentService;
        private IArticleEngagementService engagementService;
        private string articleId;
        private Action onBack;
        private Action<BlockActionData> onAction;
        private bool bookmarked;
        private bool flowered;
        private bool canComment = true;
        private int flowerCount;
        private int engagementRequestVersion;
        private string activeEngagementArticleId;
        private GameObject commentPreviewDivider;
        private RectTransform commentPreviewRoot;

        public void Build(
            RectTransform parent,
            IContentService service,
            string targetArticleId,
            Action backHandler,
            Action<BlockActionData> actionHandler,
            string knownVersion = "",
            BlockRenderContext renderContext = null,
            IArticleEngagementService articleEngagementService = null)
        {
            contentService = service;
            engagementService = articleEngagementService ?? new MockArticleEngagementService();
            articleId = targetArticleId;
            onBack = backHandler;
            onAction = actionHandler;

            var background = AppUIFactory.CreateImage("ArticleBackground", parent, AppTheme.PageBackground);
            AppUIFactory.Stretch(background.rectTransform);

            BuildTopBar(parent, "详情");
            BuildContentArea(parent);
            BuildCommentInputBar(parent);

            blockRenderer = gameObject.AddComponent<BlockRenderer>();
            blockRenderer.Initialize(contentRoot, BlockRegistry.CreateDefault(), HandleBlockAction, renderContext);

            if (contentService == null)
            {
                Debug.LogWarning("Content service is missing for article: " + articleId);
                return;
            }

            contentService.PageUpdated += HandlePageUpdated;
            contentService.LoadPage(ContentRequest.Article(articleId, knownVersion), HandleLoadResult);
        }

        private void BuildTopBar(RectTransform parent, string title)
        {
            var topBar = AppUIFactory.CreateImage("ArticleTopBar", parent, AppTheme.Surface);
            AppUIFactory.AnchorTop(topBar.rectTransform, 56f);

            var line = AppUIFactory.CreateImage("BottomLine", topBar.transform, AppTheme.Hairline);
            line.rectTransform.anchorMin = Vector2.zero;
            line.rectTransform.anchorMax = new Vector2(1f, 0f);
            line.rectTransform.pivot = new Vector2(0.5f, 0f);
            line.rectTransform.offsetMin = Vector2.zero;
            line.rectTransform.offsetMax = new Vector2(0f, 1f);

            var back = AppUIFactory.CreateButton("BackButton", topBar.transform, AppTheme.Surface, () => onBack?.Invoke());
            var backRect = back.GetComponent<RectTransform>();
            backRect.anchorMin = new Vector2(0f, 0f);
            backRect.anchorMax = new Vector2(0f, 1f);
            backRect.offsetMin = Vector2.zero;
            backRect.offsetMax = new Vector2(56f, 0f);

            var backIcon = AppUIFactory.CreateText(
                "Icon",
                back.transform,
                "‹",
                34f,
                AppTheme.PrimaryText,
                FontStyles.Normal,
                TextAlignmentOptions.Center);
            AppUIFactory.Stretch(backIcon.rectTransform);

            titleLabel = AppUIFactory.CreateText(
                "Title",
                topBar.transform,
                title,
                16f,
                AppTheme.PrimaryText,
                FontStyles.Bold,
                TextAlignmentOptions.Center);
            titleLabel.maxVisibleLines = 1;
            titleLabel.rectTransform.anchorMin = Vector2.zero;
            titleLabel.rectTransform.anchorMax = Vector2.one;
            titleLabel.rectTransform.offsetMin = new Vector2(62f, 0f);
            titleLabel.rectTransform.offsetMax = new Vector2(-62f, 0f);

            var bookmark = AppUIFactory.CreateButton("BookmarkButton", topBar.transform, AppTheme.Surface, ToggleBookmark);
            var bookmarkRect = bookmark.GetComponent<RectTransform>();
            bookmarkRect.anchorMin = new Vector2(1f, 0f);
            bookmarkRect.anchorMax = Vector2.one;
            bookmarkRect.offsetMin = new Vector2(-56f, 0f);
            bookmarkRect.offsetMax = Vector2.zero;

            bookmarkIconLabel = AppUIFactory.CreateText(
                "Icon",
                bookmark.transform,
                "☆",
                24f,
                AppTheme.SecondaryText,
                FontStyles.Bold,
                TextAlignmentOptions.Center);
            AppUIFactory.Stretch(bookmarkIconLabel.rectTransform);
            UpdateBookmarkVisual();
        }

        private void BuildContentArea(RectTransform parent)
        {
            var scrollRoot = AppUIFactory.CreateRect("ArticleScroll", parent);
            scrollRoot.anchorMin = Vector2.zero;
            scrollRoot.anchorMax = Vector2.one;
            scrollRoot.offsetMin = new Vector2(0f, 72f);
            scrollRoot.offsetMax = new Vector2(0f, -56f);

            var scrollRect = scrollRoot.gameObject.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Elastic;
            scrollRect.scrollSensitivity = 28f;

            var viewport = AppUIFactory.CreateImage("Viewport", scrollRoot, AppTheme.PageBackground);
            AppUIFactory.Stretch(viewport.rectTransform);
            var mask = viewport.gameObject.AddComponent<Mask>();
            mask.showMaskGraphic = false;
            scrollRect.viewport = viewport.rectTransform;

            contentRoot = AppUIFactory.CreateRect("Content", viewport.transform);
            contentRoot.anchorMin = new Vector2(0f, 1f);
            contentRoot.anchorMax = Vector2.one;
            contentRoot.pivot = new Vector2(0.5f, 1f);
            contentRoot.anchoredPosition = Vector2.zero;
            contentRoot.sizeDelta = Vector2.zero;
            scrollRect.content = contentRoot;

            AppUIFactory.AddVerticalLayout(
                contentRoot.gameObject,
                16f,
                new RectOffset((int)AppTheme.PagePadding, (int)AppTheme.PagePadding, 18, 26));

            var fitter = contentRoot.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        }

        private void BuildCommentInputBar(RectTransform parent)
        {
            var footer = AppUIFactory.CreateImage("ArticleCommentInputBar", parent, AppTheme.PageBackground);
            footerRoot = footer.rectTransform;
            AppUIFactory.AnchorBottom(footerRoot, 72f);

            var line = AppUIFactory.CreateImage("TopLine", footer.transform, AppTheme.Hairline);
            line.rectTransform.anchorMin = new Vector2(0f, 1f);
            line.rectTransform.anchorMax = Vector2.one;
            line.rectTransform.pivot = new Vector2(0.5f, 1f);
            line.rectTransform.offsetMin = new Vector2(0f, -1f);
            line.rectTransform.offsetMax = Vector2.zero;

            var row = AppUIFactory.CreateRect("InputRow", footer.transform);
            row.anchorMin = Vector2.zero;
            row.anchorMax = Vector2.one;
            row.offsetMin = new Vector2(16f, 10f);
            row.offsetMax = new Vector2(-12f, -10f);
            AppUIFactory.AddHorizontalLayout(row.gameObject, 10f, new RectOffset(0, 0, 0, 0), TextAnchor.MiddleLeft);

            var input = AppUIFactory.CreateButton("CommentInput", row, AppTheme.Hairline, OpenCommentInput);
            var inputLayout = AppUIFactory.AddLayoutElement(input.gameObject, 44f);
            inputLayout.flexibleWidth = 1f;

            commentInputPlaceholderLabel = AppUIFactory.CreateText(
                "Placeholder",
                input.transform,
                "我也说一句...",
                15f,
                AppTheme.SecondaryText,
                FontStyles.Normal,
                TextAlignmentOptions.MidlineLeft);
            commentInputPlaceholderLabel.rectTransform.anchorMin = Vector2.zero;
            commentInputPlaceholderLabel.rectTransform.anchorMax = Vector2.one;
            commentInputPlaceholderLabel.rectTransform.offsetMin = new Vector2(16f, 0f);
            commentInputPlaceholderLabel.rectTransform.offsetMax = new Vector2(-12f, 0f);

            var emoji = AppUIFactory.CreateButton("EmojiButton", row, AppTheme.PageBackground, OpenCommentInput);
            AppUIFactory.AddLayoutElement(emoji.gameObject, 44f, 44f);
            var emojiLabel = AppUIFactory.CreateText(
                "Icon",
                emoji.transform,
                "☺",
                22f,
                AppTheme.PrimaryText,
                FontStyles.Normal,
                TextAlignmentOptions.Center);
            AppUIFactory.Stretch(emojiLabel.rectTransform);
        }

        private void HandleBlockAction(BlockActionData action)
        {
            onAction?.Invoke(action);
        }

        private void HandleLoadResult(ContentResult result)
        {
            if (result == null || !result.success)
            {
                Debug.LogWarning("Article content load failed: " + (result != null ? result.error : "empty result"));
                return;
            }

            RenderArticle(result.page);
        }

        private void HandlePageUpdated(ContentResult result)
        {
            if (!MatchesCurrentArticle(result))
            {
                return;
            }

            RenderArticle(result.page);
        }

        private void RenderArticle(PageData page)
        {
            if (page == null)
            {
                return;
            }

            if (titleLabel != null)
            {
                titleLabel.text = string.IsNullOrEmpty(page.title) ? "详情" : page.title;
            }

            blockRenderer.Render(page);
            commentPreviewDivider = null;
            commentPreviewRoot = null;
            AppendArticleFooter();
            var engagementArticleId = string.IsNullOrEmpty(page.pageId) ? articleId : page.pageId;
            RequestEngagementSummary(engagementArticleId);
        }

        private void AppendArticleFooter()
        {
            CreateEndMarker();
            CreateEngagementSection();
        }

        private void CreateEndMarker()
        {
            var marker = AppUIFactory.CreateRect("ArticleEndMarker", contentRoot);
            AppUIFactory.AddLayoutElement(marker.gameObject, 54f);

            var label = AppUIFactory.CreateText(
                "Marker",
                marker,
                "◆",
                22f,
                AppTheme.Accent,
                FontStyles.Bold,
                TextAlignmentOptions.Center);
            AppUIFactory.Stretch(label.rectTransform);
        }

        private void CreateEngagementSection()
        {
            var section = AppUIFactory.CreateRect("ArticleEngagement", contentRoot);
            AppUIFactory.AddLayoutElement(section.gameObject, 158f);
            AppUIFactory.AddVerticalLayout(section.gameObject, 10f, new RectOffset(0, 0, 0, 12), TextAnchor.UpperCenter);

            var prompt = AppUIFactory.CreateText(
                "Prompt",
                section,
                "这篇文章对你有帮助吗？",
                15f,
                AppTheme.SecondaryText,
                FontStyles.Normal,
                TextAlignmentOptions.Center);
            AppUIFactory.AddLayoutElement(prompt.gameObject, 24f);

            var flower = AppUIFactory.CreateButton("FlowerButton", section, AppTheme.SurfaceMuted, GiveFlower);
            AppUIFactory.AddLayoutElement(flower.gameObject, 44f, 120f);
            flowerButtonLabel = AppUIFactory.CreateText(
                "Label",
                flower.transform,
                BuildFlowerText(),
                15f,
                AppTheme.PrimaryText,
                FontStyles.Bold,
                TextAlignmentOptions.Center);
            AppUIFactory.Stretch(flowerButtonLabel.rectTransform);

            flowerFeedbackLabel = AppUIFactory.CreateText(
                "FlowerFeedback",
                section,
                string.Empty,
                13f,
                AppTheme.Accent,
                FontStyles.Bold,
                TextAlignmentOptions.Center);
            AppUIFactory.AddLayoutElement(flowerFeedbackLabel.gameObject, 20f);

            var share = AppUIFactory.CreateButton("ShareButton", section, AppTheme.SurfaceMuted, ShareArticle);
            AppUIFactory.AddLayoutElement(share.gameObject, 44f);
            var shareLabel = AppUIFactory.CreateText(
                "Label",
                share.transform,
                "分享给棋友",
                15f,
                AppTheme.PrimaryText,
                FontStyles.Bold,
                TextAlignmentOptions.Center);
            AppUIFactory.Stretch(shareLabel.rectTransform);
        }

        private void RequestEngagementSummary(string engagementArticleId)
        {
            activeEngagementArticleId = engagementArticleId;
            var requestVersion = ++engagementRequestVersion;
            if (engagementService == null)
            {
                return;
            }

            engagementService.LoadSummary(
                engagementArticleId,
                summary => RenderEngagementSummary(engagementArticleId, requestVersion, summary));
        }

        private void RenderEngagementSummary(
            string requestArticleId,
            int requestVersion,
            ArticleEngagementSummaryData summary)
        {
            if (!IsCurrentEngagementResult(requestArticleId, requestVersion, summary))
            {
                return;
            }

            bookmarked = summary.bookmarked;
            flowered = summary.flowered;
            flowerCount = summary.flowerCount;
            canComment = summary.canComment;
            UpdateBookmarkVisual();
            UpdateFlowerVisual();
            UpdateCommentInputVisual();
            RenderCommentPreview(summary);
        }

        private void RenderCommentPreview(ArticleEngagementSummaryData summary)
        {
            if (summary == null || contentRoot == null)
            {
                return;
            }

            ClearCommentPreview();
            var divider = AppUIFactory.CreateImage("CommentDivider", contentRoot, AppTheme.Hairline);
            commentPreviewDivider = divider.gameObject;
            AppUIFactory.AddLayoutElement(divider.gameObject, 1f);

            var section = AppUIFactory.CreateRect("ArticleCommentsPreview", contentRoot);
            commentPreviewRoot = section;
            var count = summary.previewComments != null ? summary.previewComments.Count : 0;
            AppUIFactory.AddLayoutElement(section.gameObject, CalculateCommentsPreviewHeight(summary));
            AppUIFactory.AddVerticalLayout(section.gameObject, 0f, new RectOffset(0, 0, 8, 10), TextAnchor.UpperLeft);

            var title = AppUIFactory.CreateText(
                "Title",
                section,
                "棋友怎么说",
                18f,
                AppTheme.PrimaryText,
                FontStyles.Bold,
                TextAlignmentOptions.Left);
            AppUIFactory.AddLayoutElement(title.gameObject, 42f);

            for (var i = 0; i < count; i++)
            {
                CreateCommentRow(section, summary.previewComments[i], i < count - 1);
            }

            var more = AppUIFactory.CreateButton("MoreCommentsButton", section, AppTheme.PageBackground, OpenAllComments);
            AppUIFactory.AddLayoutElement(more.gameObject, 44f);
            var moreLabel = AppUIFactory.CreateText(
                "Label",
                more.transform,
                "查看全部 " + summary.commentCount + " 条评论 >",
                14f,
                AppTheme.Accent,
                FontStyles.Bold,
                TextAlignmentOptions.Center);
            AppUIFactory.Stretch(moreLabel.rectTransform);
        }

        private bool IsCurrentEngagementResult(
            string requestArticleId,
            int requestVersion,
            ArticleEngagementSummaryData summary)
        {
            if (this == null || summary == null)
            {
                return false;
            }

            if (requestVersion != engagementRequestVersion)
            {
                return false;
            }

            if (!string.Equals(requestArticleId, activeEngagementArticleId, StringComparison.Ordinal))
            {
                return false;
            }

            return string.IsNullOrEmpty(summary.articleId)
                || string.Equals(summary.articleId, requestArticleId, StringComparison.Ordinal);
        }

        private void ClearCommentPreview()
        {
            DestroyGeneratedObject(commentPreviewDivider);
            DestroyGeneratedObject(commentPreviewRoot != null ? commentPreviewRoot.gameObject : null);
            commentPreviewDivider = null;
            commentPreviewRoot = null;
        }

        private static void DestroyGeneratedObject(GameObject target)
        {
            if (target == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                UnityEngine.Object.Destroy(target);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(target);
            }
        }

        private void CreateCommentRow(Transform parent, ArticleCommentData comment, bool divider)
        {
            if (comment == null)
            {
                return;
            }

            var hasReply = comment.authorReply != null && !string.IsNullOrEmpty(comment.authorReply.text);
            var rowHeight = hasReply ? 134f : 86f;
            var row = AppUIFactory.CreateRect("Comment_" + comment.commentId, parent);
            AppUIFactory.AddLayoutElement(row.gameObject, rowHeight);
            AppUIFactory.AddHorizontalLayout(row.gameObject, 10f, new RectOffset(0, 0, 0, 0), TextAnchor.UpperLeft);

            var avatar = AppUIFactory.CreateImage("Avatar", row, AppTheme.Hairline);
            AppUIFactory.AddLayoutElement(avatar.gameObject, 36f, 36f);
            var avatarLabel = AppUIFactory.CreateText(
                "Label",
                avatar.transform,
                string.IsNullOrEmpty(comment.avatarText) ? "棋" : comment.avatarText,
                14f,
                AppTheme.SecondaryText,
                FontStyles.Bold,
                TextAlignmentOptions.Center);
            AppUIFactory.Stretch(avatarLabel.rectTransform);

            var body = AppUIFactory.CreateRect("Body", row);
            var bodyLayout = AppUIFactory.AddLayoutElement(body.gameObject, rowHeight);
            bodyLayout.flexibleWidth = 1f;
            AppUIFactory.AddVerticalLayout(body.gameObject, 4f, new RectOffset(0, 0, 0, 0), TextAnchor.UpperLeft);

            var header = AppUIFactory.CreateRect("Header", body);
            AppUIFactory.AddLayoutElement(header.gameObject, 22f);
            AppUIFactory.AddHorizontalLayout(header.gameObject, 8f, new RectOffset(0, 0, 0, 0), TextAnchor.MiddleLeft);

            var name = AppUIFactory.CreateText("Name", header, comment.authorName ?? string.Empty, 15f, AppTheme.PrimaryText, FontStyles.Bold);
            var nameLayout = AppUIFactory.AddLayoutElement(name.gameObject, 22f);
            nameLayout.flexibleWidth = 1f;

            var time = AppUIFactory.CreateText("Time", header, comment.time ?? string.Empty, 12f, AppTheme.SecondaryText, FontStyles.Normal, TextAlignmentOptions.MidlineRight);
            AppUIFactory.AddLayoutElement(time.gameObject, 22f, 72f);

            var text = AppUIFactory.CreateText("Text", body, comment.text ?? string.Empty, 15f, AppTheme.PrimaryText, FontStyles.Normal);
            text.lineSpacing = 2f;
            AppUIFactory.AddLayoutElement(text.gameObject, hasReply ? 42f : 38f);

            if (hasReply)
            {
                var reply = AppUIFactory.CreateImage("AuthorReply", body, AppTheme.Rgb(251, 247, 237));
                AppUIFactory.AddLayoutElement(reply.gameObject, 50f);
                var replyLabel = AppUIFactory.CreateText(
                    "ReplyText",
                    reply.transform,
                    comment.authorReply.authorName + "：" + comment.authorReply.text,
                    13f,
                    AppTheme.PrimaryText,
                    FontStyles.Normal);
                replyLabel.rectTransform.anchorMin = Vector2.zero;
                replyLabel.rectTransform.anchorMax = Vector2.one;
                replyLabel.rectTransform.offsetMin = new Vector2(10f, 6f);
                replyLabel.rectTransform.offsetMax = new Vector2(-10f, -6f);
            }

            if (!divider)
            {
                return;
            }

            var line = AppUIFactory.CreateImage("Divider", body, AppTheme.Hairline);
            var lineLayout = line.gameObject.AddComponent<LayoutElement>();
            lineLayout.ignoreLayout = true;
            line.rectTransform.anchorMin = Vector2.zero;
            line.rectTransform.anchorMax = new Vector2(1f, 0f);
            line.rectTransform.pivot = new Vector2(0.5f, 0f);
            line.rectTransform.offsetMin = Vector2.zero;
            line.rectTransform.offsetMax = Vector2.zero;
        }

        private static float CalculateCommentsPreviewHeight(ArticleEngagementSummaryData summary)
        {
            var height = 58f + 54f;
            if (summary == null || summary.previewComments == null)
            {
                return height;
            }

            for (var i = 0; i < summary.previewComments.Count; i++)
            {
                var comment = summary.previewComments[i];
                var hasReply = comment != null
                    && comment.authorReply != null
                    && !string.IsNullOrEmpty(comment.authorReply.text);
                height += hasReply ? 134f : 86f;
            }

            return height;
        }

        private void ToggleBookmark()
        {
            bookmarked = !bookmarked;
            UpdateBookmarkVisual();
        }

        private void GiveFlower()
        {
            if (flowered)
            {
                return;
            }

            flowered = true;
            flowerCount++;
            UpdateFlowerVisual();

            if (flowerFeedbackLabel != null)
            {
                flowerFeedbackLabel.text = "王五 献了一朵花";
            }

            onAction?.Invoke(new BlockActionData
            {
                type = "article_flower",
                target = articleId
            });
        }

        private void ShareArticle()
        {
            onAction?.Invoke(new BlockActionData
            {
                type = "article_share",
                target = articleId
            });
        }

        private void OpenAllComments()
        {
            onAction?.Invoke(new BlockActionData
            {
                type = "open_article_comments",
                target = articleId
            });
        }

        private void OpenCommentInput()
        {
            if (!canComment)
            {
                return;
            }

            onAction?.Invoke(new BlockActionData
            {
                type = "open_comment_input",
                target = articleId
            });
        }

        private void UpdateBookmarkVisual()
        {
            if (bookmarkIconLabel == null)
            {
                return;
            }

            bookmarkIconLabel.text = bookmarked ? "★" : "☆";
            bookmarkIconLabel.color = bookmarked ? AppTheme.Accent : AppTheme.SecondaryText;
        }

        private void UpdateFlowerVisual()
        {
            if (flowerButtonLabel == null)
            {
                return;
            }

            flowerButtonLabel.text = BuildFlowerText();
            flowerButtonLabel.color = flowered ? AppTheme.Accent : AppTheme.PrimaryText;
        }

        private string BuildFlowerText()
        {
            if (flowerCount > 0)
            {
                return flowered ? "已献花 " + flowerCount : "献花 " + flowerCount;
            }

            return flowered ? "已献花" : "献花";
        }

        private void UpdateCommentInputVisual()
        {
            if (commentInputPlaceholderLabel == null)
            {
                return;
            }

            commentInputPlaceholderLabel.text = canComment ? "我也说一句..." : "登录后评论";
        }

        private bool MatchesCurrentArticle(ContentResult result)
        {
            if (result == null || string.IsNullOrEmpty(articleId))
            {
                return false;
            }

            return result.page != null && result.page.pageId == articleId
                || result.metadata != null && result.metadata.pageId == articleId;
        }

        private void OnDestroy()
        {
            engagementRequestVersion++;
            activeEngagementArticleId = string.Empty;

            if (contentService != null)
            {
                contentService.PageUpdated -= HandlePageUpdated;
            }
        }
    }
}
