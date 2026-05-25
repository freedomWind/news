using System;
using System.Collections.Generic;
using System.Linq;
using NewsFramework.Data.Blocks;
using NewsFramework.Data.Features;
using NewsFramework.Data.GameRoom;
using NewsFramework.Data.Mock;
using NewsFramework.Data.Replay;
using NewsFramework.Services.Content;
using NewsFramework.Services.Content.Runtime;
using NewsFramework.Services.Media;
using NewsFramework.UI.Base;
using NewsFramework.UI.Blocks;
using NewsFramework.UI.Features;
using NewsFramework.UI.GameRoom;
using NewsFramework.UI.Pages;
using UnityEditor;
using UnityEngine;

namespace NewsFramework.Editor
{
    public sealed class UIPreviewWindow : EditorWindow
    {
        private static readonly Dictionary<string, string[]> Catalog = new Dictionary<string, string[]>
        {
            ["Pages"] = new[]
            {
                "HomePage",
                "ArticlePage",
                "GameRoomPage (Player)",
                "GameRoomPage (Spectator)"
            },
            ["Blocks"] = new[]
            {
                "article_header", "article_tip_card", "board_preview", "divider",
                "featured_match", "image", "lifestyle_card", "live_match_item",
                "news_item", "paragraph", "replay", "replay_preview",
                "section_title", "spacer", "video"
            },
            ["Feature Sections"] = new[]
            {
                "about_footer", "achievement_strip", "empty_state",
                "feature_action_card", "profile_header", "quick_action_grid",
                "rank_list", "recent_match_list", "section_header",
                "settings_list", "stats_row"
            },
            ["Modals"] = new[]
            {
                "ModalOverlay (Normal)",
                "ModalOverlay (Destructive)",
                "ModalOverlay (Warning)",
                "GameResultModal (Result)",
                "GameResultModal (Report)"
            }
        };

        private string selectedCategory;
        private string selectedItem;
        private GameObject previewRoot;
        private Canvas previewCanvas;
        private Vector2 listScroll;
        private int frameDelay;

        [MenuItem("NewsFramework/UI Preview")]
        public static void Open() => GetWindow<UIPreviewWindow>("UI Preview");

        private void OnEnable()
        {
            Selection.selectionChanged += Repaint;
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= Repaint;
            DestroyPreview();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            DrawCatalog();
            DrawPreview();
            EditorGUILayout.EndHorizontal();
        }

        // ──── 左侧分类列表 ────

        private void DrawCatalog()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(220f));
            listScroll = EditorGUILayout.BeginScrollView(listScroll);

            foreach (var kv in Catalog)
            {
                var isOpen = selectedCategory == kv.Key;
                var newOpen = EditorGUILayout.Foldout(isOpen, kv.Key, true);
                if (newOpen && !isOpen)
                {
                    selectedCategory = kv.Key;
                }

                if (newOpen)
                {
                    EditorGUI.indentLevel++;
                    foreach (var item in kv.Value)
                    {
                        var itemSelected = selectedCategory == kv.Key && selectedItem == item;
                        GUI.backgroundColor = itemSelected ? new Color(0.4f, 0.6f, 0.9f) : Color.white;
                        if (GUILayout.Button(item, GUILayout.Height(24f)))
                        {
                            SelectItem(kv.Key, item);
                        }
                    }
                    GUI.backgroundColor = Color.white;
                    EditorGUI.indentLevel--;
                }
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            EditorGUILayout.LabelField("", GUI.skin.verticalSlider, GUILayout.Width(2f), GUILayout.ExpandHeight(true));
        }

        // ──── 右侧预览区 ────

        private void DrawPreview()
        {
            EditorGUILayout.BeginVertical();

            if (!string.IsNullOrEmpty(selectedItem))
            {
                EditorGUILayout.LabelField($"{selectedCategory} / {selectedItem}", EditorStyles.boldLabel);
                EditorGUILayout.Space(4f);
            }

            var previewRect = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            EditorGUI.DrawRect(previewRect, new Color(0.22f, 0.22f, 0.22f));

            if (previewCanvas != null)
            {
                previewCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(previewRect.width, previewRect.height);
            }

            if (frameDelay > 0)
            {
                frameDelay--;
                if (frameDelay == 0) Repaint();
            }

            EditorGUILayout.EndVertical();
        }

        // ──── 选中逻辑 ────

        private void SelectItem(string category, string item)
        {
            selectedCategory = category;
            selectedItem = item;
            DestroyPreview();
            BuildPreview();
            Repaint();
            frameDelay = 1;
        }

        // ──── 预览构建 ────

        private void BuildPreview()
        {
            previewCanvas = AppUIFactory.CreateOverlayCanvas("PreviewCanvas");
            previewCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            previewCanvas.worldCamera = Camera.current;
            previewCanvas.planeDistance = 1f;
            previewCanvas.sortingOrder = 999;

            previewRoot = new GameObject("PreviewRoot", typeof(RectTransform));
            var rootRt = previewRoot.GetComponent<RectTransform>();
            rootRt.SetParent(previewCanvas.transform, false);
            rootRt.anchorMin = Vector2.zero;
            rootRt.anchorMax = Vector2.one;
            rootRt.offsetMin = Vector2.zero;
            rootRt.offsetMax = Vector2.zero;

            switch (selectedCategory)
            {
                case "Pages": BuildPagePreview(rootRt); break;
                case "Blocks": BuildBlockPreview(rootRt); break;
                case "Feature Sections": BuildFeaturePreview(rootRt); break;
                case "Modals": BuildModalPreview(rootRt); break;
            }
        }

        private void DestroyPreview()
        {
            if (previewRoot != null) DestroyImmediate(previewRoot);
            if (previewCanvas != null) DestroyImmediate(previewCanvas.gameObject);
            previewRoot = null;
            previewCanvas = null;
        }

        // ──── Pages ────

        private void BuildPagePreview(RectTransform parent)
        {
            AppUIFactory.EnsureEventSystem();

            switch (selectedItem)
            {
                case "HomePage":
                {
                    var config = new ContentRuntimeConfig { mode = ContentRuntimeMode.Mock };
                    parent.gameObject.AddComponent<HomePage>().Build(config);
                    break;
                }
                case "ArticlePage":
                {
                    var contentService = ContentServiceFactory.CreateMockCachedService();
                    parent.gameObject.AddComponent<ArticlePage>().Build(
                        parent, contentService, "article_001", null, _ => { });
                    break;
                }
                case "GameRoomPage (Player)":
                {
                    var roomData = GameRoomMockData.CreatePlayerRoom();
                    parent.gameObject.AddComponent<GameRoomPage>().Build(parent, roomData, null);
                    break;
                }
                case "GameRoomPage (Spectator)":
                {
                    var roomData = GameRoomMockData.CreateSpectatorRoom("preview_match");
                    parent.gameObject.AddComponent<GameRoomPage>().Build(parent, roomData, null);
                    break;
                }
            }
        }

        // ──── Blocks ────

        private void BuildBlockPreview(RectTransform parent)
        {
            var blockData = CreateMockBlockData(selectedItem);
            if (blockData == null) return;

            var registry = BlockRegistry.CreateDefault();
            var view = registry.Create(blockData, parent);
            view.Bind(blockData, _ =>
            {
                if (blockData.action != null)
                    Debug.Log($"Block action: {blockData.action.type} -> {blockData.action.target}");
            });
        }

        private static BlockData CreateMockBlockData(string type)
        {
            return type switch
            {
                "featured_match" => new BlockData
                {
                    type = "featured_match", title = "午间挑战赛",
                    subtitle = "张三 vs 李四", badge = "进行中",
                    time = "第 32 回合", action = new BlockActionData { type = "open_match", target = "match_live" }
                },
                "board_preview" => new BlockData
                {
                    type = "board_preview", title = "精彩残局", subtitle = "红先胜",
                    fen = "rnbakabnr/9/1c5c1/p1p1p1p1p/9/9/P1P1P1P1P/1C5C1/9/RNBAKABNR w",
                    action = new BlockActionData { type = "open_replay", target = "board_001" }
                },
                "section_title" => new BlockData { type = "section_title", title = "今日棋坛" },
                "news_item" => new BlockData
                {
                    type = "news_item", title = "2026年全国象棋锦标赛开幕",
                    subtitle = "32位选手齐聚杭州", source = "华夏棋苑", time = "2小时前",
                    action = new BlockActionData { type = "open_article", target = "article_001" }
                },
                "live_match_item" => new BlockData
                {
                    type = "live_match_item", title = "张三 vs 李四",
                    subtitle = "第42回合", badge = "直播中",
                    action = new BlockActionData { type = "open_match", target = "match_live" }
                },
                "article_tip_card" => new BlockData
                {
                    type = "article_tip_card", title = "屏风马破当头炮的三种变化",
                    subtitle = "大师级教程", source = "华夏棋苑",
                    action = new BlockActionData { type = "open_article", target = "article_tip" }
                },
                "lifestyle_card" => new BlockData
                {
                    type = "lifestyle_card", title = "棋子里的东方美学",
                    subtitle = "从材质到工艺", source = "文化频道",
                    action = new BlockActionData { type = "open_article", target = "article_lifestyle" }
                },
                "spacer" => new BlockData { type = "spacer", height = 16f },
                "divider" => new BlockData { type = "divider" },
                "paragraph" => new BlockData
                {
                    type = "paragraph",
                    text = "中国象棋源远流长，是中国传统文化的瑰宝。棋盘由九条纵线和十条横线交叉组成，共九十个交叉点。棋子共有三十二枚，红黑各十六枚。"
                },
                "image" => new BlockData
                {
                    type = "image",
                    media = new NewsFramework.Data.Media.MediaAssetData { url = "mock://chess_illustration", width = 800, height = 400 },
                    aspectRatio = 2f
                },
                "video" => new BlockData
                {
                    type = "video", title = "象棋入门教学：基本杀法", durationSeconds = 360f,
                    posterUrl = "mock://video_poster"
                },
                "replay_preview" => new BlockData
                {
                    type = "replay_preview", title = "经典对局回顾", boardTitle = "1982年全国赛决赛",
                    subtitle = "胡荣华 vs 杨官璘", text = "42回合 · 红方胜",
                    action = new BlockActionData { type = "open_replay", target = "replay_001" }
                },
                "replay" => new BlockData
                {
                    type = "replay", title = "经典对局回放",
                    replay = ReplayMockData.CreateArticlePreview()
                },
                "article_header" => new BlockData
                {
                    type = "article_header", title = "2026年全国象棋锦标赛精彩回顾",
                    subtitle = "32位选手齐聚杭州，鏖战七日", source = "华夏棋苑", time = "2026-05-20"
                },
                _ => new BlockData { type = type, title = $"[{type}] 占位预览" }
            };
        }

        // ──── Feature Sections ────

        private void BuildFeaturePreview(RectTransform parent)
        {
            var sectionData = CreateMockSectionData(selectedItem);
            if (sectionData == null) return;

            var registry = FeatureSectionRegistry.CreateDefault();
            var view = registry.Create(sectionData, parent);
            view.Bind(sectionData, _ => Debug.Log($"Section action: {_.type} -> {_.target}"));
        }

        private static FeatureSectionData CreateMockSectionData(string type)
        {
            // Use the existing mock page data and pick the first matching section
            FeaturePageData mockPage;
            switch (type)
            {
                case "section_header":
                case "quick_action_grid":
                case "feature_action_card":
                case "recent_match_list":
                    mockPage = MatchMockData.Create();
                    break;
                case "profile_header":
                case "stats_row":
                case "achievement_strip":
                case "rank_list":
                case "settings_list":
                case "about_footer":
                    mockPage = ProfileMockData.Create();
                    break;
                case "empty_state":
                    mockPage = FeatureMockData.CreateDataPage();
                    break;
                default:
                    return new FeatureSectionData { type = type };
            }

            return mockPage.sections?.FirstOrDefault(s => s.type == type)
                   ?? new FeatureSectionData { type = type };
        }

        // ──── Modals ────

        private void BuildModalPreview(RectTransform parent)
        {
            switch (selectedItem)
            {
                case "ModalOverlay (Normal)":
                    ModalOverlay.Show(new ModalOverlay.Config
                    {
                        title = "提示",
                        body = "这是一条普通提示信息。",
                        buttons = new[]
                        {
                            new ModalOverlay.ButtonDef("知道了", () => Debug.Log("确认")),
                            new ModalOverlay.ButtonDef("取消", () => Debug.Log("取消"))
                        }
                    });
                    break;

                case "ModalOverlay (Destructive)":
                    ModalOverlay.Show(new ModalOverlay.Config
                    {
                        title = "确定要认输吗？",
                        body = "这局棋会被记录为\"负\"",
                        buttons = new[]
                        {
                            new ModalOverlay.ButtonDef("认输", () => Debug.Log("已认输"), ModalButtonStyle.Destructive),
                            new ModalOverlay.ButtonDef("取消", () => Debug.Log("取消"))
                        }
                    });
                    break;

                case "ModalOverlay (Warning)":
                    ModalOverlay.Show(new ModalOverlay.Config
                    {
                        title = "确定要悔棋吗？",
                        body = "你将撤回上一步走棋",
                        buttons = new[]
                        {
                            new ModalOverlay.ButtonDef("悔棋", () => Debug.Log("已悔棋"), ModalButtonStyle.Warning),
                            new ModalOverlay.ButtonDef("取消", () => Debug.Log("取消"))
                        }
                    });
                    break;

                case "GameResultModal (Result)":
                    GameResultModal.Show(new GameResultModal.ResultConfig
                    {
                        winnerLabel = "红方胜",
                        redPlayer = "张三",
                        blackPlayer = "李四",
                        totalRounds = 42,
                        timeUsed = "12:35",
                        onGenerateReport = () => Debug.Log("生成AI战报"),
                        onRematch = () => Debug.Log("再来一局"),
                        onGoHome = () => Debug.Log("返回首页")
                    });
                    break;

                case "GameResultModal (Report)":
                {
                    GameObject overlay = null;
                    overlay = GameResultModal.Show(new GameResultModal.ResultConfig
                    {
                        winnerLabel = "红方胜",
                        redPlayer = "张三",
                        blackPlayer = "李四",
                        totalRounds = 42,
                        timeUsed = "12:35",
                        onGenerateReport = () =>
                        {
                            GameResultModal.ShowReport(overlay, new GameResultModal.ReportConfig
                            {
                                reportTitle = "张三15回合屏风马绝杀李四",
                                reportSubtitle = "屏风马破当头炮 · 午间挑战赛",
                                onShareWechat = () => Debug.Log("分享到微信"),
                                onRematch = () => Debug.Log("再来一局"),
                                onGoHome = () => Debug.Log("返回首页")
                            });
                        },
                        onRematch = () => Debug.Log("再来一局"),
                        onGoHome = () => Debug.Log("返回首页")
                    });
                    break;
                }
            }
        }
    }
}
