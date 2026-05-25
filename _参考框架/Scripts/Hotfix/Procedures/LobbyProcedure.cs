using GameFramework.Fsm;
using GameFramework.Procedure;
using Unity.Entities;
using UnityEngine;
using UnityGameFramework.Runtime;

public class LobbyProcedure : GameBaseProcedure
{
    enum State
    {
        None,
        Enter,
        Update,
        Level
    }

    private State _st;
    private IGameLobby _lobby;
    
    protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        var scene = AssetUtility.GetScene("Lobby");
        EntityInst.Scene.LoadScene(scene);
        _st = State.None;
    }

    protected override void OnNewUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnNewUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        
#if UNITY_EDITOR
#endif

        if (_lobby == null)
        {
            _lobby = UnityEngine.Object.FindObjectOfType<IGameLobby>();
        }

        if (_lobby == null)
            return;
        switch (_st)
        {
            case State.None:
                _lobby.OnInit();
                _st = State.Enter;
                break;
            case State.Enter:
                _lobby.OnLobbyEnter();
                _st = State.Update;
                break;
            case State.Update:
                _lobby.OnLobbyUpdate();
                break;
            default:
                break;
        }
    }

    protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
    {
        // var scene = AssetUtility.GetScene("Lobby");
        // EntityInst.Scene.UnloadScene(scene);
        if (_lobby != null)
        {
            _st = State.Level;
            _lobby.OnLobbyExit();
        }
    }

    public void EnterView()
    {
        ChangeStateWithoutLoading<DisplayProcedure>();
    }
}

public abstract class IGameLobby : UnityEngine.MonoBehaviour
{
    public abstract void OnInit();
    public abstract void OnLobbyEnter();
    public abstract  void OnLobbyUpdate();
    public abstract void OnLobbyExit();
}