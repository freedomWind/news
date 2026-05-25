using System.Collections.Generic;
using Unity.Collections;

namespace Core.Game
{
    public class GameRoomModule : IGameModule
    {
        private RoomData _room;

        public RoomData RoomInfo => _room;
        protected void Awake()
        {
            _room = new RoomData()
            {
                Players = new List<RoomData.PlayerInfo>()
            };
        }

        public void CreateRoom(int playerCount, string mapAssetName)
        {
            _room = new RoomData()
            {
                Players = new List<RoomData.PlayerInfo>(playerCount),
                PlayerCount = playerCount,
                mapAssetName = "Map_1"
            };
        }

        // public void JoinRoom(int id,int roleId, BattleType.BattleSide side)
        // {
        //     _room.Players.Add(new RoomData.PlayerInfo()
        //     {
        //         playerId = id,
        //         side = side,
        //         heroId = roleId,
        //     });
        // }

        public void LeaveRoom(int id)
        {
            var room = _room;
            for (int i = 0; i < room.Players.Count; i++)
            {
                if (room.Players[i].playerId == id)
                {
                    room.Players.RemoveAtSwapBack(i);
                    break;
                }
            }
        }

        public bool IsAllPlayerReady()
        {
            var room = _room;
            if (room.Players.Count < room.PlayerCount)
                return false;
            foreach (var player in room.Players)
            {
                if (!player.isReady)
                    return false;

            }

            return true;
        }
        
        public class RoomData 
        {
            public class PlayerInfo
            {
                public int playerId;
                public int heroId;
                //public BattleType.BattleSide side;
                public bool isReady;
                public int loadingProgress;
            }
            public List<PlayerInfo> Players;
            public int PlayerCount;
            public string mapAssetName;
        }
    }
}