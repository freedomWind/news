using System;
using System.Collections.Generic;
using System.IO;
using GameFramework.Resource;
using UnityEngine;

namespace Hotfix.ResourceLoader.Loaders
{
    /// <summary>
    /// 二进制资源加载器
    /// </summary>
    /// <summary>
    /// 二进制资源加载器 - 只负责加载和解析，不存储数据
    /// </summary>
    public class BinaryResourceLoader : IResourceLoader
    {
        public string ResourceType => "Binary";
        
        public void Load(string resourceName, Action<object, object> onSuccess, Action<string> onFailure, object userData = null)
        {
            // 使用 GameFramework 的资源系统加载
            LoadAssetSuccessCallback success = (name, asset, duration, data) =>
            {
                if (asset is TextAsset textAsset)
                {
                    byte[] bytes = textAsset.bytes;
                    onSuccess?.Invoke(bytes, bytes);
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
            // 二进制数据不需要解析，直接返回
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

