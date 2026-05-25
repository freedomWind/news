using System;
using NewsFramework.Data.Blocks;
using NewsFramework.Data.GameRoom;
using NewsFramework.Data.Mock;

namespace NewsFramework.GameRuntime
{
    public sealed class GameRoomRuntime : IGameRuntime
    {
        private GameLaunchRequest request;
        private GameViewState viewState;

        public string RuntimeId { get; } = Guid.NewGuid().ToString("N");

        public GameViewState CreateInitialState(GameLaunchRequest launchRequest, string sessionId, GameResourceManifest resources)
        {
            request = launchRequest ?? new GameLaunchRequest();
            var roomData = request.roomData ?? ResolveRoomData(request);
            viewState = new GameViewState
            {
                sessionId = sessionId,
                gameId = request.gameId,
                mode = request.mode,
                phase = GameSessionState.Ready.ToString(),
                tick = 0,
                roomData = roomData,
                resources = resources
            };
            AddRoomInputs(viewState, roomData);
            return viewState;
        }

        public GameInputResult HandleInput(GameInputAction action)
        {
            if (action == null)
            {
                return GameInputResult.Rejected("input_null");
            }

            if (viewState == null)
            {
                return GameInputResult.Rejected("view_state_empty");
            }

            if (!viewState.HasInput(action.actionId, action.type) && action.type != GameInputTypes.Exit)
            {
                return GameInputResult.Rejected(
                    "input_not_available",
                    new GameErrorData
                    {
                        code = "illegal_input",
                        severity = "warning",
                        message = "Input is not available in current state.",
                        recoverable = true,
                        source = "GameRoomRuntime"
                    });
            }

            viewState.tick++;

            if (action.type == GameInputTypes.Resign)
            {
                return GameInputResult.WithResult(viewState, CreateResult(viewState.sessionId, "resign"), "resign");
            }

            return GameInputResult.Accepted(viewState, "handled_" + action.type);
        }

        public GameViewState Tick(float deltaTime)
        {
            return viewState;
        }

        public GameResultData CreateResult(string sessionId, string reason)
        {
            var normalizedReason = string.IsNullOrEmpty(reason) ? "exit" : reason;
            var outcome = normalizedReason == "resign" ? "lose" : "exited";
            return new GameResultData
            {
                sessionId = sessionId,
                gameId = request != null ? request.gameId : "xiangqi",
                mode = request != null ? request.mode : GameRuntimeModes.Spectator,
                outcome = outcome,
                reason = normalizedReason,
                finalTick = viewState != null ? viewState.tick : 0,
                returnAction = new BlockActionData
                {
                    type = "return_route",
                    target = request != null ? request.returnRoute : "home"
                }
            };
        }

        public void Dispose()
        {
            request = null;
            viewState = null;
        }

        private static GameRoomData ResolveRoomData(GameLaunchRequest request)
        {
            if (request == null)
            {
                return GameRoomMockData.CreateSpectatorRoom();
            }

            switch (request.mode)
            {
                case GameRuntimeModes.AiTraining:
                    return GameRoomMockData.CreatePlayerRoom();
                case GameRuntimeModes.Replay:
                    return GameRoomMockData.CreateReplayRoom(request.replayId);
                default:
                    return GameRoomMockData.CreateSpectatorRoom(request.roomId);
            }
        }

        private static void AddRoomInputs(GameViewState state, GameRoomData roomData)
        {
            state.availableInputs.Add(new GameInputDescriptor
            {
                actionId = GameInputTypes.Exit,
                type = GameInputTypes.Exit,
                label = "退出"
            });

            if (roomData == null || roomData.actions == null)
            {
                return;
            }

            for (var i = 0; i < roomData.actions.Count; i++)
            {
                var roomAction = roomData.actions[i];
                if (roomAction == null || string.IsNullOrEmpty(roomAction.actionId))
                {
                    continue;
                }

                var input = GameInputAction.FromRoomAction(roomAction);
                state.availableInputs.Add(new GameInputDescriptor
                {
                    actionId = input.actionId,
                    type = input.type,
                    label = roomAction.label
                });
            }

            if (roomData.IsSpectator() && state.mode != GameRuntimeModes.Replay)
            {
                state.availableInputs.Add(new GameInputDescriptor { actionId = "send_comment", type = GameInputTypes.SendComment, label = "发送" });
                state.availableInputs.Add(new GameInputDescriptor { actionId = "flower", type = GameInputTypes.Flower, label = "献花" });
            }
        }
    }
}
