using NewsFramework.Data.Blocks;
using NewsFramework.Services.Media;
using NewsFramework.UI.Base;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.Blocks
{
    public sealed class VideoBlockView : BlockViewBase
    {
        private LayoutElement rootLayout;
        private LayoutElement posterLayout;
        private RawImage posterImage;
        private TextMeshProUGUI placeholderLabel;
        private TextMeshProUGUI playLabel;
        private TextMeshProUGUI titleLabel;
        private TextMeshProUGUI metaLabel;

        public static BlockViewBase Create(Transform parent)
        {
            var root = AppUIFactory.CreateRect("VideoBlock", parent);
            var view = root.gameObject.AddComponent<VideoBlockView>();
            view.rootLayout = AppUIFactory.AddLayoutElement(root.gameObject, 272f);
            AppUIFactory.AddVerticalLayout(root.gameObject, 8f, new RectOffset(0, 0, 0, 0));

            var poster = AppUIFactory.CreateImage("PosterSurface", root, AppTheme.SurfaceMuted);
            view.posterLayout = AppUIFactory.AddLayoutElement(poster.gameObject, 210f);

            view.posterImage = AppUIFactory.CreateObject("PosterImage", poster.transform).AddComponent<RawImage>();
            view.posterImage.color = Color.clear;
            view.posterImage.raycastTarget = false;
            AppUIFactory.Stretch(view.posterImage.rectTransform);

            view.placeholderLabel = AppUIFactory.CreateText(
                "Placeholder",
                poster.transform,
                string.Empty,
                13f,
                AppTheme.SecondaryText,
                FontStyles.Normal,
                TextAlignmentOptions.Center);
            AppUIFactory.Stretch(view.placeholderLabel.rectTransform);

            view.playLabel = AppUIFactory.CreateText(
                "Play",
                poster.transform,
                "▶",
                34f,
                Color.white,
                FontStyles.Bold,
                TextAlignmentOptions.Center);
            AppUIFactory.Stretch(view.playLabel.rectTransform);

            view.titleLabel = AppUIFactory.CreateText(
                "Title",
                root,
                string.Empty,
                15f,
                AppTheme.PrimaryText,
                FontStyles.Bold,
                TextAlignmentOptions.Left);
            AppUIFactory.AddLayoutElement(view.titleLabel.gameObject, 22f);

            view.metaLabel = AppUIFactory.CreateText(
                "Meta",
                root,
                string.Empty,
                12f,
                AppTheme.SecondaryText,
                FontStyles.Normal,
                TextAlignmentOptions.Left);
            AppUIFactory.AddLayoutElement(view.metaLabel.gameObject, 18f);

            return view;
        }

        protected override void OnBind(BlockData data)
        {
            var videoRequest = MediaAssetRequest.Video(data);
            var posterHeight = ResolveHeight(videoRequest.aspectRatio);
            posterLayout.preferredHeight = posterHeight;
            rootLayout.preferredHeight = posterHeight + 56f;

            posterImage.texture = null;
            posterImage.color = Color.clear;
            titleLabel.text = string.IsNullOrEmpty(data.title) ? "视频" : data.title;
            metaLabel.text = FormatDuration(videoRequest.durationSeconds);
            playLabel.enabled = true;

            Context.MediaAssetService.ResolveVideoSource(videoRequest, result =>
            {
                if (this == null || Data != data || metaLabel == null)
                {
                    return;
                }

                if (result == null || !result.success)
                {
                    metaLabel.text = "视频源未配置";
                }
            });

            if (string.IsNullOrEmpty(videoRequest.posterUrl))
            {
                placeholderLabel.text = "视频封面";
                return;
            }

            placeholderLabel.text = "视频封面加载中";
            var posterRequest = new MediaAssetRequest
            {
                mediaId = string.IsNullOrEmpty(videoRequest.mediaId) ? string.Empty : videoRequest.mediaId + "_poster",
                mediaType = MediaAssetTypes.Image,
                url = videoRequest.posterUrl,
                version = videoRequest.version,
                hash = videoRequest.hash,
                aspectRatio = videoRequest.aspectRatio
            };

            Context.MediaAssetService.LoadTexture(posterRequest, result => HandlePosterLoaded(data, result));
        }

        private void HandlePosterLoaded(BlockData boundData, MediaAssetResult result)
        {
            if (this == null || Data != boundData || posterImage == null)
            {
                return;
            }

            if (result == null || !result.success || result.texture == null)
            {
                placeholderLabel.text = "视频封面加载失败";
                posterImage.texture = null;
                posterImage.color = Color.clear;
                return;
            }

            posterImage.texture = result.texture;
            posterImage.color = Color.white;
            placeholderLabel.text = string.Empty;
        }

        private static float ResolveHeight(float aspectRatio)
        {
            var ratio = aspectRatio <= 0.1f ? 1.777f : aspectRatio;
            return Mathf.Clamp(343f / ratio, 150f, 240f);
        }

        private static string FormatDuration(float seconds)
        {
            if (seconds <= 0f)
            {
                return "视频";
            }

            var totalSeconds = Mathf.RoundToInt(seconds);
            var minutes = totalSeconds / 60;
            var remainder = totalSeconds % 60;
            return minutes + ":" + remainder.ToString("00");
        }
    }
}
