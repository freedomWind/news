/*
*FileName:	EntityInst
*Author:	油菜花
*CreateTime:2022/5/24 15:19:46
*Description:
*/
using GameFramework.Fsm;
using GameFramework.Procedure;
using GameFramework.Resource;
//using Unity.Entities;
using UnityGameFramework.Runtime;
using UnityEngine;

public class LaunchProcedure : GameBaseProcedure
{
    private bool isSplashOver;
    protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);

        isSplashOver = true;
        Application.runInBackground = true;

        Log.Info("launch..");
        
        
    }

    protected override void OnNewUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        if (isSplashOver)
        {
            if (EntityInst.Base.EditorResourceMode)
            {
                ChangeState<CodeInitProcedure>(procedureOwner);
            }
            else
            {
                ChangeState<CheckVersionProcedure>(procedureOwner);
            }
        }
    }
}