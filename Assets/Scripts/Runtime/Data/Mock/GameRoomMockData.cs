using System.Collections.Generic;
using NewsFramework.Data.GameRoom;

namespace NewsFramework.Data.Mock
{
    public static class GameRoomMockData
    {
        public static GameRoomData CreatePlayerRoom()
        {
            return new GameRoomData
            {
                roomId = "ai_training_001",
                title = "AI陪练 · 业余五段",
                mode = GameRoomMode.AiTraining,
                roundText = "第 24 回合",
                statusText = "轮到你走棋了",
                countdownText = "00:45",
                redPlayer = new GamePlayerData
                {
                    playerId = "player_zhangsan",
                    displayName = "张三",
                    rank = "业余五段",
                    avatarText = "张",
                    timerText = "00:45",
                    sideText = "红方",
                    side = GamePieceSide.Red,
                    active = true,
                    localPlayer = true
                },
                blackPlayer = new GamePlayerData
                {
                    playerId = "ai_lisi",
                    displayName = "李四",
                    rank = "业余七段",
                    avatarText = "李",
                    timerText = "01:23",
                    sideText = "黑方",
                    side = GamePieceSide.Black
                },
                pieces = CreateOpeningPieces(),
                actions = new List<GameRoomActionData>
                {
                    CreateAction("draw", "求和"),
                    CreateAction("resign", "认输"),
                    CreateAction("undo", "悔棋"),
                    CreateAction("review", "复盘")
                }
            };
        }

        public static GameRoomData CreateSpectatorRoom(string matchId = "")
        {
            return new GameRoomData
            {
                roomId = string.IsNullOrEmpty(matchId) ? "match_live_001" : matchId,
                title = "此刻棋坛",
                mode = GameRoomMode.Spectator,
                roundText = "第 42 回合",
                statusText = "红方思考中",
                countdownText = "00:28",
                viewerCount = 23,
                redPlayer = new GamePlayerData
                {
                    playerId = "player_zhaoliu",
                    displayName = "赵六",
                    rank = "业余七段",
                    avatarText = "赵",
                    sideText = "红方",
                    side = GamePieceSide.Red,
                    active = true
                },
                blackPlayer = new GamePlayerData
                {
                    playerId = "player_wangwu",
                    displayName = "王五",
                    rank = "业余六段",
                    avatarText = "王",
                    sideText = "黑方",
                    side = GamePieceSide.Black
                },
                pieces = CreateSpectatorPieces(),
                redCapturedPieces = new List<GameCapturedPieceData>
                {
                    CreateCaptured("炮", GamePieceSide.Red),
                    CreateCaptured("马", GamePieceSide.Red),
                    CreateCaptured("兵", GamePieceSide.Red)
                },
                blackCapturedPieces = new List<GameCapturedPieceData>
                {
                    CreateCaptured("卒", GamePieceSide.Black),
                    CreateCaptured("马", GamePieceSide.Black),
                    CreateCaptured("象", GamePieceSide.Black)
                },
                moveMarkers = new List<GameMoveMarkerData>
                {
                    new GameMoveMarkerData { x = 6, y = 3, alpha = 0.65f },
                    new GameMoveMarkerData { x = 6, y = 5, alpha = 0.3f }
                },
                actions = new List<GameRoomActionData>
                {
                    CreateAction("replay", "棋局回顾", "◇"),
                    CreateAction("share", "分享对局", "↗"),
                    CreateAction("settings", "观战设置", "⚙")
                },
                danmaku = new List<GameDanmakuData>
                {
                    new GameDanmakuData { text = "这步棋走得妙啊", track = 0f, offset = 18f },
                    new GameDanmakuData { text = "老赵这盘稳了", track = 1f, offset = 116f },
                    new GameDanmakuData { text = "红方要小心底线了", track = 2f, offset = 58f }
                }
            };
        }

        private static GameRoomActionData CreateAction(string actionId, string label, string icon = "")
        {
            return new GameRoomActionData
            {
                actionId = actionId,
                label = label,
                icon = icon
            };
        }

        private static GameCapturedPieceData CreateCaptured(string text, GamePieceSide side)
        {
            return new GameCapturedPieceData
            {
                text = text,
                side = side
            };
        }

        private static List<GamePieceData> CreateOpeningPieces()
        {
            return new List<GamePieceData>
            {
                Piece("b_ju_1", "車", GamePieceSide.Black, 0, 0),
                Piece("b_ma_1", "馬", GamePieceSide.Black, 1, 0),
                Piece("b_xiang_1", "象", GamePieceSide.Black, 2, 0),
                Piece("b_shi_1", "士", GamePieceSide.Black, 3, 0),
                Piece("b_jiang", "將", GamePieceSide.Black, 4, 0),
                Piece("b_shi_2", "士", GamePieceSide.Black, 5, 0),
                Piece("b_xiang_2", "象", GamePieceSide.Black, 6, 0),
                Piece("b_ma_2", "馬", GamePieceSide.Black, 7, 0),
                Piece("b_ju_2", "車", GamePieceSide.Black, 8, 0),
                Piece("b_pao_1", "炮", GamePieceSide.Black, 1, 2),
                Piece("b_pao_2", "炮", GamePieceSide.Black, 7, 2),
                Piece("b_zu_1", "卒", GamePieceSide.Black, 0, 3),
                Piece("b_zu_2", "卒", GamePieceSide.Black, 2, 3),
                Piece("b_zu_3", "卒", GamePieceSide.Black, 4, 3),
                Piece("b_zu_4", "卒", GamePieceSide.Black, 6, 3),
                Piece("b_zu_5", "卒", GamePieceSide.Black, 8, 3),
                Piece("r_ju_1", "車", GamePieceSide.Red, 0, 9),
                Piece("r_ma_1", "馬", GamePieceSide.Red, 1, 9),
                Piece("r_xiang_1", "相", GamePieceSide.Red, 2, 9),
                Piece("r_shi_1", "仕", GamePieceSide.Red, 3, 9),
                Piece("r_shuai", "帥", GamePieceSide.Red, 4, 9, true),
                Piece("r_shi_2", "仕", GamePieceSide.Red, 5, 9),
                Piece("r_xiang_2", "相", GamePieceSide.Red, 6, 9),
                Piece("r_ma_2", "馬", GamePieceSide.Red, 7, 9),
                Piece("r_ju_2", "車", GamePieceSide.Red, 8, 9),
                Piece("r_pao_1", "炮", GamePieceSide.Red, 1, 7),
                Piece("r_pao_2", "炮", GamePieceSide.Red, 7, 7),
                Piece("r_bing_1", "兵", GamePieceSide.Red, 0, 6),
                Piece("r_bing_2", "兵", GamePieceSide.Red, 2, 6),
                Piece("r_bing_3", "兵", GamePieceSide.Red, 4, 6),
                Piece("r_bing_4", "兵", GamePieceSide.Red, 6, 6),
                Piece("r_bing_5", "兵", GamePieceSide.Red, 8, 6)
            };
        }

        private static List<GamePieceData> CreateSpectatorPieces()
        {
            return new List<GamePieceData>
            {
                Piece("r_shuai", "帅", GamePieceSide.Red, 4, 9, true),
                Piece("r_shi_1", "仕", GamePieceSide.Red, 3, 8),
                Piece("r_shi_2", "仕", GamePieceSide.Red, 5, 8),
                Piece("r_bing", "兵", GamePieceSide.Red, 4, 6),
                Piece("r_pao", "炮", GamePieceSide.Red, 2, 4),
                Piece("r_ma", "马", GamePieceSide.Red, 6, 3),
                Piece("b_jiang", "将", GamePieceSide.Black, 4, 0),
                Piece("b_shi", "士", GamePieceSide.Black, 3, 1),
                Piece("b_zu", "卒", GamePieceSide.Black, 4, 2),
                Piece("b_ju", "车", GamePieceSide.Black, 1, 3),
                Piece("b_pao", "炮", GamePieceSide.Black, 7, 5)
            };
        }

        private static GamePieceData Piece(
            string pieceId,
            string text,
            GamePieceSide side,
            int x,
            int y,
            bool highlighted = false)
        {
            return new GamePieceData
            {
                pieceId = pieceId,
                text = text,
                side = side,
                x = x,
                y = y,
                highlighted = highlighted
            };
        }
    }
}
