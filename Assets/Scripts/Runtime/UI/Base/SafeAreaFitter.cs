using UnityEngine;

namespace NewsFramework.UI.Base
{
    [ExecuteAlways]
    public sealed class SafeAreaFitter : MonoBehaviour
    {
        [SerializeField] private bool applyLeft = true;
        [SerializeField] private bool applyTop = true;
        [SerializeField] private bool applyRight = true;
        [SerializeField] private bool applyBottom = true;

        private RectTransform rectTransform;
        private Rect lastSafeArea;
        private Vector2Int lastScreenSize;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            Apply();
        }

        private void OnEnable()
        {
            Apply();
        }

        private void Update()
        {
            if (lastSafeArea != Screen.safeArea || lastScreenSize.x != Screen.width || lastScreenSize.y != Screen.height)
            {
                Apply();
            }
        }

        private void Apply()
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }

            if (rectTransform == null || Screen.width <= 0 || Screen.height <= 0)
            {
                return;
            }

            var safe = Screen.safeArea;
            var min = safe.position;
            var max = safe.position + safe.size;

            min.x = applyLeft ? min.x / Screen.width : 0f;
            min.y = applyBottom ? min.y / Screen.height : 0f;
            max.x = applyRight ? max.x / Screen.width : 1f;
            max.y = applyTop ? max.y / Screen.height : 1f;

            rectTransform.anchorMin = min;
            rectTransform.anchorMax = max;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            lastSafeArea = safe;
            lastScreenSize = new Vector2Int(Screen.width, Screen.height);
        }
    }
}
