using System;
using UnityEngine;

namespace NewsFramework.GameRuntime
{
    public sealed class GameHostController : MonoBehaviour
    {
        private IGameSession session;
        private IGameSurfaceRenderer surfaceRenderer;
        private Action<GameResultData> onCompleted;
        private bool completed;

        public void Launch(RectTransform parent, GameLaunchRequest request, Action<GameResultData> completedHandler)
        {
            onCompleted = completedHandler;
            completed = false;

            session = new DefaultGameSession(new GameRoomRuntime());
            surfaceRenderer = new GameRoomSurfaceRenderer();

            var prepared = session.Prepare(request);
            if (!prepared)
            {
                Complete(session.Exit("prepare_failed"));
                return;
            }

            surfaceRenderer.Mount(parent, session.CurrentViewState.resources);
            surfaceRenderer.Render(session.CurrentViewState, HandleInput);
            session.Start();
            surfaceRenderer.SetSessionState(session.State);
            surfaceRenderer.Render(session.CurrentViewState, HandleInput);
        }

        public void RequestExit(string reason)
        {
            if (session == null || completed)
            {
                return;
            }

            Complete(session.Exit(reason));
        }

        private void HandleInput(GameInputAction action)
        {
            if (session == null || completed)
            {
                return;
            }

            var result = session.HandleInput(action);
            if (result != null && result.viewState != null)
            {
                surfaceRenderer.Render(result.viewState, HandleInput);
            }

            if (result != null && result.result != null)
            {
                Complete(result.result);
            }
        }

        private void Complete(GameResultData result)
        {
            if (completed)
            {
                return;
            }

            completed = true;
            surfaceRenderer?.SetSessionState(GameSessionState.Exiting);
            surfaceRenderer?.Unmount();
            session?.Dispose();
            onCompleted?.Invoke(result);
        }

        private void OnApplicationPause(bool pause)
        {
            if (session == null || completed)
            {
                return;
            }

            if (pause)
            {
                session.Pause();
            }
            else
            {
                session.Resume();
            }
        }

        private void OnDestroy()
        {
            surfaceRenderer?.Unmount();
            session?.Dispose();
            surfaceRenderer = null;
            session = null;
            onCompleted = null;
        }
    }
}
