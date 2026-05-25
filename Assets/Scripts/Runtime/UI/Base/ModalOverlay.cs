using System;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.Base
{
    public enum ModalButtonStyle
    {
        Normal,
        Destructive,
        Warning
    }

    public static class ModalOverlay
    {
        public struct Config
        {
            public string title;
            public string body;
            public ButtonDef[] buttons;
        }

        public struct ButtonDef
        {
            public string label;
            public Action onClick;
            public ModalButtonStyle style;

            public ButtonDef(string label, Action onClick = null, ModalButtonStyle style = ModalButtonStyle.Normal)
            {
                this.label = label;
                this.onClick = onClick;
                this.style = style;
            }
        }

        public static GameObject Show(Config config, Transform parent = null)
        {
            if (parent == null)
            {
                var canvas = UnityEngine.Object.FindFirstObjectByType<Canvas>();
                if (canvas != null) parent = canvas.transform;
            }

            if (parent == null)
            {
                Debug.LogError("ModalOverlay: no Canvas found and no parent provided.");
                return null;
            }

            var dimmer = CreateDimmer(parent);
            var panel = BuildPanel(dimmer.transform, config);
            var closeAction = new Action(() => UnityEngine.Object.Destroy(dimmer.gameObject));

            // 点遮罩关闭
            dimmer.GetComponent<Button>().onClick.AddListener(() => closeAction());

            return dimmer.gameObject;
        }

        public static void Close(GameObject overlay)
        {
            if (overlay != null) UnityEngine.Object.Destroy(overlay);
        }

        // ──── 内部构建 ────

        internal static Image CreateDimmer(Transform parent)
        {
            var dimmer = AppUIFactory.CreateImage("ModalDimmer", parent,
                new Color(0f, 0f, 0f, 0.55f));
            dimmer.raycastTarget = true;
            dimmer.gameObject.AddComponent<Button>();
            AppUIFactory.Stretch(dimmer.rectTransform);
            return dimmer;
        }

        internal static RectTransform BuildPanel(Transform parent, string title, string body,
            ButtonDef[] buttons, float panelWidth = 284f)
        {
            var panel = AppUIFactory.CreateImage("ModalPanel", parent, AppTheme.Surface);
            var panelRect = panel.rectTransform;
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            panelRect.sizeDelta = new Vector2(panelWidth, 120f);

            AppUIFactory.AddVerticalLayout(panel.gameObject, 16f,
                new RectOffset(24, 24, 24, 24), TextAnchor.UpperCenter);

            if (!string.IsNullOrEmpty(title))
            {
                var t = AppUIFactory.CreateText("Title", panel.transform,
                    title, 18f, AppTheme.PrimaryText, FontStyles.Bold,
                    TextAlignmentOptions.Center);
                AppUIFactory.AddLayoutElement(t.gameObject, 28f);
            }

            if (!string.IsNullOrEmpty(body))
            {
                var b = AppUIFactory.CreateText("Body", panel.transform,
                    body, 14f, AppTheme.SecondaryText, FontStyles.Normal,
                    TextAlignmentOptions.Center);
                b.lineSpacing = 4f;
                var bodyLayout = AppUIFactory.AddLayoutElement(b.gameObject, -1f);
                bodyLayout.flexibleHeight = 1f;
            }

            if (buttons != null && buttons.Length > 0)
            {
                BuildButtonRow(panel.transform, buttons);
            }

            return panelRect;
        }

        private static RectTransform BuildPanel(Transform parent, Config config)
        {
            return BuildPanel(parent, config.title, config.body, config.buttons);
        }

        internal static void BuildButtonRow(Transform panel, ButtonDef[] buttons)
        {
            var buttonRow = AppUIFactory.CreateRect("ButtonRow", panel);
            var rowLayout = AppUIFactory.AddHorizontalLayout(buttonRow.gameObject, 10f,
                new RectOffset(0, 8, 0, 0), TextAnchor.MiddleCenter);
            rowLayout.childForceExpandWidth = false;
            rowLayout.childControlWidth = false;
            AppUIFactory.AddLayoutElement(buttonRow.gameObject, 42f);

            foreach (var def in buttons)
            {
                BuildButton(buttonRow, def);
            }
        }

        internal static Button BuildButton(Transform parent, ButtonDef def)
        {
            var (bg, text) = ResolveButtonColors(def.style);

            var button = AppUIFactory.CreateButton("Btn_" + def.label, parent, bg, null);
            var btnRect = button.GetComponent<RectTransform>();
            btnRect.sizeDelta = new Vector2(0f, 38f);
            var btnLayout = AppUIFactory.AddLayoutElement(button.gameObject, 38f);
            btnLayout.minWidth = 72f;
            btnLayout.flexibleWidth = 1f;

            var label = AppUIFactory.CreateText("Label", button.transform,
                def.label, 15f, text, FontStyles.Bold, TextAlignmentOptions.Center);
            AppUIFactory.Stretch(label.rectTransform);

            button.onClick.AddListener(() => def.onClick?.Invoke());
            return button;
        }

        private static (Color bg, Color text) ResolveButtonColors(ModalButtonStyle style)
        {
            return style switch
            {
                ModalButtonStyle.Destructive => (AppTheme.Accent, Color.white),
                ModalButtonStyle.Warning => (Rgb(212, 168, 83), AppTheme.PrimaryText),
                _ => (AppTheme.SurfaceMuted, AppTheme.PrimaryText)
            };
        }

        private static Color Rgb(byte r, byte g, byte b) => new Color32(r, g, b, 255);
    }
}
