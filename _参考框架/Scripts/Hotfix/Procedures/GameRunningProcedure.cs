using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;

/// <summary>
/// 游戏运行流程
/// </summary>
public class GameRunningProcedure : GameBaseProcedure
{
    private IGameRunningUpdater _gameRunningUpdater;
    protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);

        _gameRunningUpdater = UnityEngine.Object.FindObjectOfType<IGameRunningUpdater>();
        if(_gameRunningUpdater == null)
            throw new System.Exception("游戏运行时IGameRunningUpdater未找到");
        
        _gameRunningUpdater.OnGameEnter();
    }

    protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
    {
        _gameRunningUpdater.OnGameExit();
        
        base.OnLeave(procedureOwner, isShutdown);
    }

    protected override void OnNewUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnNewUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        
        _gameRunningUpdater.OnGameUpdate();
    }
}

public abstract class IGameRunningUpdater : MonoBehaviour
{
    public abstract void OnGameEnter();
    public abstract void OnGameUpdate();
    public abstract void OnGameExit();
}