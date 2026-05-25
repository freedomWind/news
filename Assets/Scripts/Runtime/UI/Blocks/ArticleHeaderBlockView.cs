using NewsFramework.Data.Blocks;
using NewsFramework.UI.Base;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;

namespace NewsFramework.UI.Blocks
{
    public sealed class ArticleHeaderBlockView : BlockViewBase
    {
        private TextMeshProUGUI titleLabel;
        private TextMeshProUGUI metaLabel;

        public static BlockViewBase Create(Transform parent)
        {
            var root = AppUIFactory.CreateRect("ArticleHeaderBlock", parent);
            var view = root.gameObject.AddComponent<ArticleHeaderBlockView>();
            AppUIFactory.AddLayoutElement(root.gameObject, 124f);
            AppUIFactory.AddVerticalLayout(root.gameObject, 8f, new RectOffset(0, 0, 4, 8));

            view.titleLabel = AppUIFactory.CreateText(
                "Title",
                root,
                string.Empty,
                24f,
                AppTheme.PrimaryText,
                FontStyles.Bold);
            view.titleLabel.maxVisibleLines = 3;

            view.metaLabel = AppUIFactory.CreateText(
                "Meta",
                root,
                string.Empty,
                13f,
                AppTheme.SecondaryText);

            return view;
        }

        protected override void OnBind(BlockData data)
        {
            titleLabel.text = data.title ?? string.Empty;
            metaLabel.text = BuildMeta(data);
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
