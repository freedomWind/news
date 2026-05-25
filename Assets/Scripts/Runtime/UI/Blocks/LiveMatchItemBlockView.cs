using NewsFramework.Data.Blocks;
using NewsFramework.UI.Base;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.Blocks
{
    public sealed class LiveMatchItemBlockView : BlockViewBase
    {
        private TextMeshProUGUI iconLabel;
        private TextMeshProUGUI titleLabel;
        private TextMeshProUGUI subtitleLabel;

        public static BlockViewBase Create(Transform parent)
        {
            var root = AppUIFactory.CreateButton("LiveMatchItemBlock", parent, AppTheme.Surface, null);
            var view = root.gameObject.AddComponent<LiveMatchItemBlockView>();
            AppUIFactory.AddLayoutElement(root.gameObject, 72f);
            AppUIFactory.AddHorizontalLayout(root.gameObject, 12f, new RectOffset(12, 12, 10, 10), TextAnchor.MiddleLeft);

            var iconRoot = AppUIFactory.CreateImage("IconRoot", root.transform, AppTheme.SurfaceMuted);
            AppUIFactory.AddLayoutElement(iconRoot.gameObject, 52f, 52f);
            view.iconLabel = AppUIFactory.CreateText(
                "Icon",
                iconRoot.transform,
                "♞",
                24f,
                AppTheme.SecondaryText,
                FontStyles.Normal,
                TextAlignmentOptions.Center);
            AppUIFactory.Stretch(view.iconLabel.rectTransform);

            var textColumn = AppUIFactory.CreateRect("TextColumn", root.transform);
            var textLayout = AppUIFactory.AddLayoutElement(textColumn.gameObject, 52f);
            textLayout.flexibleWidth = 1f;
            AppUIFactory.AddVerticalLayout(textColumn.gameObject, 4f, new RectOffset(0, 0, 2, 0), TextAnchor.MiddleLeft);

            view.titleLabel = AppUIFactory.CreateText(
                "Title",
                textColumn,
                string.Empty,
                17f,
                AppTheme.PrimaryText,
                FontStyles.Bold);
            view.titleLabel.maxVisibleLines = 1;
            AppUIFactory.AddLayoutElement(view.titleLabel.gameObject, 24f);

            view.subtitleLabel = AppUIFactory.CreateText(
                "Subtitle",
                textColumn,
                string.Empty,
                13f,
                AppTheme.SecondaryText);
            view.subtitleLabel.maxVisibleLines = 1;
            AppUIFactory.AddLayoutElement(view.subtitleLabel.gameObject, 20f);

            var watch = AppUIFactory.CreateImage("WatchButton", root.transform, AppTheme.Accent);
            AppUIFactory.AddLayoutElement(watch.gameObject, 42f, 52f);
            var watchLabel = AppUIFactory.CreateText(
                "Label",
                watch.transform,
                "围观",
                14f,
                Color.white,
                FontStyles.Bold,
                TextAlignmentOptions.Center);
            AppUIFactory.Stretch(watchLabel.rectTransform);

            root.onClick.AddListener(view.TriggerAction);
            return view;
        }

        protected override void OnBind(BlockData data)
        {
            iconLabel.text = string.IsNullOrEmpty(data.badge) ? "♞" : data.badge;
            titleLabel.text = data.title ?? string.Empty;
            subtitleLabel.text = data.subtitle ?? data.text ?? string.Empty;
        }
    }
}
