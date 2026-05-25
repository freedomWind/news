using System;

namespace NewsFramework.GameRuntime
{
    public sealed class DefaultGameSession : IGameSession
    {
        private readonly IGameRuntime runtime;
        private GameResourceManifest resources;
        private bool disposed;

        public DefaultGameSession(IGameRuntime gameRuntime)
        {
            runtime = gameRuntime ?? throw new ArgumentNullException(nameof(gameRuntime));
            SessionId = Guid.NewGuid().ToString("N");
            State = GameSessionState.Empty;
        }

        public string SessionId { get; }
        public GameSessionState State { get; private set; }
        public GameLaunchRequest LaunchRequest { get; private set; }
        public GameViewState CurrentViewState { get; private set; }

        public bool Prepare(GameLaunchRequest request)
        {
            LaunchRequest = request;
            SetState(GameSessionState.Preparing, "prepare");

            var error = Validate(request);
            if (error != null)
            {
                CurrentViewState = new GameViewState
                {
                    sessionId = SessionId,
                    gameId = request != null ? request.gameId : string.Empty,
                    mode = request != null ? request.mode : string.Empty,
                    phase = GameSessionState.Error.ToString(),
                    error = error
                };
                SetState(GameSessionState.Error, error.code);
                return false;
            }

            SetState(GameSessionState.LoadingAssets, "resolve_resources");
            resources = request.resourcePolicy ?? GameResourceManifest.CreateDefault(request);
            GameRuntimeLog.Fallback(SessionId, request.gameId, request.mode, resources);

            CurrentViewState = runtime.CreateInitialState(request, SessionId, resources);
            ApplyStateToView();
            SetState(GameSessionState.Ready, "first_view_state_ready");
            return true;
        }

        public void Start()
        {
            if (State != GameSessionState.Ready && State != GameSessionState.Paused)
            {
                return;
            }

            SetState(GameSessionState.Playing, "start");
        }

        public void Pause()
        {
            if (State == GameSessionState.Playing)
            {
                SetState(GameSessionState.Paused, "pause");
            }
        }

        public void Resume()
        {
            if (State == GameSessionState.Paused)
            {
                SetState(GameSessionState.Playing, "resume");
            }
        }

        public GameInputResult HandleInput(GameInputAction action)
        {
            if (action == null)
            {
                var result = GameInputResult.Rejected("input_null");
                LogInput(action, result);
                return result;
            }

            if (State != GameSessionState.Playing && action.type != GameInputTypes.Exit)
            {
                var result = GameInputResult.Rejected("session_not_playing");
                LogInput(action, result);
                return result;
            }

            if (action.type == GameInputTypes.Exit)
            {
                var exitResult = GameInputResult.WithResult(CurrentViewState, Exit("user_exit"), "exit_requested");
                LogInput(action, exitResult);
                return exitResult;
            }

            var inputResult = runtime.HandleInput(action);

            if (inputResult.viewState != null)
            {
                CurrentViewState = inputResult.viewState;
                ApplyStateToView();
            }

            if (inputResult.result != null)
            {
                SetState(GameSessionState.Result, inputResult.result.reason);
                SetState(GameSessionState.Exiting, inputResult.result.reason);
                GameRuntimeLog.Result(inputResult.result);
            }

            LogInput(action, inputResult);
            return inputResult;
        }

        public GameResultData Exit(string reason)
        {
            var result = runtime.CreateResult(SessionId, string.IsNullOrEmpty(reason) ? "exit" : reason);
            SetState(GameSessionState.Result, result.reason);
            SetState(GameSessionState.Exiting, result.reason);
            GameRuntimeLog.Result(result);
            return result;
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            runtime.Dispose();
            SetState(GameSessionState.Disposed, "dispose");
        }

        private void SetState(GameSessionState state, string detail)
        {
            State = state;
            ApplyStateToView();
            GameRuntimeLog.State(
                SessionId,
                LaunchRequest != null ? LaunchRequest.gameId : string.Empty,
                LaunchRequest != null ? LaunchRequest.mode : string.Empty,
                State,
                detail);
        }

        private void ApplyStateToView()
        {
            if (CurrentViewState == null)
            {
                return;
            }

            CurrentViewState.sessionId = SessionId;
            CurrentViewState.phase = State.ToString();
            CurrentViewState.resources = resources;
        }

        private void LogInput(GameInputAction action, GameInputResult result)
        {
            GameRuntimeLog.Input(
                SessionId,
                LaunchRequest != null ? LaunchRequest.gameId : string.Empty,
                LaunchRequest != null ? LaunchRequest.mode : string.Empty,
                action,
                result != null && result.accepted,
                result != null ? result.reason : "null_result");
        }

        private static GameErrorData Validate(GameLaunchRequest request)
        {
            if (request == null)
            {
                return Error("invalid_launch_request", "Launch request is empty.");
            }

            if (string.IsNullOrEmpty(request.gameId))
            {
                return Error("invalid_launch_request", "gameId is required.");
            }

            if (string.IsNullOrEmpty(request.mode))
            {
                return Error("unsupported_mode", "mode is required.");
            }

            if (request.mode == GameRuntimeModes.Spectator && string.IsNullOrEmpty(request.roomId))
            {
                return Error("invalid_launch_request", "roomId is required for spectator mode.");
            }

            if (request.mode == GameRuntimeModes.Replay && string.IsNullOrEmpty(request.replayId))
            {
                return Error("invalid_launch_request", "replayId is required for replay mode.");
            }

            return null;
        }

        private static GameErrorData Error(string code, string message)
        {
            return new GameErrorData
            {
                code = code,
                severity = "fatal",
                message = message,
                recoverable = false,
                source = "DefaultGameSession"
            };
        }
    }
}
