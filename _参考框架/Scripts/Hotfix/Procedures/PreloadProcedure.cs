using System.Collections.Generic;
using System.IO;
using GameFramework.Event;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;

public class PreloadProcedure : GameBaseProcedure
{
    public static string[] DataTableNames = new string[]
    {
        @"MapConfig",
        "CardList",
        "CardGroup"
    };

    private readonly Dictionary<string, bool> _allLoadFlags = new Dictionary<string, bool>();
    protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);
        
        EntityInst.Event.Subscribe(LoadDataTableSuccessEventArgs.EventId, OnLoadDataTableSuccess);
        EntityInst.Event.Subscribe(LoadDataTableFailureEventArgs.EventId, OnLoadDataTableFailure);

        foreach (var dt in DataTableNames)
        {
            var assetName = AssetUtility.GetTableAsset(dt);
            EntityInst.Table.LoadDataTable(dt, assetName, this);
            _allLoadFlags[dt] = false;
        }
    }

    protected override void OnNewUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnNewUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        
        // foreach (var kvp in _allLoadFlags)
        // {
        //     if (kvp.Value == false)
        //         return;
        // }
        ChangeState<Preload2Procedure>(procedureOwner);
    }

    protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
    {
        base.OnLeave(procedureOwner, isShutdown);
        EntityInst.Event.Unsubscribe(LoadDataTableSuccessEventArgs.EventId, OnLoadDataTableSuccess);
        EntityInst.Event.Unsubscribe(LoadDataTableFailureEventArgs.EventId, OnLoadDataTableFailure);
    }

    private void OnLoadDataTableFailure(object sender, GameEventArgs e)
    {
        var ne = e as LoadDataTableFailureEventArgs;
        if (ne.UserData != this)
            return;
        Log.Error($"Load DataTable Failure:tb = {ne.DataTableAssetName}");
    }

    private void OnLoadDataTableSuccess(object sender, GameEventArgs e)
    {
        var ne = e as LoadDataTableSuccessEventArgs;
        if (ne.UserData != this)
            return;
        Log.Info($"Load DataTable Success:tb = {ne.DataTableAssetName}");
        var dt = Path.GetFileNameWithoutExtension(ne.DataTableAssetName);
        //var dt = ne.DataTableAssetName.Substring(0, ne.DataTableAssetName.IndexOf('.'));
        
        Debug.Log($"dt name = {dt}");
        if (_allLoadFlags.ContainsKey(dt))
        {
            _allLoadFlags[dt] = true;
        }
    }
}