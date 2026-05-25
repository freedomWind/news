using System.Collections;
using System.Collections.Generic;
using NewsFramework.Data.Blocks;
using NewsFramework.Data.Features;
using NewsFramework.Data.Mock;
using NewsFramework.Services.Content;
using NewsFramework.Services.Content.Feed;
using NewsFramework.Services.Content.Runtime;
using NewsFramework.Services.Media;

using NewsFramework.UI.Base;
using NewsFramework.UI.Blocks;
using NewsFramework.UI.Features;
using NewsFramework.UI.GameRoom;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.Pages
{
    public sealed class HomePage : MonoBehaviour
    {

        private RectTransform safeRoot;
        private RectTransform articleLayer;
        private RectTransform gameRoomLayer;
        private TabPageHost pageHost;
        private TabPageView homePageView;
        private TabPageView dataPageView;
        private TabPageView matchPageView;
        private TabPageView profilePageView;
        private BlockRenderer blockRenderer;
        private FeaturePageRenderer dataRenderer;
        private FeaturePageRenderer matchRenderer;
        private FeaturePageRenderer profileRenderer;
        private TextMeshProUGUI titleLabel;
        private IContentService contentService;
        private IFeedPager feedPager;
        private ContentRuntimeServices runtimeServices;
        private BlockRenderContext blockRenderContext;
        private FeaturePageData dataPage;
        private FeaturePageData matchPage;
        private FeaturePageData profilePage;
        private string homeTitle = "首页";
        private bool homeTabActive;
        private bool homeFeedDirty = true;
        private bool homeFeedRendered;
        private bool pendingHomeReset;
        private Coroutine homeRefreshRoutine;
        private readonly List<BlockData> pendingHomeBlocks = new List<BlockData>();
        private readonly List<TextMeshProUGUI> tabLabels = new List<TextMeshProUGUI>();

        public void Build()
        {
            Build(new ContentRuntimeConfig(), new MediaServerConfig());
        }

        public void Build(ContentRuntimeConfig runtimeConfig)
        {
            Build(runtimeConfig, new MediaServerConfig());
        }

        public void Build(ContentRuntimeConfig runtimeConfig, MediaServerConfig mediaServerConfig)
        {
            runtimeServices?.Dispose();
            runtimeServices = ContentRuntimeServiceFactory.Create(runtimeConfig);
            contentService = runtimeServices.ContentService;
            feedPager = runtimeServices.FeedPager;
            blockRenderContext = BlockRenderContext.CreateDefault(mediaServerConfig);

            AppUIFactory.EnsureEventSystem();

            var canvas = AppUIFactory.CreateOverlayCanvas();
            safeRoot = AppUIFactory.CreateRect("SafeArea", canvas.transform);
            AppUIFactory.Stretch(safeRoot);
            safeRoot.gameObject.AddComponent<SafeAreaFitter>();

            var background = AppUIFactory.CreateImage("PageBackground", safeRoot, AppTheme.PageBackground);
            AppUIFactory.Stretch(background.rectTransform);

            BuildTopArea(safeRoot);
            BuildBottomTabs(safeRoot);
            BuildContentArea(safeRoot);

            blockRenderer = gameObject.AddComponent<BlockRenderer>();
            blockRenderer.Initialize(homePageView.ContentRoot, BlockRegistry.CreateDefault(), HandleAction, blockRenderContext);

            dataRenderer = gameObject.AddComponent<FeaturePageRenderer>();
            dataRenderer.Initialize(dataPageView.ContentRoot, FeatureSectionRegistry.CreateDefault(), HandleAction);

            matchRenderer = gameObject.AddComponent<FeaturePageRenderer>();
            matchRenderer.Initialize(matchPageView.ContentRoot, FeatureSectionRegistry.CreateDefault(), HandleAction);

            profileRenderer = gameObject.AddComponent<FeaturePageRenderer>();
            profileRenderer.Initialize(profilePageView.ContentRoot, FeatureSectionRegistry.CreateDefault(), HandleAction);

            dataPage = FeatureMockData.CreateDataPage();
            matchPage = MatchMockData.Create();
            profilePage = ProfileMockData.Create();

            dataRenderer.Render(dataPage);
            matchRenderer.Render(matchPage);
            profileRenderer.Render(profilePage);

            SelectTab(0);
            titleLabel.text = "加载中";
            feedPager.Start((runtimeConfig ?? new ContentRuntimeConfig()).CreateFeedPagerConfig(), HandleFeedPageLoaded);
        }

        private void OnDestroy()
        {
            runtimeServices?.Dispose();
            runtimeServices = null;
        }

        private void BuildTopArea(RectTransform parent)
        {
            var topArea = AppUIFactory.CreateImage("TopArea", parent, AppTheme.Surface);
            AppUIFactory.AnchorTop(topArea.rectTransform, 100f);

            var status = AppUIFactory.CreateImage("StatusBar", topArea.transform, AppTheme.Surface);
            status.rectTransform.anchorMin = new Vector2(0f, 1f);
            status.rectTransform.anchorMax = Vector2.one;
            status.rectTransform.pivot = new Vector2(0.5f, 1f);
            status.rectTransform.offsetMin = new Vector2(0f, -42f);
            status.rectTransform.offsetMax = Vector2.zero;

            var time = AppUIFactory.CreateText(
                "Time",
                status.transform,
                "11:36",
                13f,
                AppTheme.PrimaryText,
                FontStyles.Bold);
            time.rectTransform.anchorMin = new Vector2(0f, 0f);
            time.rectTransform.anchorMax = new Vector2(0f, 1f);
            time.rectTransform.offsetMin = new Vector2(20f, 0f);
            time.rectTransform.offsetMax = new Vector2(96f, 0f);
            time.alignment = TextAlignmentOptions.MidlineLeft;

            var systemIcons = AppUIFactory.CreateText(
                "SystemIcons",
                status.transform,
                "▴  ≋  ▱",
                12f,
                AppTheme.PrimaryText,
                FontStyles.Normal,
                TextAlignmentOptions.MidlineRight);
            systemIcons.rectTransform.anchorMin = new Vector2(1f, 0f);
            systemIcons.rectTransform.anchorMax = Vector2.one;
            systemIcons.rectTransform.offsetMin = new Vector2(-104f, 0f);
            systemIcons.rectTransform.offsetMax = new Vector2(-20f, 0f);

            var nav = AppUIFactory.CreateRect("NavigationBar", topArea.transform);
            nav.anchorMin = Vector2.zero;
            nav.anchorMax = Vector2.one;
            nav.offsetMin = Vector2.zero;
            nav.offsetMax = new Vector2(0f, -42f);

            var line = AppUIFactory.CreateImage("BottomLine", topArea.transform, AppTheme.Hairline);
            line.rectTransform.anchorMin = Vector2.zero;
            line.rectTransform.anchorMax = new Vector2(1f, 0f);
            line.rectTransform.pivot = new Vector2(0.5f, 0f);
            line.rectTransform.offsetMin = Vector2.zero;
            line.rectTransform.offsetMax = new Vector2(0f, 1f);

            titleLabel = AppUIFactory.CreateText(
                "Title",
                nav,
                string.Empty,
                24f,
                AppTheme.PrimaryText,
                FontStyles.Bold,
                TextAlignmentOptions.MidlineLeft);
            titleLabel.rectTransform.anchorMin = Vector2.zero;
            titleLabel.rectTransform.anchorMax = Vector2.one;
            titleLabel.rectTransform.offsetMin = new Vector2(20f, 0f);
            titleLabel.rectTransform.offsetMax = new Vector2(-72f, 0f);

            var search = AppUIFactory.CreateButton("SearchButton", nav, AppTheme.Surface, () => Debug.Log("Search tapped"));
            search.GetComponent<RectTransform>().anchorMin = new Vector2(1f, 0f);
            search.GetComponent<RectTransform>().anchorMax = Vector2.one;
            search.GetComponent<RectTransform>().offsetMin = new Vector2(-64f, 0f);
            search.GetComponent<RectTransform>().offsetMax = Vector2.zero;

            var searchIcon = AppUIFactory.CreateText(
                "Icon",
                search.transform,
                "⌕",
                28f,
                AppTheme.PrimaryText,
                FontStyles.Normal,
                TextAlignmentOptions.Center);
            AppUIFactory.Stretch(searchIcon.rectTransform);
        }

        private void BuildBottomTabs(RectTransform parent)
        {
            var bottom = AppUIFactory.CreateImage("BottomTabBar", parent, AppTheme.Surface);
            AppUIFactory.AnchorBottom(bottom.rectTransform, AppTheme.BottomBarHeight);

            var line = AppUIFactory.CreateImage("TopLine", bottom.transform, AppTheme.Hairline);
            line.rectTransform.anchorMin = new Vector2(0f, 1f);
            line.rectTransform.anchorMax = Vector2.one;
            line.rectTransform.pivot = new Vector2(0.5f, 1f);
            line.rectTransform.offsetMin = new Vector2(0f, -1f);
            line.rectTransform.offsetMax = Vector2.zero;

            var tabsRoot = AppUIFactory.CreateRect("Tabs", bottom.transform);
            AppUIFactory.Stretch(tabsRoot);
            AppUIFactory.AddHorizontalLayout(tabsRoot.gameObject, 0f, new RectOffset(0, 0, 4, 0), TextAnchor.MiddleCenter);

            tabLabels.Clear();
            CreateTab(tabsRoot, "⌂", "首页", 0);
            CreateTab(tabsRoot, "▥", "数据", 1);
            CreateTab(tabsRoot, "↯", "对局", 2);
            CreateTab(tabsRoot, "♙", "我的", 3);
        }

        private void CreateTab(Transform parent, string icon, string label, int index)
        {
            var button = AppUIFactory.CreateButton("Tab_" + label, parent, AppTheme.Surface, () => SelectTab(index));
            AppUIFactory.AddLayoutElement(button.gameObject, AppTheme.BottomBarHeight - 6f);
            var layout = button.GetComponent<LayoutElement>();
            layout.flexibleWidth = 1f;

            var text = AppUIFactory.CreateText(
                "Label",
                button.transform,
                icon + "\n" + label,
                12f,
                AppTheme.TabInactive,
                FontStyles.Normal,
                TextAlignmentOptions.Center);
            text.lineSpacing = -8f;
            AppUIFactory.Stretch(text.rectTransform);
            tabLabels.Add(text);
        }

        private void SelectTab(int index)
        {
            if (index < 0 || index >= tabLabels.Count)
            {
                return;
            }

            if (pageHost == null)
            {
                return;
            }

            for (var i = 0; i < tabLabels.Count; i++)
            {
                var selected = i == index;
                tabLabels[i].color = selected ? AppTheme.Accent : AppTheme.TabInactive;
                tabLabels[i].fontStyle = selected ? FontStyles.Bold : FontStyles.Normal;
            }

            Debug.Log("Tab selected: " + index);
            titleLabel.text = ResolveTabTitle(index);
            var animate = pageHost.ActiveIndex >= 0;
            pageHost.Show(index, animate);
        }

        private void BuildContentArea(RectTransform parent)
        {
            var hostRoot = AppUIFactory.CreateRect("TabPageHost", parent);
            hostRoot.anchorMin = Vector2.zero;
            hostRoot.anchorMax = Vector2.one;
            hostRoot.offsetMin = new Vector2(0f, AppTheme.BottomBarHeight);
            hostRoot.offsetMax = new Vector2(0f, -100f);
            hostRoot.gameObject.AddComponent<RectMask2D>();

            pageHost = gameObject.AddComponent<TabPageHost>();
            pageHost.Initialize(hostRoot);

            homePageView = pageHost.CreatePage("Home");
            dataPageView = pageHost.CreatePage("Data");
            matchPageView = pageHost.CreatePage("Match");
            profilePageView = pageHost.CreatePage("Profile");

            homePageView.ScrollRect.onValueChanged.AddListener(_ => EnsureFeedBuffer());
            homePageView.OnEnter = EnterHomeTab;
            homePageView.OnExit = ExitHomeTab;
        }

        private void HandleAction(BlockActionData action)
        {
            if (action == null)
            {
                return;
            }

            if (action.type == "open_article")
            {
                ShowArticle(action.GetParameter("articleId"));
                return;
            }

            if (action.type == "open_match" && action.target == "match_live")
            {
                ShowSpectatorRoom(action.GetParameter("matchId"));
                return;
            }

            if (action.type == "open_ai_practice" || action.type == "start_ranked_match")
            {
                ShowPlayerRoom();
                return;
            }

            Debug.Log($"Unhandled action: {action?.type} -> {action?.target}");
        }

        private void ShowSpectatorRoom(string matchId)
        {
            ShowGameRoom(GameRoomMockData.CreateSpectatorRoom(matchId));
        }

        private void ShowPlayerRoom()
        {
            ShowGameRoom(GameRoomMockData.CreatePlayerRoom());
        }

        private void ShowGameRoom(NewsFramework.Data.GameRoom.GameRoomData roomData)
        {
            CloseGameRoom();

            gameRoomLayer = AppUIFactory.CreateRect("GameRoomLayer", safeRoot);
            AppUIFactory.Stretch(gameRoomLayer);
            gameRoomLayer.SetAsLastSibling();

            var page = gameRoomLayer.gameObject.AddComponent<GameRoomPage>();
            page.Build(gameRoomLayer, roomData, CloseGameRoom);
        }

        private void CloseGameRoom()
        {
            if (gameRoomLayer == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Destroy(gameRoomLayer.gameObject);
            }
            else
            {
                DestroyImmediate(gameRoomLayer.gameObject);
            }

            gameRoomLayer = null;
        }

        private void ShowArticle(string articleId)
        {
            if (string.IsNullOrEmpty(articleId))
            {
                Debug.LogWarning("Article id is empty.");
                return;
            }

            CloseArticle();

            articleLayer = AppUIFactory.CreateRect("ArticleLayer", safeRoot);
            AppUIFactory.Stretch(articleLayer);
            articleLayer.SetAsLastSibling();

            var page = articleLayer.gameObject.AddComponent<ArticlePage>();
            page.Build(
                articleLayer,
                contentService,
                articleId,
                CloseArticle,
                HandleAction,
                string.Empty,
                blockRenderContext,
                runtimeServices != null ? runtimeServices.ArticleEngagementService : null);
        }

        private void CloseArticle()
        {
            if (articleLayer == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Destroy(articleLayer.gameObject);
            }
            else
            {
                DestroyImmediate(articleLayer.gameObject);
            }

            articleLayer = null;
        }

        private void HandleFeedPageLoaded(FeedPageResult result)
        {
            if (result == null || !result.success)
            {
                Debug.LogWarning("Home feed load failed: " + (result != null ? result.error : "empty result"));
                return;
            }

            homeTitle = string.IsNullOrEmpty(result.title) ? "首页" : result.title;
            if (!homeTabActive)
            {
                QueueInactiveHomeFeedResult(result);
                return;
            }

            titleLabel.text = homeTitle;
            pendingHomeBlocks.Clear();
            pendingHomeReset = false;

            if (result.reset || !homeFeedRendered)
            {
                RenderHomeFeedFromCache();
            }
            else
            {
                blockRenderer.Append(result.blocks);
                homeFeedRendered = true;
                homeFeedDirty = false;
            }

            Canvas.ForceUpdateCanvases();
            EnsureFeedBuffer();
        }

        private void EnsureFeedBuffer()
        {
            if (!homeTabActive || feedPager == null || homePageView == null)
            {
                return;
            }

            var contentScroll = homePageView.ScrollRect;
            var contentRoot = homePageView.ContentRoot;
            if (contentScroll == null || contentRoot == null)
            {
                return;
            }

            var viewportHeight = contentScroll.viewport != null ? contentScroll.viewport.rect.height : 0f;
            var contentHeight = contentRoot.rect.height;
            var scrollY = Mathf.Max(0f, contentRoot.anchoredPosition.y);
            var remainingPixels = Mathf.Max(0f, contentHeight - viewportHeight - scrollY);
            feedPager.EnsureAhead(remainingPixels, viewportHeight, HandleFeedPageLoaded);
        }

        private void EnterHomeTab()
        {
            homeTabActive = true;
            feedPager?.SetActive(true);
            var scheduledRefresh = false;

            if (titleLabel != null)
            {
                titleLabel.text = homeTitle;
            }

            if (homeFeedDirty || !homeFeedRendered)
            {
                if (homeFeedRendered)
                {
                    ScheduleHomeRefresh();
                    scheduledRefresh = true;
                }
                else
                {
                    RenderHomeFeedFromCache();
                }
            }

            if (scheduledRefresh)
            {
                return;
            }

            EnsureFeedBuffer();
        }

        private void ExitHomeTab()
        {
            homeTabActive = false;
            feedPager?.SetActive(false);
            StopHomeRefreshRoutine();
        }

        private void RenderHomeFeedFromCache()
        {
            if (blockRenderer == null)
            {
                return;
            }

            var loadedBlocks = feedPager != null ? feedPager.LoadedBlocks : null;
            if (loadedBlocks == null || loadedBlocks.Count == 0)
            {
                blockRenderer.Clear();
                homeFeedRendered = false;
                return;
            }

            blockRenderer.Render(new PageData
            {
                pageId = "home",
                pageType = "home",
                title = homeTitle,
                blocks = new List<BlockData>(loadedBlocks)
            });
            homeFeedRendered = true;
            homeFeedDirty = false;
            pendingHomeReset = false;
            pendingHomeBlocks.Clear();
        }

        private void QueueInactiveHomeFeedResult(FeedPageResult result)
        {
            homeFeedDirty = true;

            if (result == null)
            {
                return;
            }

            if (result.reset || !homeFeedRendered)
            {
                pendingHomeReset = true;
                pendingHomeBlocks.Clear();
                return;
            }

            if (result.blocks == null || result.blocks.Count == 0)
            {
                return;
            }

            pendingHomeBlocks.AddRange(result.blocks);
        }

        private void ScheduleHomeRefresh()
        {
            StopHomeRefreshRoutine();
            homeRefreshRoutine = StartCoroutine(RefreshHomeAfterTabAnimation());
        }

        private IEnumerator RefreshHomeAfterTabAnimation()
        {
            yield return new WaitForSecondsRealtime(0.22f);
            homeRefreshRoutine = null;

            if (!homeTabActive || blockRenderer == null)
            {
                yield break;
            }

            if (pendingHomeReset)
            {
                RenderHomeFeedFromCache();
            }
            else if (pendingHomeBlocks.Count > 0)
            {
                blockRenderer.Append(pendingHomeBlocks);
                pendingHomeBlocks.Clear();
                homeFeedDirty = false;
                homeFeedRendered = true;
            }
            else if (homeFeedDirty)
            {
                RenderHomeFeedFromCache();
            }

            Canvas.ForceUpdateCanvases();
            EnsureFeedBuffer();
        }

        private void StopHomeRefreshRoutine()
        {
            if (homeRefreshRoutine == null)
            {
                return;
            }

            StopCoroutine(homeRefreshRoutine);
            homeRefreshRoutine = null;
        }

        private string ResolveTabTitle(int index)
        {
            switch (index)
            {
                case 0:
                    return homeTitle;
                case 1:
                    return dataPage != null && !string.IsNullOrEmpty(dataPage.title) ? dataPage.title : "数据";
                case 2:
                    return matchPage != null && !string.IsNullOrEmpty(matchPage.title) ? matchPage.title : "对局";
                case 3:
                    return profilePage != null && !string.IsNullOrEmpty(profilePage.title) ? profilePage.title : "我的";
                default:
                    return string.Empty;
            }
        }

    }
}
