using System;
using GameFramework.Resource;
using UnityEditor;
using UnityEngine;

namespace Hotfix.ResourceLoader.Loaders
{
    public class AssetResourceLoader : IResourceLoader
    {
        public string ResourceType => "Asset";
        
        public void Load(string resourceName, Action<object, object> onSuccess, Action<string> onFailure, object userData = null)
        {
            LoadAssetSuccessCallback success = (name, asset, duration, data) =>
            {
                if (asset is UnityEngine.Object obj)
                {
                    onSuccess?.Invoke(asset, obj);
                }
                else
                {
                    onFailure?.Invoke($"资源类型错误，期望 Asset，实际: {asset?.GetType()}");
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