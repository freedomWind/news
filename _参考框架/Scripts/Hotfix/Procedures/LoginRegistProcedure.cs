using System;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Runtime;

public class LoginRegistProcedure : GameBaseProcedure
{
    private int loginId = -1;
    
    protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        loginId = EntityInst.UI.OpenUIForm(AssetUtility.GetUIForm("LoginRegisterForm"), "Default", this);

        procedureOwner.SetData<VarString>("BGM", "defaultBgm");
    }

    protected override void OnNewUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnNewUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        
        // 检查登录结果（通过Procedure数据）
        var result = procedureOwner.HasData("LoginResult") && procedureOwner.GetData<VarBoolean>("LoginResult").Value;
        if (result)
        {
            //切换到大厅流程
            ChangeStateWithoutLoading<LobbyProcedure>();
        }
    }

    protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
    {
        base.OnLeave(procedureOwner, isShutdown);
        
        if (loginId != -1)
        {
            EntityInst.UI.CloseUIForm(loginId);
        }
    }
}