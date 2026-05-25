using System;
using System.Collections.Generic;
using GameFramework.Resource;
using UnityEngine;

namespace Hotfix.ResourceLoader.Loaders
{
    /// <summary>
    /// ScriptableObject 资源加载器
    /// </summary>
    /// <summary>
    /// ScriptableObject 资源加载器 - 只负责加载和解析，不存储数据
    /// </summary>
    public class ScriptableObjectResourceLoader : IResourceLoader
    {
        public string ResourceType => "ScriptableObject";
        
        public void Load(string resourceName, Action<object, object> onSuccess, Action<string> onFailure, object userData = null)
        {
            LoadAssetSuccessCallback success = (name, asset, duration, data) =>
            {
                if (asset is ScriptableObject so)
                {
                    onSuccess?.Invoke(so, so);
                }
                else
                {
                    onFailure?.Invoke($"资源类型错误，期望 ScriptableObject，实际: {asset?.GetType()}");
                }
            };
            
            LoadAssetFailureCallback failure = (name, status, message, data) =>
            {
                onFailure?.Invoke($"加载失败: {message}");
            };
            
            EntityInst.Resource.LoadAsset(resourceName, new LoadAssetCallbacks(success, failure), userData);
        }
        
        public object Parse(object rawAsset, string resourceName)
        {
            // ScriptableObject 不需要解析
            return rawAsset;
        }
        
        public void Store(object parsedData, string resourceName)
        {
            // V3版本不存储数据，数据通过回调直接传递给业务层
        }
        
        public object GetStored(string resourceName)
        {
            // V3版本不存储数据，返回null
            return null;
        }
        
        public void Unload(string resourceName)
        {
            EntityInst.Resource.UnloadAsset(resourceName);
        }
    }
    
    /// <summary>
    /// 泛型 ScriptableObject 资源加载器
    /// </summary>
    /// <summary>
    /// 泛型 ScriptableObject 资源加载器 - 只负责加载和解析，不存储数据
    /// </summary>
    public class ScriptableObjectResourceLoader<T> : IResourceLoader where T : ScriptableObject
    {
        public string ResourceType => $"ScriptableObject<{typeof(T).Name}>";
        
        public void Load(string resourceName, Action<object, object> onSuccess, Action<string> onFailure, object userData = null)
        {
            LoadAssetSuccessCallback success = (name, asset, duration, data) =>
            {
                if (asset is T so)
                {
                    onSuccess?.Invoke(so, so);
                }
                else
                {
                    onFailure?.Invoke($"资源类型错误，期望 {typeof(T).Name}，实际: {asset?.GetType()}");
                }
            };
            
            LoadAssetFailureCallback failure = (name, status, message, data) =>
            {
                onFailure?.Invoke($"加载失败: {message}");
            };
            
            EntityInst.Resource.LoadAsset(resourceName, new LoadAssetCallbacks(success, failure), userData);
        }
        
        public object Parse(object rawAsset, string resourceName)
        {
            return rawAsset;
        }
        
        public void Store(object parsedData, string resourceName)
        {
            // V3版本不存储数据，数据通过回调直接传递给业务层
        }
        
        public object GetStored(string resourceName)
        {
            // V3版本不存储数据，返回null
            return null;
        }
        
        public void Unload(string resourceName)
        {
            EntityInst.Resource.UnloadAsset(resourceName);
        }
    }
}

