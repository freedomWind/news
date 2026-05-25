using System;
using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using UnityGameFramework.Runtime;
using UnityEngine;

namespace Hotfix.ResourceLoader.Loaders
{
    /// <summary>
    /// 场景资源加载器
    /// </summary>
    public class SceneResourceLoader : IResourceLoader
    {
        public string ResourceType => "Scene";
        
        public void Load(string resourceName, Action<object, object> onSuccess, Action<string> onFailure, object userData = null)
        {
            // 订阅场景加载事件
            int eventId = resourceName.GetHashCode();
            
            void OnSceneLoadSuccess(object sender, GameEventArgs args)
            {
                var e = args as LoadSceneSuccessEventArgs;
                if (e.UserData is int code && code == eventId)
                {
                    EntityInst.Event.Unsubscribe(LoadSceneSuccessEventArgs.EventId, OnSceneLoadSuccess);
                    EntityInst.Event.Unsubscribe(LoadSceneFailureEventArgs.EventId, OnSceneLoadFailure);
                    EntityInst.Event.Unsubscribe(LoadSceneUpdateEventArgs.EventId, OnSceneLoading);
                    
                    onSuccess?.Invoke(e.SceneAssetName, e.SceneAssetName); // 场景资源作为解析后的数据
                }
            }
            
            void OnSceneLoadFailure(object sender, GameEventArgs args)
            {
                var e = args as LoadSceneFailureEventArgs;
                if (e.UserData is int code && code == eventId)
                {
                    EntityInst.Event.Unsubscribe(LoadSceneSuccessEventArgs.EventId, OnSceneLoadSuccess);
                    EntityInst.Event.Unsubscribe(LoadSceneFailureEventArgs.EventId, OnSceneLoadFailure);
                    EntityInst.Event.Unsubscribe(LoadSceneUpdateEventArgs.EventId, OnSceneLoading);
                    
                    onFailure?.Invoke(e.ErrorMessage);
                }
            }
            
            void OnSceneLoading(object sender, GameEventArgs args)
            {
                // 加载中，不做处理
            }
            
            EntityInst.Event.Subscribe(LoadSceneSuccessEventArgs.EventId, OnSceneLoadSuccess);
            EntityInst.Event.Subscribe(LoadSceneFailureEventArgs.EventId, OnSceneLoadFailure);
            EntityInst.Event.Subscribe(LoadSceneUpdateEventArgs.EventId, OnSceneLoading);
            
            // 开始加载场景
            EntityInst.Scene.LoadScene(resourceName, 100, eventId);
        }
        
        public object Parse(object rawAsset, string resourceName)
        {
            // 场景资源不需要额外解析
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
            // 卸载场景
            EntityInst.Scene.UnloadScene(resourceName);
        }
    }
}

