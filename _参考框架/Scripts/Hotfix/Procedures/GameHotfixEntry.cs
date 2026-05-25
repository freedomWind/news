using GameFramework;
using GameFramework.Fsm;
using GameFramework.Procedure;
using GameFramework.Resource;
using UnityEngine;
using UnityGameFramework.Runtime;
using System.Collections.Generic;
using System;
using Unity.Entities;

public class GameHotfixEntry
{
    public static void Start()
    {
        // 删除原生对话框。
        //EntityInst.BuiltinData.DestroyDialog();

        // 重置流程组件，初始化热更新流程。
        EntityInst.Fsm.DestroyFsm<IProcedureManager>();
        var procedureManager = GameFrameworkEntry.GetModule<IProcedureManager>();
        ProcedureBase[] procedures =
        {
            new PreloadProcedure(),
            new Preload2Procedure(),
            new ADInitProcedure(),
            new LobbyProcedure(),
            new LoginRegistProcedure(),
            new RuntimeBattleProcedure(),
            new GameBattle2pProcedure(),
            new DisplayProcedure(),
            new GameLoadProcedureV3(),
            new GameRunningProcedure()
            
        };
        procedureManager.Initialize(GameFrameworkEntry.GetModule<IFsmManager>(), procedures);
        procedureManager.StartProcedure<PreloadProcedure>();

        // 加载自定义组件。
        EntityInst.Resource.LoadAsset("Assets/RuntimeResources/Utility/CustomGame.prefab",
            new LoadAssetCallbacks(OnLoadAssetSuccess, OnLoadAssetFail));
    }

    private static void OnLoadAssetSuccess(string assetName, object asset, float duration, object userdata)
    {
        GameObject game = UnityEngine.Object.Instantiate((GameObject) asset);
        game.name = "Custom";
        var trans = GameObject.Find("_framework/GameFramework");
        game.transform.SetParent(trans.transform);
    }

    private static void OnLoadAssetFail(string assetName, LoadResourceStatus status, string errormessage,
        object userdata)
    {
        Log.Error("Load game failed. {0}", errormessage);
    }
}