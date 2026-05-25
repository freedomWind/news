using System.Collections;
using System.Collections.Generic;
using NewsFramework.UI.Base;
using NewsFramework.UI.Theme;
using UnityEngine;

namespace NewsFramework.UI.Pages
{
    public sealed class TabPageHost : MonoBehaviour
    {
        [SerializeField] private RectTransform root;
        [SerializeField] private float transitionSeconds = 0.18f;

        private readonly List<TabPageView> pages = new List<TabPageView>();
        private Coroutine transitionRoutine;

        public int ActiveIndex { get; private set; } = -1;
        public bool IsTransitioning => transitionRoutine != null;

        public void Initialize(RectTransform hostRoot)
        {
            root = hostRoot;
        }

        public TabPageView CreatePage(string pageId)
        {
            if (root == null)
            {
                Debug.LogWarning("TabPageHost root is missing.");
                return null;
            }

            var page = TabPageView.Create(pageId, root);
            pages.Add(page);
            return page;
        }

        public bool Show(int index, bool animate)
        {
            if (index < 0 || index >= pages.Count)
            {
                return false;
            }

            if (ActiveIndex == index && !IsTransitioning)
            {
                return false;
            }

            CompleteInterruptedTransition();

            var previousIndex = ActiveIndex;
            var previous = previousIndex >= 0 && previousIndex < pages.Count ? pages[previousIndex] : null;
            var next = pages[index];

            previous?.NotifyExit();
            ActiveIndex = index;
            next.SetVisible(true);
            next.ResetPageOffset();
            next.SetAlpha(1f);
            next.NotifyEnter();

            HideInactivePages(previousIndex, index);

            if (previous == null || !animate)
            {
                previous?.SetVisible(false);
                next.SetInteraction(true);
                return true;
            }

            var direction = index > previousIndex ? 1f : -1f;
            var width = ResolveWidth();
            previous.SetVisible(true);
            previous.SetInteraction(false);
            next.SetInteraction(false);
            previous.SetPageOffset(0f);
            next.SetPageOffset(direction * width);
            transitionRoutine = StartCoroutine(AnimateTransition(previous, next, direction, width));
            return true;
        }

        private void HideInactivePages(int previousIndex, int nextIndex)
        {
            for (var i = 0; i < pages.Count; i++)
            {
                if (i == previousIndex || i == nextIndex)
                {
                    continue;
                }

                pages[i].SetVisible(false);
                pages[i].ResetPageOffset();
            }
        }

        private IEnumerator AnimateTransition(TabPageView previous, TabPageView next, float direction, float width)
        {
            var duration = Mathf.Max(0.01f, transitionSeconds);
            var elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var eased = EaseOutCubic(t);
                previous.SetPageOffset(Mathf.Lerp(0f, -direction * width, eased));
                next.SetPageOffset(Mathf.Lerp(direction * width, 0f, eased));
                previous.SetAlpha(Mathf.Lerp(1f, 0.96f, eased));
                next.SetAlpha(1f);
                yield return null;
            }

            previous.SetVisible(false);
            previous.ResetPageOffset();
            next.ResetPageOffset();
            next.SetAlpha(1f);
            next.SetInteraction(true);
            transitionRoutine = null;
        }

        private void CompleteInterruptedTransition()
        {
            if (transitionRoutine == null)
            {
                return;
            }

            StopCoroutine(transitionRoutine);
            transitionRoutine = null;

            for (var i = 0; i < pages.Count; i++)
            {
                var active = i == ActiveIndex;
                pages[i].ResetPageOffset();
                pages[i].SetVisible(active);
            }
        }

        private float ResolveWidth()
        {
            if (root != null && root.rect.width > 1f)
            {
                return root.rect.width;
            }

            return Mathf.Max(1f, AppTheme.ScreenWidth);
        }

        private static float EaseOutCubic(float value)
        {
            var inverse = 1f - Mathf.Clamp01(value);
            return 1f - inverse * inverse * inverse;
        }
    }
}
