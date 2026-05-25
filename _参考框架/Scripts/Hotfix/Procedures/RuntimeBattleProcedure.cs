/*
*FileName:	EntityInst
*Author:	油菜花
*CreateTime:2022/5/24 15:19:46
*Description:
*/

using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityGameFramework.Runtime;
using GameFramework.Event;
using UnityEngine;

public class RuntimeBattleProcedure : GameBaseProcedure
{
    private int mainUISerialId = -1;
    
    private LoadingForm _loadingForm;
    private float _loadingProgress = 0;
    private bool finishLoadingFlag = false;
    // private BattleType.ViewPointType birthPoint;
    // private BattleType.BattleCountryType country;

    protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);
        
        EntityInst.Event.Subscribe(LoadSceneSuccessEventArgs.EventId,OnSceneLoadSuccess);
        EntityInst.Event.Subscribe(LoadSceneFailureEventArgs.EventId, OnSceneLoadFailure);
        EntityInst.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnUIOpenSuccess);

        GameEntry.GetComponent<DebuggerComponent>().ActiveWindow = false;
        
        _loadingProgress = 0;
        finishLoadingFlag = false;
        
        Log.Info("开始pvp..");

        EntityInst.UI.CloseAllLoadedUIForms();
        var loading = AssetUtility.GetUIForm("BattleLoadingForm");
        EntityInst.UI.OpenUIForm(loading, "Pop", 100, true, this);

        //var mapName = EntityInst.DataNode.GetData<VarString>(GameConstant.BattleRuntimeMapName);
        var mapName = "battle002Data";
        // DataConfigUtility.LoadForBattle(mapName, () =>
        // {
        //     _loadingProgress = 0.3f;
        //
        //     var terrainScene = DataConfigUtility.TerrainConfig.TerrainName;
        //     terrainScene = AssetUtility.GetBattleScene(terrainScene);
        //     Debug.Log("load battle scene:" + terrainScene);
        //     EntityInst.Scene.LoadScene(terrainScene, 100, this);
        //     
        //     RenderEngine.CreateRender();
        // });
    }

    protected override void OnNewUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnNewUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

        if (finishLoadingFlag)
            return;

        if (_loadingForm == null || _loadingProgress == 0)
            return;
        if (_loadingForm.LoadingProgress < _loadingProgress)
        {
            _loadingForm.LoadingProgress += 0.01f;
            return;
        }

        // if (!BattleSetting.IsBattleBegin)
        //     return;

        if (_loadingForm.LoadingProgress >= 1)
        {
            finishLoadingFlag = true;
            EntityInst.UI.CloseUIForm(_loadingForm.UIForm);
            return;
        }

       // _loadingProgress = 0.9999f;
        _loadingForm.LoadingProgress = 1;
        //RenderEngine.GetRenderSystem<BattleUIRenderSystem>().OpenBattleUIs();
    }

    protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
    {
        EntityInst.Event.Unsubscribe(LoadSceneSuccessEventArgs.EventId,OnSceneLoadSuccess);
        EntityInst.Event.Unsubscribe(LoadSceneFailureEventArgs.EventId, OnSceneLoadFailure);
        EntityInst.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnUIOpenSuccess);
        
        //RenderEngine.DestroyRender();
        
        //BattleSetting.BattleDispose();
    }

    private void OnSceneLoadSuccess(object sender, GameEventArgs args)
    {
        var ne = args as LoadSceneSuccessEventArgs;
        if (ne.UserData != this)
            return;
        _loadingProgress = 0.8f;
        
        //就绪
       // BattleSetting.FightReady();
    }

    private void OnSceneLoadFailure(object sender, GameEventArgs args)
    {
        var ne = args as LoadSceneFailureEventArgs;
        if (ne.UserData != this)
            return;
        Log.Error("load scene faliure,scena name = "+ ne.SceneAssetName);
    }

    private void OnUIOpenSuccess(object sender, GameEventArgs args)
    {
        var ne = args as OpenUIFormSuccessEventArgs;
        if (ne.UserData != this)
            return;

        _loadingForm = (LoadingForm) ne.UIForm.Logic;
        _loadingForm.LoadingProgress = 0;
    }
    
    
    public static void ExitBattle()
    {
        if (EntityInst.Procedure.CurrentProcedure.GetType() != typeof(RuntimeBattleProcedure))
        {
            return;
        }

        ChangeStateWithoutLoading<LobbyProcedure>();
    }

    public static void LoadBattle(string battleName)
    {
        EntityInst.DataNode.SetData<VarString>(GameConstant.BattleRuntimeMapName,battleName);
        ChangeStateWithoutLoading<RuntimeBattleProcedure>();
    }
}