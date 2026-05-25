using System;
using GameFramework.Procedure;
using Unity.Entities;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Core.Game
{
    public class GameBattleModule : IGameModule
    {
        //private FrameWorld _world;

        public void GameStartLoad()
        {
            // EntityInst.Resource.LoadAsset(AssetUtility.GetBattleAsset("MapConfigs/Battle003.asset"), (asset) =>
            // {
            //     var mapData = (MapDescriptionData)asset;
            //     mapData.OnLoad();
            // });
            //
            // if (!EntityInst.Entity.HasEntityGroup("UnitGroup"))
            // {
            //     EntityInst.Entity.AddEntityGroup("UnitGroup", 120, 20, 200, 100);
            // }
            //
            // var fsm = EntityInst.Fsm.GetFsm<IProcedureManager>();
            // fsm.SetData<VarBoolean>("QuickStart", true);
        }
        
        public void GameStart()
        {
            // _world = FrameWorld.CreateFrameWorld();
            // _world.Start();
        }

        public void CreateBattle(int type)
        {
            // var room = GameModuleComponent.MM.GetModule<GameRoomModule>();
            // if (!room.IsAllPlayerReady())
            // {
            //     Debug.LogError("room not ready");
            //     return;
            // }
            
            DefaultWorldInitialization.Initialize("Default World", false);
        }

        private void FixedUpdate()
        {
            // if (_world != null)
            // {
            //     _world.Update();
            // }
           // ClientUtility.FrameTick();
        }
    }

    public class GameConfigModule : IGameModule
    {
        public void GameStartLoad()
        {
            // EntityInst.Resource.LoadAsset(AssetUtility.GetBattleAsset("Configs/BattleConfig.asset"), (asset) =>
            // {
            //     var battleConfig = (BattleConfig)asset;
            //     battleConfig.OnLoad();
            // });
        }

        public void StartGame(int id)
        {
            
        }
    }
}