using NewsFramework.Data.Blocks;
using NewsFramework.Services.Media;
using NewsFramework.UI.Base;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.Blocks
{
    public sealed class NewsItemBlockView : BlockViewBase
    {
        private LayoutElement rootLayout;
        private RectTransform thumbnailRoot;
        private RawImage thumbnailImage;
        private TextMeshProUGUI thumbnailPlaceholder;
        private TextMeshProUGUI titleLabel;
        private TextMeshProUGUI metaLabel;

        public static BlockViewBase Create(Transform parent)
        {
            var root = AppUIFactory.CreateButton("NewsItemBlock", parent, AppTheme.PageBackground, null);
            var view = root.gameObject.AddComponent<NewsItemBlockView>();
            view.rootLayout = AppUIFactory.AddLayoutElement(root.gameObject, 86f);
            AppUIFactory.AddVerticalLayout(root.gameObject, 7f, new RectOffset(0, 0, 4, 6));

            var contentRow = AppUIFactory.CreateRect("ContentRow", root.transform);
            AppUIFactory.AddLayoutElement(contentRow.gameObject, 72f);
            AppUIFactory.AddHorizontalLayout(contentRow.gameObject, 10f, new RectOffset(0, 0, 0, 0));

            var textColumn = AppUIFactory.CreateRect("TextColumn", contentRow);
            var textLayout = AppUIFactory.AddLayoutElement(textColumn.gameObject, 72f);
            textLayout.flexibleWidth = 1f;
            AppUIFactory.AddVerticalLayout(textColumn.gameObject, 5f, new RectOffset(0, 0, 0, 0));

            view.titleLabel = AppUIFactory.CreateText(
                "Title",
                textColumn,
                string.Empty,
                16f,
                AppTheme.PrimaryText,
                FontStyles.Bold);
            view.titleLabel.maxVisibleLines = 2;

            view.metaLabel = AppUIFactory.CreateText(
                "Meta",
                textColumn,
                string.Empty,
                12f,
                AppTheme.SecondaryText);
            view.metaLabel.maxVisibleLines = 1;

            view.thumbnailRoot = AppUIFactory.CreateRect("Thumbnail", contentRow);
            AppUIFactory.AddLayoutElement(view.thumbnailRoot.gameObject, 72f, 96f);
            var thumbnailBackground = view.thumbnailRoot.gameObject.AddComponent<Image>();
            thumbnailBackground.color = AppTheme.SurfaceMuted;

            view.thumbnailImage = AppUIFactory.CreateObject("ThumbnailImage", view.thumbnailRoot).AddComponent<RawImage>();
            view.thumbnailImage.color = Color.clear;
            view.thumbnailImage.raycastTarget = false;
            AppUIFactory.Stretch(view.thumbnailImage.rectTransform);

            view.thumbnailPlaceholder = AppUIFactory.CreateText(
                "ThumbnailPlaceholder",
                view.thumbnailRoot,
                string.Empty,
                11f,
                AppTheme.SecondaryText,
                FontStyles.Normal,
                TextAlignmentOptions.Center);
            AppUIFactory.Stretch(view.thumbnailPlaceholder.rectTransform);

            var divider = AppUIFactory.CreateImage("Divider", root.transform, AppTheme.Hairline);
            AppUIFactory.AddLayoutElement(divider.gameObject, 1f);

            root.onClick.AddListener(view.TriggerAction);
            return view;
        }

        protected override void OnBind(BlockData data)
        {
            titleLabel.text = "◆ " + (data.title ?? string.Empty);
            metaLabel.text = BuildMeta(data);
            BindThumbnail(data);
        }

        private void BindThumbnail(BlockData data)
        {
            var request = MediaAssetRequest.Image(data);
            var hasThumbnail = !string.IsNullOrEmpty(request.url);
            thumbnailRoot.gameObject.SetActive(hasThumbnail);
            rootLayout.preferredHeight = hasThumbnail ? 86f : 76f;

            thumbnailImage.texture = null;
            thumbnailImage.color = Color.clear;
            thumbnailPlaceholder.text = string.Empty;

            if (!hasThumbnail)
            {
                return;
            }

            thumbnailPlaceholder.text = "图";
            Context.MediaAssetService.LoadTexture(request, result => HandleThumbnailLoaded(data, result));
        }

        private void HandleThumbnailLoaded(BlockData boundData, MediaAssetResult result)
        {
            if (this == null || Data != boundData || thumbnailImage == null)
            {
                return;
            }

            if (result == null || !result.success || result.texture == null)
            {
                thumbnailPlaceholder.text = "图";
                thumbnailImage.texture = null;
                thumbnailImage.color = Color.clear;
                return;
            }

            thumbnailImage.texture = result.texture;
            thumbnailImage.color = Color.white;
            thumbnailPlaceholder.text = string.Empty;
        }

        private static string BuildMeta(BlockData data)
        {
            if (string.IsNullOrEmpty(data.source))
            {
                return data.time ?? string.Empty;
            }

            if (string.IsNullOrEmpty(data.time))
            {
                return data.source;
            }

            return data.source + " | " + data.time;
        }
    }
}
