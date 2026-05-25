using NewsFramework.Data.Features;
using NewsFramework.UI.Base;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.Features
{
    public sealed class SettingsListSectionView : FeatureSectionViewBase
    {
        private Transform rowRoot;

        public static FeatureSectionViewBase Create(Transform parent)
        {
            var root = FeatureViewHelpers.CreateCard("SettingsList", parent);
            var view = root.gameObject.AddComponent<SettingsListSectionView>();
            view.rowRoot = root.transform;
            AppUIFactory.AddVerticalLayout(root.gameObject, 0f, new RectOffset(0, 0, 0, 0));
            return view;
        }

        protected override void OnBind(FeatureSectionData data)
        {
            var count = data.items != null ? data.items.Count : 0;
            AppUIFactory.AddLayoutElement(gameObject, Mathf.Max(1, count) * 56f);

            for (var i = 0; i < count; i++)
            {
                CreateSettingRow(data.items[i], i < count - 1);
            }
        }

        private void CreateSettingRow(FeatureItemData item, bool divider)
        {
            var rowButton = AppUIFactory.CreateButton("Setting_" + item.id, rowRoot, AppTheme.Surface, () => TriggerItemAction(item));
            AppUIFactory.AddLayoutElement(rowButton.gameObject, 56f);
            AppUIFactory.AddHorizontalLayout(rowButton.gameObject, 8f, new RectOffset(14, 14, 0, 0), TextAnchor.MiddleLeft);

            var title = AppUIFactory.CreateText("Title", rowButton.transform, item.title ?? string.Empty, 16f, AppTheme.PrimaryText, FontStyles.Normal, TextAlignmentOptions.MidlineLeft);
            var titleLayout = AppUIFactory.AddLayoutElement(title.gameObject, 52f);
            titleLayout.flexibleWidth = 1f;

            var value = AppUIFactory.CreateText("Value", rowButton.transform, item.value ?? string.Empty, 14f, item.state == "accent" ? AppTheme.Accent : AppTheme.SecondaryText, FontStyles.Normal, TextAlignmentOptions.MidlineRight);
            AppUIFactory.AddLayoutElement(value.gameObject, 52f, 142f);

            if (!divider)
            {
                return;
            }

            var line = AppUIFactory.CreateImage("Divider", rowButton.transform, AppTheme.Hairline);
            var lineLayout = line.gameObject.AddComponent<LayoutElement>();
            lineLayout.ignoreLayout = true;
            line.rectTransform.anchorMin = Vector2.zero;
            line.rectTransform.anchorMax = new Vector2(1f, 0f);
            line.rectTransform.pivot = new Vector2(0.5f, 0f);
            line.rectTransform.offsetMin = new Vector2(14f, 0f);
            line.rectTransform.offsetMax = new Vector2(-14f, 0f);
        }
    }
}
