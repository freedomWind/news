using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using NewsFramework.UI.Theme;

namespace NewsFramework.UI.Base
{
    public static class AppUIFactory
    {
        public static GameObject CreateObject(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            if (parent != null)
            {
                go.transform.SetParent(parent, false);
            }

            return go;
        }

        public static Canvas CreateOverlayCanvas(string name = "AppCanvas")
        {
            var canvasObject = CreateObject(name, null);
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = false;

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(AppTheme.ScreenWidth, AppTheme.ScreenHeight);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        public static void EnsureEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null)
            {
                return;
            }

            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        public static RectTransform CreateRect(string name, Transform parent)
        {
            return CreateObject(name, parent).GetComponent<RectTransform>();
        }

        public static Image CreateImage(string name, Transform parent, Color color)
        {
            var image = CreateObject(name, parent).AddComponent<Image>();
            image.color = color;
            return image;
        }

        public static Button CreateButton(string name, Transform parent, Color background, UnityAction onClick)
        {
            var image = CreateImage(name, parent, background);
            var button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            if (onClick != null)
            {
                button.onClick.AddListener(onClick);
            }

            var colors = button.colors;
            colors.highlightedColor = Tint(background, 0.96f);
            colors.pressedColor = Tint(background, 0.9f);
            colors.selectedColor = background;
            button.colors = colors;
            return button;
        }

        public static TextMeshProUGUI CreateText(
            string name,
            Transform parent,
            string text,
            float fontSize,
            Color color,
            FontStyles style = FontStyles.Normal,
            TextAlignmentOptions alignment = TextAlignmentOptions.Left)
        {
            var label = CreateObject(name, parent).AddComponent<TextMeshProUGUI>();
            label.text = text;
            label.fontSize = fontSize;
            label.color = color;
            label.fontStyle = style;
            label.alignment = alignment;
            label.enableWordWrapping = true;
            label.overflowMode = TextOverflowModes.Ellipsis;
            label.raycastTarget = false;
            label.margin = Vector4.zero;

            if ((style & FontStyles.Bold) != 0 && AppTheme.FontAssetBold != null)
            {
                label.font = AppTheme.FontAssetBold;
            }
            else if (AppTheme.FontAsset != null)
            {
                label.font = AppTheme.FontAsset;
            }

            return label;
        }

        public static VerticalLayoutGroup AddVerticalLayout(
            GameObject target,
            float spacing,
            RectOffset padding,
            TextAnchor alignment = TextAnchor.UpperLeft)
        {
            var layout = target.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = alignment;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.spacing = spacing;
            layout.padding = padding;
            return layout;
        }

        public static HorizontalLayoutGroup AddHorizontalLayout(
            GameObject target,
            float spacing,
            RectOffset padding,
            TextAnchor alignment = TextAnchor.MiddleLeft)
        {
            var layout = target.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = alignment;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            layout.spacing = spacing;
            layout.padding = padding;
            return layout;
        }

        public static LayoutElement AddLayoutElement(GameObject target, float preferredHeight = -1f, float preferredWidth = -1f)
        {
            var layout = target.AddComponent<LayoutElement>();
            if (preferredHeight >= 0f)
            {
                layout.preferredHeight = preferredHeight;
            }

            if (preferredWidth >= 0f)
            {
                layout.preferredWidth = preferredWidth;
            }

            return layout;
        }

        public static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        public static void AnchorTop(RectTransform rect, float height)
        {
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 1f);
            rect.offsetMin = new Vector2(0f, -height);
            rect.offsetMax = Vector2.zero;
        }

        public static void AnchorBottom(RectTransform rect, float height)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = new Vector2(0f, height);
        }

        public static void SetOffsets(RectTransform rect, float left, float top, float right, float bottom)
        {
            rect.offsetMin = new Vector2(left, bottom);
            rect.offsetMax = new Vector2(-right, -top);
        }

        public static Color Tint(Color color, float factor)
        {
            return new Color(
                Mathf.Clamp01(color.r * factor),
                Mathf.Clamp01(color.g * factor),
                Mathf.Clamp01(color.b * factor),
                color.a);
        }
    }
}
