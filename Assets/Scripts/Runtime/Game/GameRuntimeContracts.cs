using System;
using System.Collections.Generic;
using NewsFramework.Data.Blocks;
using NewsFramework.Data.GameRoom;
using UnityEngine;

namespace NewsFramework.GameRuntime
{
    public static class GameRuntimeModes
    {
        public const string Spectator = "spectator";
        public const string AiTraining = "ai_training";
        public const string Player = "player";
        public const string Replay = "replay";
    }

    public static class GameDataSources
    {
        public const string Mock = "mock";
        public const string LocalSimulation = "local_simulation";
        public const string RemoteRoom = "remote_room";
        public const string Replay = "replay";
    }

    public static class GameInputTypes
    {
        public const string Exit = "exit";
        public const string Resign = "resign";
        public const string RequestDraw = "request_draw";
        public const string Undo = "undo";
        public const string Share = "share";
        public const string SendComment = "send_comment";
        public const string Flower = "flower";
        public const string ReplayPlay = "replay_play";
        public const string OpenSettings = "open_settings";
    }

    public enum GameSessionState
    {
        Empty,
        Preparing,
        LoadingAssets,
        Ready,
        Playing,
        Paused,
        Result,
        Error,
        Exiting,
        Disposed
    }

    public sealed class GameFallbackData
    {
        public string type;
        public string target;
        public string message;

        public static GameFallbackData MinimalSurface(string message = "Using minimal game surface.")
        {
            return new GameFallbackData
            {
                type = "minimal_surface",
                target = string.Empty,
                message = message
            };
        }
    }

    public sealed class GameResourceRef
    {
        public string key;
        public bool required;
    }

    public sealed class GameResourceManifest
    {
        public string gameId;
        public string version = "1";
        public string mode;
        public List<GameResourceRef> prefabKeys = new List<GameResourceRef>();
        public List<GameResourceRef> assetBundleKeys = new List<GameResourceRef>();
        public int timeoutMs = 8000;
        public GameFallbackData fallback = GameFallbackData.MinimalSurface();

        public static GameResourceManifest CreateDefault(GameLaunchRequest request)
        {
            var manifest = new GameResourceManifest
            {
                gameId = request != null && !string.IsNullOrEmpty(request.gameId) ? request.gameId : "xiangqi",
                mode = request != null && !string.IsNullOrEmpty(request.mode) ? request.mode : GameRuntimeModes.Spectator,
                fallback = GameFallbackData.MinimalSurface("使用基础棋盘模式。")
            };
            manifest.prefabKeys.Add(new GameResourceRef { key = "Prefabs/GameSurface/GameRoomSurface", required = false });
            manifest.assetBundleKeys.Add(new GameResourceRef { key = "game_xiangqi_board_default", required = false });
            return manifest;
        }
    }

    public sealed class GameLaunchRequest
    {
        public string launchId;
        public string gameId = "xiangqi";
        public string mode = GameRuntimeModes.Spectator;
        public string roomId;
        public string replayId;
        public string sourcePageId = "home";
        public string sourceContentId;
        public string returnRoute = "home";
        public string initialState;
        public string initialStateFormat;
        public string dataSource = GameDataSources.Mock;
        public Dictionary<string, string> launchParams = new Dictionary<string, string>();
        public GameResourceManifest resourcePolicy;
        public Dictionary<string, string> tracking = new Dictionary<string, string>();
        public GameRoomData roomData;

        public string GetParameter(string key)
        {
            if (string.IsNullOrEmpty(key) || launchParams == null)
            {
                return string.Empty;
            }

            return launchParams.TryGetValue(key, out var value) ? value : string.Empty;
        }

        public static GameLaunchRequest Spectator(string roomId, GameRoomData roomData)
        {
            return Create(GameRuntimeModes.Spectator, roomId, string.Empty, GameDataSources.RemoteRoom, roomData);
        }

        public static GameLaunchRequest AiTraining(GameRoomData roomData)
        {
            return Create(GameRuntimeModes.AiTraining, roomData != null ? roomData.roomId : "ai_training_001", string.Empty, GameDataSources.LocalSimulation, roomData);
        }

        public static GameLaunchRequest Replay(string replayId, GameRoomData roomData)
        {
            return Create(GameRuntimeModes.Replay, roomData != null ? roomData.roomId : "replay_room_001", replayId, GameDataSources.Replay, roomData);
        }

        private static GameLaunchRequest Create(
            string mode,
            string roomId,
            string replayId,
            string dataSource,
            GameRoomData roomData)
        {
            return new GameLaunchRequest
            {
                launchId = Guid.NewGuid().ToString("N"),
                gameId = "xiangqi",
                mode = mode,
                roomId = roomId,
                replayId = replayId,
                dataSource = dataSource,
                returnRoute = "home",
                roomData = roomData
            };
        }
    }

    public sealed class GameInputDescriptor
    {
        public string actionId;
        public string type;
        public string label;
    }

    public sealed class GameViewState
    {
        public string sessionId;
        public string gameId;
        public string mode;
        public string phase;
        public int tick;
        public string version = "1";
        public GameRoomData roomData;
        public List<GameInputDescriptor> availableInputs = new List<GameInputDescriptor>();
        public GameResultData result;
        public GameErrorData error;
        public GameResourceManifest resources;
        public Dictionary<string, string> trackingContext = new Dictionary<string, string>();

        public bool HasInput(string actionId, string type)
        {
            if (availableInputs == null)
            {
                return false;
            }

            for (var i = 0; i < availableInputs.Count; i++)
            {
                var input = availableInputs[i];
                if (input == null)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(actionId) && input.actionId == actionId)
                {
                    return true;
                }

                if (!string.IsNullOrEmpty(type) && input.type == type)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public sealed class GameInputAction
    {
        public string actionId;
        public string type;
        public string source;
        public string playerId;
        public int sequence;
        public int tick;
        public string target;
        public string payload;
        public Dictionary<string, string> parameters = new Dictionary<string, string>();
        public long clientTimeMs;

        public static GameInputAction Exit(string source)
        {
            return new GameInputAction
            {
                actionId = GameInputTypes.Exit,
                type = GameInputTypes.Exit,
                source = source,
                clientTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };
        }

        public static GameInputAction FromRoomAction(GameRoomActionData action)
        {
            var actionId = action != null ? action.actionId : string.Empty;
            return new GameInputAction
            {
                actionId = actionId,
                type = ResolveType(actionId),
                source = "game_room_ui",
                target = action != null ? action.label : string.Empty,
                clientTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };
        }

        private static string ResolveType(string actionId)
        {
            switch (actionId)
            {
                case "draw":
                    return GameInputTypes.RequestDraw;
                case "resign":
                    return GameInputTypes.Resign;
                case "undo":
                    return GameInputTypes.Undo;
                case "replay":
                case "review":
                    return GameInputTypes.ReplayPlay;
                case "share":
                    return GameInputTypes.Share;
                case "settings":
                    return GameInputTypes.OpenSettings;
                case "send_comment":
                    return GameInputTypes.SendComment;
                case "flower":
                    return GameInputTypes.Flower;
                default:
                    return string.IsNullOrEmpty(actionId) ? "unknown" : actionId;
            }
        }
    }

    public sealed class GameInputResult
    {
        public bool accepted;
        public string reason;
        public GameViewState viewState;
        public GameResultData result;
        public GameErrorData error;

        public static GameInputResult Accepted(GameViewState viewState, string reason = "accepted")
        {
            return new GameInputResult
            {
                accepted = true,
                reason = reason,
                viewState = viewState
            };
        }

        public static GameInputResult WithResult(GameViewState viewState, GameResultData result, string reason)
        {
            return new GameInputResult
            {
                accepted = true,
                reason = reason,
                viewState = viewState,
                result = result
            };
        }

        public static GameInputResult Rejected(string reason, GameErrorData error = null)
        {
            return new GameInputResult
            {
                accepted = false,
                reason = reason,
                error = error
            };
        }
    }

    public sealed class GameResultData
    {
        public string sessionId;
        public string gameId;
        public string mode;
        public string outcome;
        public string reason;
        public int finalTick;
        public BlockActionData returnAction = BlockActionData.None();
        public Dictionary<string, string> analytics = new Dictionary<string, string>();
    }

    public sealed class GameErrorData
    {
        public string code;
        public string severity;
        public string message;
        public bool recoverable;
        public BlockActionData retryAction = BlockActionData.None();
        public BlockActionData fallbackAction = BlockActionData.None();
        public string source;
        public Dictionary<string, string> details = new Dictionary<string, string>();
    }

    public interface IGameSession : IDisposable
    {
        string SessionId { get; }
        GameSessionState State { get; }
        GameLaunchRequest LaunchRequest { get; }
        GameViewState CurrentViewState { get; }

        bool Prepare(GameLaunchRequest request);
        void Start();
        void Pause();
        void Resume();
        GameInputResult HandleInput(GameInputAction action);
        GameResultData Exit(string reason);
    }

    public interface IGameRuntime : IDisposable
    {
        string RuntimeId { get; }
        GameViewState CreateInitialState(GameLaunchRequest request, string sessionId, GameResourceManifest resources);
        GameInputResult HandleInput(GameInputAction action);
        GameViewState Tick(float deltaTime);
        GameResultData CreateResult(string sessionId, string reason);
    }

    public interface IGameSurfaceRenderer
    {
        void Mount(RectTransform parent, GameResourceManifest resources);
        void Render(GameViewState state, Action<GameInputAction> onInput);
        void SetSessionState(GameSessionState state);
        void Unmount();
    }

    public static class GameRuntimeLog
    {
        public static void State(string sessionId, string gameId, string mode, GameSessionState state, string detail)
        {
            Debug.Log($"[GameRuntime] state gameId={gameId} mode={mode} sessionId={sessionId} state={state} detail={detail}");
        }

        public static void Input(string sessionId, string gameId, string mode, GameInputAction input, bool accepted, string reason)
        {
            var actionId = input != null ? input.actionId : "null";
            var type = input != null ? input.type : "null";
            Debug.Log($"[GameRuntime] input gameId={gameId} mode={mode} sessionId={sessionId} input={actionId}/{type} accepted={accepted} reason={reason}");
        }

        public static void Result(GameResultData result)
        {
            if (result == null)
            {
                return;
            }

            var returnRoute = result.returnAction != null ? result.returnAction.target : string.Empty;
            Debug.Log($"[GameRuntime] result gameId={result.gameId} mode={result.mode} sessionId={result.sessionId} result={result.outcome} reason={result.reason} returnRoute={returnRoute}");
        }

        public static void Fallback(string sessionId, string gameId, string mode, GameResourceManifest manifest)
        {
            var fallback = manifest != null && manifest.fallback != null ? manifest.fallback.type : "none";
            Debug.Log($"[GameRuntime] fallback gameId={gameId} mode={mode} sessionId={sessionId} fallback={fallback}");
        }
    }
}
