using System;
using System.Collections.Generic;
using GameFramework.Fsm;
using GameFramework.Procedure;
using Hotfix.ResourceLoader;
using Hotfix.ResourceLoader.Loaders;
using UnityEngine;

/// <summary>
/// 游戏玩法加载流程 V3 - 简洁版本，不存储数据，只做加载工具
/// </summary>
public class GameLoadProcedureV3 : GameBaseProcedure
{
    private static ResourceLoadConfig _loadConfig = new ResourceLoadConfig();
    private SimpleResourceLoader _loader;
    private Dictionary<string, ResourceLoadConfigItem> _loadingTasks = new Dictionary<string, ResourceLoadConfigItem>();
    private int _completedCount = 0;
    private int _failedCount = 0;
    
    /// <summary>
    /// 配置加载项（静态方法，在进入 Procedure 前调用）
    /// </summary>
    /// <param name="config">加载配置</param>
    public static void ConfigureLoad(ResourceLoadConfig config)
    {
        _loadConfig = config ?? new ResourceLoadConfig();
    }
    
    /// <summary>
    /// 便捷方法：快速配置加载项
    /// </summary>
    public static ResourceLoadConfig Configure()
    {
        return _loadConfig;
    }
    
    protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);
        
        _loader = SimpleResourceLoader.Instance;
        _loadingTasks.Clear();
        _completedCount = 0;
        _failedCount = 0;
        
        // 注册默认加载器
        RegisterDefaultLoaders();
        
        // 开始加载所有资源
        StartLoading();
    }
    
    protected override void OnNewUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnNewUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        
        // 检查是否所有任务完成
        int totalTasks = _loadingTasks.Count;
        if (totalTasks == 0)
        {
            return; // 没有加载任务
        }
        
        // 如果有失败的任务，切换到失败流程
        if (_failedCount > 0)
        {
            ChangeState<GameLoadFailureProcedure>(procedureOwner);
            return;
        }
        
        // 如果所有任务都完成，切换到运行流程
        if (_completedCount >= totalTasks)
        {
            Debug.Log($"[GameLoadProcedure] 所有资源加载完成，共 {_completedCount} 个");
            ChangeState<GameRunningProcedure>(procedureOwner);
        }
    }
    
    protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
    {
        base.OnLeave(procedureOwner, isShutdown);
        
        // 清除配置（可选，如果需要保留可以注释掉）
        // _loadConfig.Clear();
        _loadingTasks.Clear();
    }
    
    /// <summary>
    /// 注册默认加载器
    /// </summary>
    private void RegisterDefaultLoaders()
    {
        _loader.RegisterLoader(new SceneResourceLoader());
        _loader.RegisterLoader(new BinaryResourceLoader());
        _loader.RegisterLoader(new AssetResourceLoader());
        _loader.RegisterLoader(new ScriptableObjectResourceLoader());
    }
    
    /// <summary>
    /// 开始加载所有资源
    /// </summary>
    private void StartLoading()
    {
        var items = _loadConfig.GetItems();
        
        if (items.Count == 0)
        {
            Debug.LogWarning("[GameLoadProcedure] 没有配置任何加载项");
            return;
        }
        
        Debug.Log($"[GameLoadProcedure] 开始加载 {items.Count} 个资源");
        
        foreach (var item in items)
        {
            string taskKey = $"{item.ResourceType}:{item.ResourceName}";
            _loadingTasks[taskKey] = item;
            
            // 加载资源
            _loader.Load(
                item.ResourceType,
                item.ResourceName,
                (data) => OnResourceLoadSuccess(taskKey, data),
                (error) => OnResourceLoadFailure(taskKey, error),
                item.UserData
            );
        }
    }
    
    /// <summary>
    /// 资源加载成功
    /// </summary>
    private void OnResourceLoadSuccess(string taskKey, object data)
    {
        if (_loadingTasks.TryGetValue(taskKey, out var item))
        {
            try
            {
                // 调用业务层的处理逻辑
                item.OnLoadSuccess?.Invoke(data);
                _completedCount++;
            }
            catch (Exception e)
            {
                Debug.LogError($"[GameLoadProcedure] 处理资源失败: {taskKey}, 错误: {e.Message}");
                _failedCount++;
            }
        }
    }
    
    /// <summary>
    /// 资源加载失败
    /// </summary>
    private void OnResourceLoadFailure(string taskKey, string error)
    {
        if (_loadingTasks.TryGetValue(taskKey, out var item))
        {
            // 调用业务层的失败处理逻辑
            item.OnLoadFailure?.Invoke(error);
            _failedCount++;
        }
    }
}

