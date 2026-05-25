/*
*FileName:	EntityInst
*Author:	油菜花
*CreateTime:2022/5/24 15:19:46
*Description:
*/

using System;
using GameFramework.Fsm;
using GameFramework.Procedure;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

public class SplashProcedure : GameBaseProcedure
{
    protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);
        
        // 构建信息：发布版本时，把一些数据以 Json 的格式写入 Assets/GameMain/Configs/BuildInfo.txt，供游戏逻辑读取
        EntityInst.BuiltinData.InitBuildInfo();
        
        //Resources.Load<PlayerSettings.SplashScreenLogo>()
        World.DefaultGameObjectInjectionWorld = null;
    }

    protected override void OnNewUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnNewUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        ChangeState<LaunchProcedure>(procedureOwner);
    }
}

