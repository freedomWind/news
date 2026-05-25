using System;
using NewsFramework.Data.GameRoom;
using NewsFramework.UI.GameRoom;
using UnityEngine;

namespace NewsFramework.GameRuntime
{
    public sealed class GameRoomSurfaceRenderer : IGameSurfaceRenderer
    {
        private RectTransform parent;
        private GameRoomPage page;
        private Action<GameInputAction> onInput;

        public void Mount(RectTransform mountParent, GameResourceManifest resources)
        {
            parent = mountParent;
        }

        public void Render(GameViewState state, Action<GameInputAction> inputHandler)
        {
            if (parent == null || state == null)
            {
                return;
            }

            onInput = inputHandler;
            if (page != null)
            {
                return;
            }

            page = parent.gameObject.AddComponent<GameRoomPage>();
            page.Build(parent, state.roomData ?? new GameRoomData(), HandleBack, HandleRoomAction);
        }

        public void SetSessionState(GameSessionState state)
        {
        }

        public void Unmount()
        {
            page = null;
            parent = null;
            onInput = null;
        }

        private void HandleBack()
        {
            onInput?.Invoke(GameInputAction.Exit("game_room_back"));
        }

        private void HandleRoomAction(GameRoomActionData action)
        {
            onInput?.Invoke(GameInputAction.FromRoomAction(action));
        }
    }
}
