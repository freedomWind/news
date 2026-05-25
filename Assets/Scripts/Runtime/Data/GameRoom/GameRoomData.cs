using System;
using System.Collections.Generic;

namespace NewsFramework.Data.GameRoom
{
    public enum GameRoomMode
    {
        Player,
        AiTraining,
        Spectator
    }

    public enum GamePieceSide
    {
        Red,
        Black
    }

    [Serializable]
    public sealed class GameRoomData
    {
        public string roomId;
        public string title;
        public GameRoomMode mode = GameRoomMode.Spectator;
        public string roundText;
        public string statusText;
        public string countdownText;
        public int viewerCount;
        public GamePlayerData redPlayer;
        public GamePlayerData blackPlayer;
        public List<GamePieceData> pieces = new List<GamePieceData>();
        public List<GameCapturedPieceData> redCapturedPieces = new List<GameCapturedPieceData>();
        public List<GameCapturedPieceData> blackCapturedPieces = new List<GameCapturedPieceData>();
        public List<GameMoveMarkerData> moveMarkers = new List<GameMoveMarkerData>();
        public List<GameRoomActionData> actions = new List<GameRoomActionData>();
        public List<GameDanmakuData> danmaku = new List<GameDanmakuData>();

        public bool IsSpectator()
        {
            return mode == GameRoomMode.Spectator;
        }
    }

    [Serializable]
    public sealed class GamePlayerData
    {
        public string playerId;
        public string displayName;
        public string rank;
        public string avatarText;
        public string timerText;
        public string sideText;
        public GamePieceSide side;
        public bool active;
        public bool localPlayer;
    }

    [Serializable]
    public sealed class GamePieceData
    {
        public string pieceId;
        public string text;
        public GamePieceSide side;
        public int x;
        public int y;
        public bool highlighted;
    }

    [Serializable]
    public sealed class GameCapturedPieceData
    {
        public string text;
        public GamePieceSide side;
    }

    [Serializable]
    public sealed class GameMoveMarkerData
    {
        public int x;
        public int y;
        public float alpha = 0.6f;
    }

    [Serializable]
    public sealed class GameRoomActionData
    {
        public string actionId;
        public string label;
        public string icon;
    }

    [Serializable]
    public sealed class GameDanmakuData
    {
        public string text;
        public float track;
        public float offset;
    }
}
