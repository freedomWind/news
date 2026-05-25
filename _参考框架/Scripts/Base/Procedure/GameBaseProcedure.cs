/*
*FileName:	EntityInst
*Author:	油菜花
*CreateTime:2022/5/24 15:19:46
*Description:
*/

using System;
using GameFramework.Fsm;
using GameFramework.Procedure;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏内流程
/// </summary>
public abstract class GameBaseProcedure : ProcedureBase
{
    private static IFsm<IProcedureManager> _procedureOwner;
    private static Type _nextProcedureType;
    private static ILoadingForm _loadingForm;

    public static IFsm<IProcedureManager> ProcedureOwner => _procedureOwner;
    protected sealed override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        _procedureOwner = procedureOwner;
        OnNewUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

        if (_loadingForm != null)
        {
            if (_loadingForm.LoadingProgress >= 1f)
            {
                
                _loadingForm.FinishLoading();
                ChangTargetState();
                return;
            }
        }
        ChangTargetState();
    }

    protected virtual void OnNewUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds,
        float realElapseSeconds)
    {
    }

    private void ChangTargetState()
    {
        if (_nextProcedureType == null)
            return;
        ChangeState(_procedureOwner, _nextProcedureType);
        _nextProcedureType = null;
        _loadingForm = null;
    }
    
    //切换流程，带loading模式
    public static void ChangeStateWithLoading<T>(ILoadingForm loadingForm,Action<Action<float>> stateChangeHandler = null) where T : GameBaseProcedure
    {
        if (_nextProcedureType == typeof(T))
            return;
        _nextProcedureType = typeof(T);
        if (loadingForm != null)
        {
            _loadingForm = loadingForm;
            loadingForm.OpenLoadingForm();
            UnloadAllScenes();
            stateChangeHandler?.Invoke(value => loadingForm.LoadingProgress = value);
        }
    }
    //切换流程
    public static void ChangeStateWithoutLoading<T>(Action stateChangeHandler = null) where T : GameBaseProcedure
    {
        if (_nextProcedureType == typeof(T))
            return;
        _nextProcedureType = typeof(T);
        UnloadAllScenes();
        stateChangeHandler?.Invoke();
    }

    private static void UnloadAllScenes()
    {
        var loadedScenes = EntityInst.Scene.GetLoadedSceneAssetNames();
        foreach (var sceneName in loadedScenes)
        {
            EntityInst.Scene.UnloadScene(sceneName);
        }
    }
}
