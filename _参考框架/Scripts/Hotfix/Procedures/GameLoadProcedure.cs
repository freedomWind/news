using System;
using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using GameFramework.Fsm;
using GameFramework.Procedure;
using GameFramework.Resource;
using UnityEngine;
using UnityGameFramework.Runtime;
using Object = System.Object;

/// <summary>
/// 游戏玩法加载流程
/// </summary>
public class GameLoadProcedure : GameBaseProcedure
{
    private struct LoadInfo
    {
        public int code;
        public string loadName;
    }
    private Dictionary<int, int> _loadMap = new Dictionary<int, int>();
    
    protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);

        _loadMap.Clear();
                
        // 订阅场景加载事件
        EntityInst.Event.Subscribe(LoadSceneSuccessEventArgs.EventId, OnSceneLoadSuccess);
        EntityInst.Event.Subscribe(LoadSceneUpdateEventArgs.EventId, OnSceneLoading);
        EntityInst.Event.Subscribe(LoadSceneFailureEventArgs.EventId, OnSceneLoadFailure);
        EntityInst.Event.Subscribe(LoadDataTableSuccessEventArgs.EventId, OnDataTableLoadSuccess);
        EntityInst.Event.Subscribe(LoadDataTableFailureEventArgs.EventId, OnDataTableLoadFailure);
        
        LoadAssetSuccessCallback success = (name, asset, duration, data) =>
        {
            if (data is int code) 
            {
                _loadMap[code] = 1;
            }
        };
        LoadAssetFailureCallback failure = (name, status, message, data) =>
        {
            if (data is int code)
            {
                _loadMap[code] = 2;
            }
        };
        
        // 获取要加载的游戏玩法，数据表，预加载资源
        string playName = procedureOwner.HasData("GamePlayName") ? procedureOwner.GetData<VarString>("GamePlayName") : string.Empty;
        string dataTableName = procedureOwner.HasData("GameTable") ? procedureOwner.GetData<VarString>("GameTable") : string.Empty;
        string preloadAssets = procedureOwner.HasData("GamePreload") ? procedureOwner.GetData<VarString>("GamePreload") : string.Empty;
        
        if (!string.IsNullOrEmpty(playName))
        {
            if (playName.Contains("#"))
            {
                string[] split = playName.Split('#');
                foreach (var name in split)
                {
                    var code = name.GetHashCode();
                    _loadMap.Add(code, 0);
                    EntityInst.Scene.LoadScene(name, 100, code);
                }
            }
            else
            {
                var code = playName.GetHashCode();
                _loadMap.Add(code, 0);
                Debug.Log($"load scene :{playName}");
                EntityInst.Scene.LoadScene(playName, 100, code);
            }
        }
        if (!string.IsNullOrEmpty(dataTableName))
        {
            if (dataTableName.Contains("#"))
            {
                string[] split = dataTableName.Split('#');
                foreach (var name in split)
                {
                    var code = name.GetHashCode();
                    _loadMap.Add(code, 0);
                    var assetName = AssetUtility.GetTableAsset(name);
                    //EntityInst.Table.LoadDataTable(name, assetName, code);
                }
            }
            else
            {
                var code = dataTableName.GetHashCode();
                _loadMap.Add(code, 0);
                var assetName = AssetUtility.GetTableAsset(dataTableName);
               // EntityInst.Table.LoadDataTable(dataTableName, assetName, code);
            }
        }
        if (!string.IsNullOrEmpty(preloadAssets))
        {
            if (preloadAssets.Contains("#"))
            {
                string[] split = preloadAssets.Split('#');
                foreach (var name in split)
                {
                    var code = name.GetHashCode();
                    _loadMap.Add(code, 0);
                    EntityInst.Resource.LoadAsset(name, new LoadAssetCallbacks(success, failure), code);
                }
            }
            else
            {
                var code = preloadAssets.GetHashCode();
                _loadMap.Add(code, 0);
                EntityInst.Resource.LoadAsset(preloadAssets, new LoadAssetCallbacks(success, failure), code);
            }
        }
    }

    protected override void OnNewUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnNewUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        
        foreach (var kvp in _loadMap)
        {
            if (kvp.Value == 2)
            {
                ChangeState<GameLoadFailureProcedure>(procedureOwner);
                return;
            }
            if (kvp.Value == 3)
            {
                return;
            }

            if (kvp.Value == 0)
            {
                return;
            }
        }
        ChangeState<GameRunningProcedure>(procedureOwner);
    }

    protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
    {
        base.OnLeave(procedureOwner, isShutdown);
        
        // 取消订阅事件
        EntityInst.Event.Unsubscribe(LoadSceneSuccessEventArgs.EventId, OnSceneLoadSuccess);
        EntityInst.Event.Unsubscribe(LoadSceneUpdateEventArgs.EventId, OnSceneLoading);
        EntityInst.Event.Unsubscribe(LoadSceneFailureEventArgs.EventId, OnSceneLoadFailure);
        EntityInst.Event.Unsubscribe(LoadDataTableSuccessEventArgs.EventId, OnDataTableLoadSuccess);
        EntityInst.Event.Unsubscribe(LoadDataTableFailureEventArgs.EventId, OnDataTableLoadFailure);
    }
    
    private void OnSceneLoadSuccess(object sender, GameEventArgs args)
    {
        var ne = args as LoadSceneSuccessEventArgs;
        if (ne.UserData is int code)
        {
            _loadMap[code] = 1;
        }
    }

    private void OnSceneLoading(object sender, GameEventArgs args)
    {
        var ne = args as LoadSceneUpdateEventArgs;
        if (ne.UserData is int code)
        {
            _loadMap[code] = 3;
        }
    }
    private void OnSceneLoadFailure(object sender, GameEventArgs args)
    {
        var ne = args as LoadSceneFailureEventArgs;
        if (ne.UserData is int code)
        {
            _loadMap[code] = 2;
        }
    }

    private void OnDataTableLoadSuccess(object sender, GameEventArgs args)
    {
        var ne = args as LoadDataTableSuccessEventArgs;
        if (ne.UserData is int code)
        {
            _loadMap[code] = 1;
        }
    }

    private void OnDataTableLoadFailure(object sender, GameEventArgs args)
    {
        var ne = args as LoadDataTableFailureEventArgs;
        if (ne.UserData is int code)
        {
            _loadMap[code] = 2;
        }
    }
}

public class GameLoadFailureProcedure : GameBaseProcedure
{
    protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);
    }

    protected override void OnNewUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnNewUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
    }

    protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
    {
        base.OnLeave(procedureOwner, isShutdown);
    }
}