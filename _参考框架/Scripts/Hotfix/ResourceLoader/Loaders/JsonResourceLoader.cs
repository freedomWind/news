using System;
using System.Collections.Generic;
using GameFramework.Resource;
using UnityEngine;

namespace Hotfix.ResourceLoader.Loaders
{
    /// <summary>
    /// JSON 资源加载器
    /// </summary>
    /// <summary>
    /// JSON 资源加载器 - 只负责加载和解析，不存储数据
    /// </summary>
    public class JsonResourceLoader : IResourceLoader
    {
        public string ResourceType => "Json";
        
        public void Load(string resourceName, Action<object, object> onSuccess, Action<string> onFailure, object userData = null)
        {
            LoadAssetSuccessCallback success = (name, asset, duration, data) =>
            {
                if (asset is TextAsset textAsset)
                {
                    string json = textAsset.text;
                    onSuccess?.Invoke(json, json);
                }
                else
                {
                    onFailure?.Invoke($"资源类型错误，期望 TextAsset，实际: {asset?.GetType()}");
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
            // JSON 字符串不需要在这里解析，由具体的业务系统解析
            // 这里只负责加载和存储原始 JSON 字符串
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
    /// 泛型 JSON 资源加载器（自动解析为指定类型）
    /// </summary>
    /// <summary>
    /// 泛型 JSON 资源加载器（自动解析为指定类型）- 只负责加载和解析，不存储数据
    /// </summary>
    public class JsonResourceLoader<T> : IResourceLoader where T : class
    {
        public string ResourceType => $"Json<{typeof(T).Name}>";
        
        public void Load(string resourceName, Action<object, object> onSuccess, Action<string> onFailure, object userData = null)
        {
            LoadAssetSuccessCallback success = (name, asset, duration, data) =>
            {
                if (asset is TextAsset textAsset)
                {
                    try
                    {
                        string json = textAsset.text;
                        T parsed = JsonUtility.FromJson<T>(json);
                        onSuccess?.Invoke(json, parsed);
                    }
                    catch (Exception e)
                    {
                        onFailure?.Invoke($"JSON 解析失败: {e.Message}");
                    }
                }
                else
                {
                    onFailure?.Invoke($"资源类型错误，期望 TextAsset，实际: {asset?.GetType()}");
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
            if (rawAsset is string json)
            {
                try
                {
                    return JsonUtility.FromJson<T>(json);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[JsonResourceLoader] JSON 解析失败: {resourceName}, 错误: {e.Message}");
                    return null;
                }
            }
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

