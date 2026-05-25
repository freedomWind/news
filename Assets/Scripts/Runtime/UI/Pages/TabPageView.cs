using System;
using NewsFramework.UI.Base;
using NewsFramework.UI.Theme;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.Pages
{
    public sealed class TabPageView
    {
        private readonly CanvasGroup canvasGroup;

        private TabPageView(
            string pageId,
            RectTransform root,
            ScrollRect scrollRect,
            RectTransform contentRoot,
            CanvasGroup group)
        {
            PageId = pageId;
            Root = root;
            ScrollRect = scrollRect;
            ContentRoot = contentRoot;
            canvasGroup = group;
        }

        public string PageId { get; }
        public RectTransform Root { get; }
        public ScrollRect ScrollRect { get; }
        public RectTransform ContentRoot { get; }
        public Action OnEnter { get; set; }
        public Action OnExit { get; set; }

        public static TabPageView Create(string pageId, Transform parent)
        {
            var root = AppUIFactory.CreateRect("TabPage_" + pageId, parent);
            AppUIFactory.Stretch(root);
            root.anchoredPosition = Vector2.zero;

            var group = root.gameObject.AddComponent<CanvasGroup>();

            var scrollRect = root.gameObject.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Elastic;
            scrollRect.scrollSensitivity = 28f;

            var viewport = AppUIFactory.CreateImage("Viewport", root, AppTheme.PageBackground);
            AppUIFactory.Stretch(viewport.rectTransform);
            var mask = viewport.gameObject.AddComponent<Mask>();
            mask.showMaskGraphic = false;
            scrollRect.viewport = viewport.rectTransform;

            var contentRoot = AppUIFactory.CreateRect("Content", viewport.transform);
            contentRoot.anchorMin = new Vector2(0f, 1f);
            contentRoot.anchorMax = Vector2.one;
            contentRoot.pivot = new Vector2(0.5f, 1f);
            contentRoot.anchoredPosition = Vector2.zero;
            contentRoot.sizeDelta = Vector2.zero;
            scrollRect.content = contentRoot;

            AppUIFactory.AddVerticalLayout(
                contentRoot.gameObject,
                14f,
                new RectOffset((int)AppTheme.PagePadding, (int)AppTheme.PagePadding, 16, 24));

            var fitter = contentRoot.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

            var view = new TabPageView(pageId, root, scrollRect, contentRoot, group);
            view.SetVisible(false);
            return view;
        }

        public void NotifyEnter()
        {
            OnEnter?.Invoke();
        }

        public void NotifyExit()
        {
            OnExit?.Invoke();
        }

        public void SetVisible(bool visible)
        {
            SetAlpha(visible ? 1f : 0f);
            SetInteraction(visible);
        }

        public void SetInteraction(bool enabled)
        {
            canvasGroup.interactable = enabled;
            canvasGroup.blocksRaycasts = enabled;
        }

        public void SetAlpha(float alpha)
        {
            canvasGroup.alpha = Mathf.Clamp01(alpha);
        }

        public void SetPageOffset(float x)
        {
            Root.anchoredPosition = new Vector2(x, Root.anchoredPosition.y);
        }

        public void ResetPageOffset()
        {
            SetPageOffset(0f);
        }

        public void ResetScrollPosition()
        {
            if (ScrollRect == null || ContentRoot == null)
            {
                return;
            }

            ContentRoot.anchoredPosition = Vector2.zero;
            ScrollRect.verticalNormalizedPosition = 1f;
        }
    }
}
