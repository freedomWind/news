using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hotfix.ResourceLoader
{
    /// <summary>
    /// 简化资源加载器 - 只负责加载和解析，不存储数据
    /// </summary>
    public class SimpleResourceLoader
    {
        private static SimpleResourceLoader _instance;
        public static SimpleResourceLoader Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SimpleResourceLoader();
                }
                return _instance;
            }
        }
        
        private Dictionary<string, IResourceLoader> _loaders = new Dictionary<string, IResourceLoader>();
        
        /// <summary>
        /// 注册加载器
        /// </summary>
        public void RegisterLoader(IResourceLoader loader)
        {
            if (loader != null)
            {
                _loaders[loader.ResourceType] = loader;
            }
        }
        
        /// <summary>
        /// 加载资源（不存储，直接通过回调返回）
        /// </summary>
        public void Load(string resourceType, string resourceName, Action<object> onSuccess, Action<string> onFailure, object userData = null)
        {
            if (!_loaders.TryGetValue(resourceType, out var loader))
            {
                onFailure?.Invoke($"未找到资源类型 '{resourceType}' 的加载器");
                return;
            }
            
            // 加载资源，成功后解析并直接通过回调返回，不存储
            loader.Load(
                resourceName,
                (rawAsset, parsedData) =>
                {
                    // 解析数据（Parse 方法可能返回解析后的数据）
                    // 如果 Parse 返回 null，使用 parsedData
                    object finalData = loader.Parse(parsedData, resourceName) ?? parsedData;
                    // 直接通过回调返回给业务层，不调用 Store
                    onSuccess?.Invoke(finalData);
                },
                onFailure,
                userData
            );
        }
    }
}

