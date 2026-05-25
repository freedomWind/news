using NewsFramework.Data.Blocks;
using NewsFramework.Services.Media;
using NewsFramework.UI.Base;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.Blocks
{
    public sealed class ImageBlockView : BlockViewBase
    {
        private LayoutElement rootLayout;
        private LayoutElement imageLayout;
        private RawImage rawImage;
        private TextMeshProUGUI placeholderLabel;
        private TextMeshProUGUI captionLabel;

        public static BlockViewBase Create(Transform parent)
        {
            var root = AppUIFactory.CreateRect("ImageBlock", parent);
            var view = root.gameObject.AddComponent<ImageBlockView>();
            view.rootLayout = AppUIFactory.AddLayoutElement(root.gameObject, 244f);
            AppUIFactory.AddVerticalLayout(root.gameObject, 6f, new RectOffset(0, 0, 0, 0));

            var image = AppUIFactory.CreateImage("ImageSurface", root, AppTheme.SurfaceMuted);
            view.imageLayout = AppUIFactory.AddLayoutElement(image.gameObject, 210f);

            view.rawImage = AppUIFactory.CreateObject("LoadedImage", image.transform).AddComponent<RawImage>();
            view.rawImage.color = Color.clear;
            view.rawImage.raycastTarget = false;
            AppUIFactory.Stretch(view.rawImage.rectTransform);

            view.placeholderLabel = AppUIFactory.CreateText(
                "Placeholder",
                image.transform,
                string.Empty,
                13f,
                AppTheme.SecondaryText,
                FontStyles.Normal,
                TextAlignmentOptions.Center);
            AppUIFactory.Stretch(view.placeholderLabel.rectTransform);

            view.captionLabel = AppUIFactory.CreateText(
                "Caption",
                root,
                string.Empty,
                12f,
                AppTheme.SecondaryText,
                FontStyles.Normal,
                TextAlignmentOptions.Center);
            AppUIFactory.AddLayoutElement(view.captionLabel.gameObject, 24f);

            return view;
        }

        protected override void OnBind(BlockData data)
        {
            var imageHeight = ResolveHeight(data.aspectRatio);
            imageLayout.preferredHeight = imageHeight;
            rootLayout.preferredHeight = imageHeight + 30f;
            rawImage.texture = null;
            rawImage.color = Color.clear;
            captionLabel.text = data.caption ?? string.Empty;

            var request = MediaAssetRequest.Image(data);
            if (string.IsNullOrEmpty(request.url))
            {
                placeholderLabel.text = "图片占位";
                return;
            }

            placeholderLabel.text = "图片加载中";
            Context.MediaAssetService.LoadTexture(request, result => HandleImageLoaded(data, result));
        }

        private void HandleImageLoaded(BlockData boundData, MediaAssetResult result)
        {
            if (this == null || Data != boundData || rawImage == null)
            {
                return;
            }

            if (result == null || !result.success || result.texture == null)
            {
                placeholderLabel.text = "图片加载失败";
                rawImage.texture = null;
                rawImage.color = Color.clear;
                return;
            }

            rawImage.texture = result.texture;
            rawImage.color = Color.white;
            placeholderLabel.text = string.Empty;
        }

        private static float ResolveHeight(float aspectRatio)
        {
            var ratio = aspectRatio <= 0.1f ? 1.6f : aspectRatio;
            return Mathf.Clamp(343f / ratio, 140f, 260f);
        }
    }
}
