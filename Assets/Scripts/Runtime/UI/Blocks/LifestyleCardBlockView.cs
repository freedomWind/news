using NewsFramework.Data.Blocks;
using NewsFramework.Services.Media;
using NewsFramework.UI.Base;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.Blocks
{
    public sealed class LifestyleCardBlockView : BlockViewBase
    {
        private RawImage thumbnailImage;
        private TextMeshProUGUI thumbnailPlaceholder;
        private TextMeshProUGUI titleLabel;
        private TextMeshProUGUI summaryLabel;
        private TextMeshProUGUI metaLabel;

        public static BlockViewBase Create(Transform parent)
        {
            var root = AppUIFactory.CreateButton("LifestyleCardBlock", parent, AppTheme.Surface, null);
            var view = root.gameObject.AddComponent<LifestyleCardBlockView>();
            AppUIFactory.AddLayoutElement(root.gameObject, 108f);
            AppUIFactory.AddHorizontalLayout(root.gameObject, 12f, new RectOffset(12, 12, 12, 12), TextAnchor.MiddleLeft);

            var thumbnail = AppUIFactory.CreateImage("Thumbnail", root.transform, AppTheme.SurfaceMuted);
            AppUIFactory.AddLayoutElement(thumbnail.gameObject, 84f, 84f);
            view.thumbnailImage = AppUIFactory.CreateObject("ThumbnailImage", thumbnail.transform).AddComponent<RawImage>();
            view.thumbnailImage.color = Color.clear;
            view.thumbnailImage.raycastTarget = false;
            AppUIFactory.Stretch(view.thumbnailImage.rectTransform);

            view.thumbnailPlaceholder = AppUIFactory.CreateText(
                "Placeholder",
                thumbnail.transform,
                "棋",
                18f,
                AppTheme.SecondaryText,
                FontStyles.Bold,
                TextAlignmentOptions.Center);
            AppUIFactory.Stretch(view.thumbnailPlaceholder.rectTransform);

            var textColumn = AppUIFactory.CreateRect("TextColumn", root.transform);
            var textLayout = AppUIFactory.AddLayoutElement(textColumn.gameObject, 84f);
            textLayout.flexibleWidth = 1f;
            AppUIFactory.AddVerticalLayout(textColumn.gameObject, 5f, new RectOffset(0, 0, 0, 0), TextAnchor.UpperLeft);

            view.titleLabel = AppUIFactory.CreateText(
                "Title",
                textColumn,
                string.Empty,
                17f,
                AppTheme.PrimaryText,
                FontStyles.Bold);
            view.titleLabel.maxVisibleLines = 2;
            AppUIFactory.AddLayoutElement(view.titleLabel.gameObject, 40f);

            view.summaryLabel = AppUIFactory.CreateText(
                "Summary",
                textColumn,
                string.Empty,
                13f,
                AppTheme.SecondaryText);
            view.summaryLabel.maxVisibleLines = 1;
            AppUIFactory.AddLayoutElement(view.summaryLabel.gameObject, 19f);

            view.metaLabel = AppUIFactory.CreateText(
                "Meta",
                textColumn,
                string.Empty,
                12f,
                AppTheme.SecondaryText);
            view.metaLabel.maxVisibleLines = 1;
            AppUIFactory.AddLayoutElement(view.metaLabel.gameObject, 18f);

            root.onClick.AddListener(view.TriggerAction);
            return view;
        }

        protected override void OnBind(BlockData data)
        {
            titleLabel.text = data.title ?? string.Empty;
            summaryLabel.text = data.text ?? data.subtitle ?? string.Empty;
            metaLabel.text = data.time ?? data.source ?? string.Empty;
            BindThumbnail(data);
        }

        private void BindThumbnail(BlockData data)
        {
            var request = MediaAssetRequest.Image(data);
            thumbnailImage.texture = null;
            thumbnailImage.color = Color.clear;
            thumbnailPlaceholder.text = "棋";

            if (string.IsNullOrEmpty(request.url))
            {
                return;
            }

            Context.MediaAssetService.LoadTexture(request, result =>
            {
                if (this == null || Data != data || thumbnailImage == null)
                {
                    return;
                }

                if (result == null || !result.success || result.texture == null)
                {
                    thumbnailImage.texture = null;
                    thumbnailImage.color = Color.clear;
                    thumbnailPlaceholder.text = "棋";
                    return;
                }

                thumbnailImage.texture = result.texture;
                thumbnailImage.color = Color.white;
                thumbnailPlaceholder.text = string.Empty;
            });
        }
    }
}
