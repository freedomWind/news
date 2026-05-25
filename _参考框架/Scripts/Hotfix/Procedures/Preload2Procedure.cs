using System;
using System.Collections.Generic;
using System.IO;
using Core.Game;
using GameFramework.Fsm;
using GameFramework.Procedure;
using GameFramework.Resource;
using Unity.Entities;
using UnityEngine;
using UnityGameFramework.Runtime;

public class Preload2Procedure : GameBaseProcedure
{
    
#if UNITY_EDITOR
    public static bool EffectTestMode = false;
#endif

    private readonly Dictionary<string, bool> _allLoadFlags = new Dictionary<string, bool>();
    protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);
        
        

        // //EntityInst.Config.AddConfig()
        // var str = EntityInst.Config.GetString("BattleAssets");
        // if (!string.IsNullOrEmpty(str))
        // {
        //     EntityInst.DataNode.SetData("BattleAssets", new BattleAssets()
        //     {
        //         battleAssets = new List<BattleSyncLoadObjectData>()
        //     });
        //     
        //     var callbacks = new LoadAssetCallbacks(this.OnLoadAssetSuccess, this.OnLoadAssetFailure);
        //     var assets = str.Split('#');
        //     foreach (var asset in assets)
        //     {
        //         if (string.IsNullOrEmpty(asset))
        //             continue;
        //         var assetName = AssetUtility.GetBattleAsset($"Configs/{asset}.asset");
        //         _allLoadFlags[asset] = false;
        //         Debug.Log($"load asset:{assetName}");
        //         EntityInst.Resource.LoadAsset(assetName, 100, callbacks);
        //     }
        // }
        //
        // EntityInst.Resource.LoadAsset(AssetUtility.GetBattleAsset("MapConfigs/BattleDesc.asset"), (asset) =>
        // {
        //     var mapData = (BattleDescriptionData)asset;
        //     mapData.OnLoad();
        // });
        //
        // GameEffectInit.Init();
        // UnitObjectData.Init();
        // PlayerObjectData.Init();
        // SkillObjectData.Init();
        // GameUtility.UnitPositionUtility.Init();
        //
        // GameModuleComponent.Init();
    }

    private void OnLoadAssetSuccess(string assetName, object asset, float duration, object userData)
    {
        try
        {
            var name = Path.GetFileNameWithoutExtension(assetName);
            _allLoadFlags[name] = true;
            // var battleAssets = EntityInst.DataNode.GetData<BattleAssets>("BattleAssets");
            // battleAssets.battleAssets.Add((BattleSyncLoadObjectData)asset);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error adding asset {assetName} to BattleAssets: {e.Message}");
        }
    }

    private void OnLoadAssetFailure(string assetName, LoadResourceStatus status, string errorMessage, object userData)
    {
        Log.Error($"Load Asset Failure:asset = {assetName},status = {status},errorMessage = {errorMessage}");
    }

    protected override void OnNewUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnNewUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

        foreach (var kvp in _allLoadFlags)
        {
            if (kvp.Value == false)
                return;
        }
#if UNITY_EDITOR
        if (EffectTestMode)
            return;
#endif
        InitializeWorld();
        ChangeState<ADInitProcedure>(procedureOwner);
    }

    protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
    {
        base.OnLeave(procedureOwner, isShutdown);
    }
    private static void InitializeWorld()
    {
        TypeManager.Shutdown();
        TypeManager.Initialize();
        //TypeManager.InitializeSharedStatics();
    }
}