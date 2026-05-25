using System;
using NewsFramework.Data.Blocks;
using NewsFramework.Data.Features;
using NewsFramework.UI.Base;
using NewsFramework.UI.Rendering;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.Features
{
    public sealed class SettingsListPrefabSectionView : MonoBehaviour, IDataBoundView<FeatureSectionData>
    {
        private Action<BlockActionData> onAction;
        private bool built;

        public void Bind(FeatureSectionData data, Action<BlockActionData> actionHandler)
        {
            onAction = actionHandler;
            EnsureBuilt();
            ClearRows();

            var count = data != null && data.items != null ? data.items.Count : 0;
            var layoutElement = GetComponent<LayoutElement>();
            layoutElement.preferredHeight = Mathf.Max(1, count) * 56f;

            for (var i = 0; i < count; i++)
            {
                CreateSettingRow(data.items[i], i < count - 1);
            }
        }

        private void EnsureBuilt()
        {
            if (built)
            {
                return;
            }

            built = true;
            if (GetComponent<RectTransform>() == null)
            {
                gameObject.AddComponent<RectTransform>();
            }

            var background = GetComponent<Image>();
            if (background == null)
            {
                background = gameObject.AddComponent<Image>();
            }

            background.color = AppTheme.Surface;

            if (GetComponent<LayoutElement>() == null)
            {
                AppUIFactory.AddLayoutElement(gameObject, 56f);
            }

            if (GetComponent<VerticalLayoutGroup>() == null)
            {
                AppUIFactory.AddVerticalLayout(gameObject, 0f, new RectOffset(0, 0, 0, 0));
            }
        }

        private void CreateSettingRow(FeatureItemData item, bool divider)
        {
            var itemId = item != null && !string.IsNullOrEmpty(item.id) ? item.id : "empty";
            var rowButton = AppUIFactory.CreateButton(
                "PrefabSetting_" + itemId,
                transform,
                AppTheme.Surface,
                () => TriggerItemAction(item));
            AppUIFactory.AddLayoutElement(rowButton.gameObject, 56f);
            AppUIFactory.AddHorizontalLayout(rowButton.gameObject, 8f, new RectOffset(14, 14, 0, 0), TextAnchor.MiddleLeft);

            var title = AppUIFactory.CreateText(
                "Title",
                rowButton.transform,
                item != null ? item.title ?? string.Empty : string.Empty,
                16f,
                AppTheme.PrimaryText,
                FontStyles.Normal,
                TextAlignmentOptions.MidlineLeft);
            var titleLayout = AppUIFactory.AddLayoutElement(title.gameObject, 52f);
            titleLayout.flexibleWidth = 1f;

            var valueColor = item != null && item.state == "accent" ? AppTheme.Accent : AppTheme.SecondaryText;
            var value = AppUIFactory.CreateText(
                "Value",
                rowButton.transform,
                item != null ? item.value ?? string.Empty : string.Empty,
                14f,
                valueColor,
                FontStyles.Normal,
                TextAlignmentOptions.MidlineRight);
            AppUIFactory.AddLayoutElement(value.gameObject, 52f, 142f);

            if (divider)
            {
                CreateDivider(rowButton.transform);
            }
        }

        private static void CreateDivider(Transform parent)
        {
            var line = AppUIFactory.CreateImage("Divider", parent, AppTheme.Hairline);
            var lineLayout = line.gameObject.AddComponent<LayoutElement>();
            lineLayout.ignoreLayout = true;
            line.rectTransform.anchorMin = Vector2.zero;
            line.rectTransform.anchorMax = new Vector2(1f, 0f);
            line.rectTransform.pivot = new Vector2(0.5f, 0f);
            line.rectTransform.offsetMin = new Vector2(14f, 0f);
            line.rectTransform.offsetMax = new Vector2(-14f, 0f);
        }

        private void TriggerItemAction(FeatureItemData item)
        {
            if (!FeatureViewHelpers.HasAction(item))
            {
                return;
            }

            onAction?.Invoke(item.action);
        }

        private void ClearRows()
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i);
                if (Application.isPlaying)
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }
    }
}
