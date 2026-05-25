using System;
using UnityEngine;

namespace NewsFramework.GameRuntime
{
    public sealed class GameRoomSurfaceRenderer : IGameSurfaceRenderer
    {
        private const string DefaultPrefabKey = "Prefabs/GameSurface/GameRoomSurface";

        private RectTransform parent;
        private GameRoomSurfaceView surfaceView;
        private GameObject surfaceObject;
        private Action<GameInputAction> onInput;
        private GameResourceManifest resources;

        public void Mount(RectTransform mountParent, GameResourceManifest resources)
        {
            parent = mountParent;
            this.resources = resources;
        }

        public void Render(GameViewState state, Action<GameInputAction> inputHandler)
        {
            if (parent == null || state == null)
            {
                return;
            }

            onInput = inputHandler;
            if (surfaceView == null)
            {
                surfaceView = CreateSurfaceView();
            }

            surfaceView?.Bind(state, HandleInput);
        }

        public void SetSessionState(GameSessionState state)
        {
        }

        public void Unmount()
        {
            if (surfaceView != null)
            {
                surfaceView.Release();
            }

            DestroySurfaceObject();

            surfaceView = null;
            parent = null;
            onInput = null;
            resources = null;
        }

        private GameRoomSurfaceView CreateSurfaceView()
        {
            var prefabKey = ResolvePrefabKey();
            var prefab = Resources.Load<GameObject>(prefabKey);
            if (prefab != null)
            {
                surfaceObject = UnityEngine.Object.Instantiate(prefab, parent, false);
                surfaceObject.name = "GameRoomSurfacePrefab";
                var surfaceRect = surfaceObject.GetComponent<RectTransform>();
                if (surfaceRect != null)
                {
                    Stretch(surfaceRect);
                }

                var prefabView = surfaceObject.GetComponent<GameRoomSurfaceView>();
                if (prefabView != null)
                {
                    return prefabView;
                }

                Debug.LogWarning("[GameRuntime] Game surface prefab missing GameRoomSurfaceView prefabKey=" + prefabKey);
                DestroySurfaceObject();
            }
            else
            {
                Debug.LogWarning("[GameRuntime] Game surface prefab missing prefabKey=" + prefabKey);
            }

            surfaceObject = new GameObject("GameRoomSurfaceFallback", typeof(RectTransform));
            surfaceObject.transform.SetParent(parent, false);
            Stretch(surfaceObject.GetComponent<RectTransform>());
            return surfaceObject.AddComponent<GameRoomSurfaceView>();
        }

        private void DestroySurfaceObject()
        {
            if (surfaceObject == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                UnityEngine.Object.Destroy(surfaceObject);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(surfaceObject);
            }

            surfaceObject = null;
        }

        private string ResolvePrefabKey()
        {
            if (resources != null && resources.prefabKeys != null)
            {
                for (var i = 0; i < resources.prefabKeys.Count; i++)
                {
                    var prefabRef = resources.prefabKeys[i];
                    if (prefabRef != null && !string.IsNullOrWhiteSpace(prefabRef.key) &&
                        prefabRef.key.Contains("GameSurface"))
                    {
                        return prefabRef.key;
                    }
                }
            }

            return DefaultPrefabKey;
        }

        private void HandleInput(GameInputAction action)
        {
            onInput?.Invoke(action);
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
