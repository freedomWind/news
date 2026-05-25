using System;
using NewsFramework.Data.GameRoom;
using NewsFramework.UI.GameRoom;
using UnityEngine;

namespace NewsFramework.GameRuntime
{
    public sealed class GameRoomSurfaceView : MonoBehaviour
    {
        private GameRoomPage page;
        private Action<GameInputAction> onInput;

        public void Bind(GameViewState state, Action<GameInputAction> inputHandler)
        {
            onInput = inputHandler;
            if (state == null)
            {
                return;
            }

            if (page != null)
            {
                return;
            }

            var host = GetComponent<RectTransform>();
            if (host == null)
            {
                host = gameObject.AddComponent<RectTransform>();
            }

            page = gameObject.AddComponent<GameRoomPage>();
            page.Build(host, state.roomData ?? new GameRoomData(), HandleBack, HandleRoomAction);
        }

        public void Release()
        {
            page = null;
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
