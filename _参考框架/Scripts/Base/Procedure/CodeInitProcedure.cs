using System;
using System.Collections.Generic;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityGameFramework.Runtime;
using GameFramework.Resource;
using System.Reflection;
using UnityEngine;
using System.Linq;
using HybridCLR;
//using Unity.Entities;

public class CodeInitProcedure : GameBaseProcedure
{
    class Dll_
    {
         public string dllName;
         public byte[] dllBytes;
         public bool isAotDll;
         public bool main = false;
    }

    private static List<Dll_> _dll = new List<Dll_>()
    {
        //aot
        new Dll_() {dllName = "Assets/RuntimeResources/Hotfix/mscorlib.dll.bytes",isAotDll = true},
        new Dll_() {dllName = "Assets/RuntimeResources/Hotfix/System.dll.bytes",isAotDll = true},
        new Dll_() {dllName = "Assets/RuntimeResources/Hotfix/System.Core.dll.bytes",isAotDll = true},
        new Dll_() {dllName = "Assets/RuntimeResources/Hotfix/UnityEngine.CoreModule.dll.bytes",isAotDll = true},
        new Dll_() {dllName = "Assets/RuntimeResources/Hotfix/Unity.Entities.dll.bytes",isAotDll = true},
        new Dll_() {dllName = "Assets/RuntimeResources/Hotfix/Unity.Collections.dll.bytes",isAotDll = true},
        
        new Dll_() { dllName = "Assets/RuntimeResources/Hotfix/Game.Hotfix.dll.bytes",isAotDll =  false, main = true, },
        new Dll_() {dllName = "Assets/RuntimeResources/Hotfix/Main.dll.bytes",isAotDll = false},
        new Dll_() {dllName = "Assets/RuntimeResources/Hotfix/Main.Render.dll.bytes",isAotDll = false},
    };

    private bool m_IsLoadedDll = false;
    private Assembly mainAssembly = null;

    public static List<Type> EntitiesSystemTypes;

    protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);
        
        if (EntityInst.Base.EditorResourceMode)
        {
            m_IsLoadedDll = true;
        }
        else
        {
            m_IsLoadedDll = false;
            foreach (var aotDllName in _dll)
            {
                EntityInst.Resource.LoadAsset(aotDllName.dllName,
                    new LoadAssetCallbacks(OnLoadAssetSuccess, OnLoadAssetFail));
            }
        }
    }

    protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
    {
        m_IsLoadedDll = false;

        base.OnLeave(procedureOwner, isShutdown);
    }

    protected override void OnNewUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        if (!m_IsLoadedDll)
        {
            return;
        }

        if (EntityInst.Base.EditorResourceMode)
        {
            mainAssembly = AppDomain.CurrentDomain.GetAssemblies().First(x => x.GetName().Name == "Game.Hotfix");
        }
        var hotfixEntry = mainAssembly.GetType("GameHotfixEntry");
        var start = hotfixEntry.GetMethod("Start");
        start?.Invoke(null, null);
    }

    private void OnLoadAssetSuccess(string assetName, object asset, float duration, object userData)
    {
        TextAsset dll = (TextAsset)asset;
        Debug.Log($"加载asset:{assetName}");
        _dll.Find(x => x.dllName == assetName).dllBytes = dll.bytes;
        m_IsLoadedDll = !(_dll.Exists(x => x.dllBytes == null));
        if (m_IsLoadedDll)
        {
            LoadMetadataForAOTAssembly();
            
            foreach (var o in _dll)
            {
                if (o.isAotDll)
                    continue;
                var tmp = AppDomain.CurrentDomain.Load(o.dllBytes);
                if (o.main)
                {
                    mainAssembly = tmp;
                }
            }
        }
    }

    private void OnLoadAssetFail(string assetName, LoadResourceStatus status, string errorMessage, object userData)
    {
        Log.Error("Load hotfix dll failed. " + errorMessage);
    }

    /// <summary>
    /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
    /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
    /// </summary>
    static void LoadMetadataForAOTAssembly()
    {
        // 可以加载任意aot assembly的对应的dll。但要求dll必须与unity build过程中生成的裁剪后的dll一致，而不能直接使用原始dll。
        // 我们在Huatuo_BuildProcessor_xxx里添加了处理代码，这些裁剪后的dll在打包时自动被复制到 {项目目录}/HuatuoData/AssembliesPostIl2CppStrip/{Target} 目录。
        
        /// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
        /// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
        foreach (var o in _dll)
        {
            if (!o.isAotDll)
                continue;
            // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
            var err = HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(o.dllBytes, HomologousImageMode.SuperSet);
            Debug.Log($"LoadMetadataForAOTAssembly:{o.dllName}. ret:{err}");
        }
    }
    
    //static void
}