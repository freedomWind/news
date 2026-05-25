using System;
using UnityEngine;

namespace Hotfix.ResourceLoader
{
    /// <summary>
    /// 资源加载器接口 - 统一的资源加载、解析、存储接口
    /// </summary>
    public interface IResourceLoader
    {
        /// <summary>
        /// 资源类型标识（用于识别加载器）
        /// </summary>
        string ResourceType { get; }
        
        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="resourceName">资源名称</param>
        /// <param name="onSuccess">成功回调（资源对象，解析后的数据）</param>
        /// <param name="onFailure">失败回调</param>
        /// <param name="userData">用户数据</param>
        void Load(string resourceName, Action<object, object> onSuccess, Action<string> onFailure, object userData = null);
        
        /// <summary>
        /// 解析资源（将原始资源转换为业务数据）
        /// </summary>
        /// <param name="rawAsset">原始资源对象</param>
        /// <param name="resourceName">资源名称</param>
        /// <returns>解析后的业务数据</returns>
        object Parse(object rawAsset, string resourceName);
        
        /// <summary>
        /// 存储资源（V3版本不存储数据，此方法保留为空实现以兼容接口）
        /// </summary>
        /// <param name="parsedData">解析后的数据</param>
        /// <param name="resourceName">资源名称</param>
        void Store(object parsedData, string resourceName);
        
        /// <summary>
        /// 获取已存储的资源（V3版本不存储数据，此方法保留为空实现以兼容接口）
        /// </summary>
        /// <param name="resourceName">资源名称</param>
        /// <returns>始终返回null（V3版本不存储数据）</returns>
        object GetStored(string resourceName);
        
        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="resourceName">资源名称</param>
        void Unload(string resourceName);
    }
}

