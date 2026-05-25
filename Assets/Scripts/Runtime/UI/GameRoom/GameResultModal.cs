using System;
using NewsFramework.UI.Base;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.GameRoom
{
    public static class GameResultModal
    {
        public struct ResultConfig
        {
            public string winnerLabel;       // "红方胜"
            public string redPlayer;         // "张三"
            public string blackPlayer;       // "李四"
            public int totalRounds;          // 42
            public string timeUsed;          // "12:35"
            public Action onGenerateReport;  // 生成AI战报
            public Action onRematch;         // 再来一局
            public Action onGoHome;          // 返回首页
        }

        public struct ReportConfig
        {
            public string reportTitle;       // "张三15回合屏风马绝杀李四"
            public string reportSubtitle;    // "屏风马破当头炮·午间挑战赛"
            public string imageUrl;          // 关键帧截图 URL（可选）
            public Action onShareWechat;     // 分享到微信
            public Action onRematch;
            public Action onGoHome;
        }

        public static GameObject Show(ResultConfig config)
        {
            var canvas = UnityEngine.Object.FindFirstObjectByType<Canvas>();
            if (canvas == null) return null;

            var dimmer = ModalOverlay.CreateDimmer(canvas.transform);
            var closeAll = new Action(() => ModalOverlay.Close(dimmer.gameObject));

            dimmer.GetComponent<Button>().onClick.AddListener(() => closeAll());

            BuildResultPanel(dimmer.transform, config, closeAll);

            return dimmer.gameObject;
        }

        public static void ShowReport(GameObject overlay, ReportConfig config)
        {
            if (overlay == null) return;

            var dimmer = overlay.GetComponent<Image>();
            if (dimmer == null) return;

            // 销毁旧面板
            var oldPanel = dimmer.transform.Find("ModalPanel");
            if (oldPanel != null) UnityEngine.Object.Destroy(oldPanel.gameObject);

            var closeAll = new Action(() => ModalOverlay.Close(overlay));
            BuildReportPanel(dimmer.transform, config, closeAll);
        }

        // ──── 结果面板 ────

        private static void BuildResultPanel(Transform parent, ResultConfig c, Action close)
        {
            var panelRect = ModalOverlay.BuildPanel(parent, null, null, null, 310f);

            // 标题区
            var titleArea = AppUIFactory.CreateRect("TitleArea", panelRect);
            AppUIFactory.AddVerticalLayout(titleArea.gameObject, 8f,
                new RectOffset(0, 0, 4, 4), TextAnchor.UpperCenter);

            var trophy = AppUIFactory.CreateText("Trophy", titleArea.transform,
                "🏆", 36f, Color.white, FontStyles.Bold,
                TextAlignmentOptions.Center);
            AppUIFactory.AddLayoutElement(trophy.gameObject, 48f);

            var title = AppUIFactory.CreateText("Winner", titleArea.transform,
                c.winnerLabel, 28f, AppTheme.PrimaryText, FontStyles.Bold,
                TextAlignmentOptions.Center);
            AppUIFactory.AddLayoutElement(title.gameObject, 38f);

            // 对局信息
            var info = AppUIFactory.CreateText("Info", panelRect.transform,
                $"{c.redPlayer}  vs  {c.blackPlayer}",
                14f, AppTheme.SecondaryText, FontStyles.Normal,
                TextAlignmentOptions.Center);
            AppUIFactory.AddLayoutElement(info.gameObject, 22f);

            var stats = AppUIFactory.CreateText("Stats", panelRect.transform,
                $"{c.totalRounds}回合 · 用时 {c.timeUsed}",
                13f, AppTheme.SecondaryText, FontStyles.Normal,
                TextAlignmentOptions.Center);
            AppUIFactory.AddLayoutElement(stats.gameObject, 20f);

            // 间距
            var spacer = AppUIFactory.CreateRect("Spacer", panelRect);
            AppUIFactory.AddLayoutElement(spacer.gameObject, 12f);

            // 按钮（垂直排列）
            BuildVerticalButton(panelRect, "生成AI战报",
                ModalButtonStyle.Destructive, 220f, () =>
                {
                    c.onGenerateReport?.Invoke();
                });

            BuildOutlineButton(panelRect, "再来一局", 220f, () =>
            {
                c.onRematch?.Invoke();
                close();
            });

            // 返回首页（文字链接）
            var homeLink = AppUIFactory.CreateText("HomeLink", panelRect,
                "返回首页  ›", 14f, AppTheme.Accent, FontStyles.Normal,
                TextAlignmentOptions.Center);
            AppUIFactory.AddLayoutElement(homeLink.gameObject, 32f);
            var linkBtn = homeLink.gameObject.AddComponent<Button>();
            linkBtn.onClick.AddListener(() => { c.onGoHome?.Invoke(); close(); });
        }

        // ──── 战报面板 ────

        private static void BuildReportPanel(Transform parent, ReportConfig c, Action close)
        {
            var panelRect = ModalOverlay.BuildPanel(parent, null, null, null, 310f);

            // 头部：战报已生成 + 关闭按钮
            var header = AppUIFactory.CreateRect("Header", panelRect);
            AppUIFactory.AddHorizontalLayout(header.gameObject, 0f,
                new RectOffset(0, 0, 0, 0), TextAnchor.MiddleLeft);
            AppUIFactory.AddLayoutElement(header.gameObject, 28f);

            var statusLabel = AppUIFactory.CreateText("Status", header.transform,
                "战报已生成", 14f, Rgb(6, 193, 96), FontStyles.Bold,
                TextAlignmentOptions.Left);
            var statusLayout = AppUIFactory.AddLayoutElement(statusLabel.gameObject, 28f);
            statusLayout.flexibleWidth = 1f;

            var closeBtn = AppUIFactory.CreateButton("CloseBtn", header.transform,
                Color.clear, () => close());
            var closeRect = closeBtn.GetComponent<RectTransform>();
            closeRect.sizeDelta = new Vector2(28f, 28f);
            AppUIFactory.CreateText("X", closeBtn.transform,
                "✕", 16f, AppTheme.SecondaryText, FontStyles.Normal,
                TextAlignmentOptions.Center);

            // 战报标题
            var reportTitle = AppUIFactory.CreateText("ReportTitle", panelRect.transform,
                c.reportTitle, 20f, AppTheme.PrimaryText, FontStyles.Bold,
                TextAlignmentOptions.Left);
            reportTitle.lineSpacing = 6f;
            AppUIFactory.AddLayoutElement(reportTitle.gameObject, -1f);

            // 战报副标题
            if (!string.IsNullOrEmpty(c.reportSubtitle))
            {
                var subtitle = AppUIFactory.CreateText("ReportSubtitle", panelRect.transform,
                    c.reportSubtitle, 13f, AppTheme.SecondaryText, FontStyles.Normal,
                    TextAlignmentOptions.Left);
                AppUIFactory.AddLayoutElement(subtitle.gameObject, 20f);
            }

            // 棋盘快照区域
            var imageArea = AppUIFactory.CreateImage("SnapshotArea", panelRect.transform,
                new Color(0.93f, 0.86f, 0.75f)); // ~#EEDCBE
            var imgRect = imageArea.rectTransform;
            imgRect.sizeDelta = new Vector2(0f, 160f);
            AppUIFactory.AddLayoutElement(imageArea.gameObject, 160f);

            if (!string.IsNullOrEmpty(c.imageUrl))
            {
                // 占位：后续接入图片加载
                var placeholder = AppUIFactory.CreateText("ImgPlaceholder", imageArea.transform,
                    "关键帧", 13f, Rgb(139, 69, 19), FontStyles.Normal,
                    TextAlignmentOptions.Center);
                AppUIFactory.Stretch(placeholder.rectTransform);
            }

            // 按钮
            var spacer = AppUIFactory.CreateRect("Spacer", panelRect);
            AppUIFactory.AddLayoutElement(spacer.gameObject, 8f);

            BuildVerticalButton(panelRect, "分享到微信",
                ModalButtonStyle.Normal, 220f, () =>
                {
                    c.onShareWechat?.Invoke();
                }, new Color(0.027f, 0.757f, 0.376f)); // WeChat green

            var hint = AppUIFactory.CreateText("Hint", panelRect,
                "稍后可在 \"我的战报\" 中查看", 11f,
                AppTheme.TabInactive, FontStyles.Normal, TextAlignmentOptions.Center);
            AppUIFactory.AddLayoutElement(hint.gameObject, 18f);

            BuildOutlineButton(panelRect, "再来一局", 220f, () =>
            {
                c.onRematch?.Invoke();
                close();
            });

            var homeLink = AppUIFactory.CreateText("HomeLink", panelRect,
                "返回首页  ›", 14f, AppTheme.Accent, FontStyles.Normal,
                TextAlignmentOptions.Center);
            AppUIFactory.AddLayoutElement(homeLink.gameObject, 32f);
            var linkBtn = homeLink.gameObject.AddComponent<Button>();
            linkBtn.onClick.AddListener(() => { c.onGoHome?.Invoke(); close(); });
        }

        // ──── 按钮工厂 ────

        private static void BuildVerticalButton(Transform parent, string label,
            ModalButtonStyle style, float width, Action onClick,
            Color? customBg = null)
        {
            var row = AppUIFactory.CreateRect("BtnRow", parent);
            AppUIFactory.AddVerticalLayout(row.gameObject, 0f,
                new RectOffset(0, 0, 0, 0), TextAnchor.UpperCenter);
            row.sizeDelta = new Vector2(width, 48f);
            var rowLayout = AppUIFactory.AddLayoutElement(row.gameObject, 48f, width);

            var def = new ModalOverlay.ButtonDef(label, onClick, style);
            var button = ModalOverlay.BuildButton(row, def);
            var btnRect = button.GetComponent<RectTransform>();
            btnRect.sizeDelta = new Vector2(width, 44f);

            if (customBg.HasValue)
            {
                button.targetGraphic.color = customBg.Value;
                var colors = button.colors;
                colors.highlightedColor = AppUIFactory.Tint(customBg.Value, 0.92f);
                colors.pressedColor = AppUIFactory.Tint(customBg.Value, 0.84f);
                button.colors = colors;
            }
        }

        private static void BuildOutlineButton(Transform parent, string label, float width, Action onClick)
        {
            var row = AppUIFactory.CreateRect("BtnRow", parent);
            AppUIFactory.AddVerticalLayout(row.gameObject, 0f,
                new RectOffset(0, 0, 0, 0), TextAnchor.UpperCenter);
            row.sizeDelta = new Vector2(width, 48f);
            AppUIFactory.AddLayoutElement(row.gameObject, 48f, width);

            var btn = AppUIFactory.CreateButton("Btn_" + label, row, PieceCream,
                new UnityEngine.Events.UnityAction(onClick));
            var btnRect = btn.GetComponent<RectTransform>();
            btnRect.sizeDelta = new Vector2(width, 44f);

            var outline = btn.gameObject.AddComponent<Outline>();
            outline.effectColor = Rgb(238, 220, 190); // #EEDCBE border
            outline.effectDistance = new Vector2(1f, -1f);

            AppUIFactory.CreateText("Label", btn.transform,
                label, 15f, AppTheme.PrimaryText, FontStyles.Normal,
                TextAlignmentOptions.Center);
        }

        // ──── 颜色 ────

        private static readonly Color PieceCream = new Color(0.996f, 0.965f, 0.925f); // #FDF6EC

        private static Color Rgb(byte r, byte g, byte b) => new Color32(r, g, b, 255);
    }
}
