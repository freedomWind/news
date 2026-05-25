using System;
using System.Collections.Generic;

namespace Hotfix.ResourceLoader
{
    /// <summary>
    /// 资源加载配置项 - 定义单个资源的加载配置
    /// </summary>
    public class ResourceLoadConfigItem
    {
        /// <summary>
        /// 资源类型（如：Scene, Binary, Json, ScriptableObject）
        /// </summary>
        public string ResourceType { get; set; }
        
        /// <summary>
        /// 资源名称
        /// </summary>
        public string ResourceName { get; set; }
        
        /// <summary>
        /// 加载成功后的处理逻辑
        /// </summary>
        public Action<object> OnLoadSuccess { get; set; }
        
        /// <summary>
        /// 加载失败后的处理逻辑（可选）
        /// </summary>
        public Action<string> OnLoadFailure { get; set; }
        
        /// <summary>
        /// 用户数据（可选）
        /// </summary>
        public object UserData { get; set; }
        
        public ResourceLoadConfigItem(string resourceType, string resourceName, Action<object> onLoadSuccess, Action<string> onLoadFailure = null)
        {
            ResourceType = resourceType;
            ResourceName = resourceName;
            OnLoadSuccess = onLoadSuccess;
            OnLoadFailure = onLoadFailure;
        }
    }
    
    /// <summary>
    /// 资源加载配置 - 定义所有需要加载的资源
    /// </summary>
    public class ResourceLoadConfig
    {
        private List<ResourceLoadConfigItem> _items = new List<ResourceLoadConfigItem>();
        
        /// <summary>
        /// 添加加载项
        /// </summary>
        public ResourceLoadConfig Add(string resourceType, string resourceName, Action<object> onLoadSuccess, Action<string> onLoadFailure = null)
        {
            _items.Add(new ResourceLoadConfigItem(resourceType, resourceName, onLoadSuccess, onLoadFailure));
            return this;
        }
        
        /// <summary>
        /// 添加加载项（泛型版本，自动类型转换）
        /// </summary>
        public ResourceLoadConfig Add<T>(string resourceType, string resourceName, Action<T> onLoadSuccess, Action<string> onLoadFailure = null) where T : class
        {
            _items.Add(new ResourceLoadConfigItem(resourceType, resourceName, 
                (data) => 
                {
                    if (data is T typedData)
                    {
                        onLoadSuccess?.Invoke(typedData);
                    }
                    else
                    {
                        onLoadFailure?.Invoke($"数据类型不匹配: 期望 {typeof(T).Name}，实际 {data?.GetType().Name}");
                    }
                },
                onLoadFailure));
            return this;
        }
        
        /// <summary>
        /// 批量添加资源（格式：类型:名称#类型:名称）
        /// </summary>
        public ResourceLoadConfig AddBatch(string resourceList, Action<string, object> onEachLoadSuccess = null, Action<string, string> onEachLoadFailure = null)
        {
            if (string.IsNullOrEmpty(resourceList))
            {
                return this;
            }
            
            string[] items = resourceList.Split('#');
            foreach (var item in items)
            {
                string[] parts = item.Split(':');
                if (parts.Length == 2)
                {
                    string type = parts[0].Trim();
                    string name = parts[1].Trim();
                    
                    Add(type, name, 
                        (data) => onEachLoadSuccess?.Invoke(name, data),
                        (error) => onEachLoadFailure?.Invoke(name, error));
                }
            }
            
            return this;
        }
        
        /// <summary>
        /// 获取所有配置项
        /// </summary>
        public List<ResourceLoadConfigItem> GetItems()
        {
            return _items;
        }
        
        /// <summary>
        /// 清空配置
        /// </summary>
        public void Clear()
        {
            _items.Clear();
        }
    }
}

