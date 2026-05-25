using System.Collections.Generic;
using System;
using UnityEngine;

namespace Core.Game
{
    public class GameModuleComponent : UnityGameFramework.Runtime.GameFrameworkComponent
    {
        private Dictionary<Type, IGameModule> m_GameModules = new Dictionary<Type, IGameModule>();
        public static GameModuleComponent MM { get; private set; }

        private void Start()
        {
            m_GameModules.Add(typeof(GameRoomModule), IGameModule.NewModule<GameRoomModule>());
        }

        public static void Init()
        {
            var p = EntityInst.Base.transform.parent;
            MM = new GameObject("GameModuleComponent").AddComponent<GameModuleComponent>();
            var t = MM.transform;
            t.SetParent(p);
            t.localPosition = Vector3.zero;
            t.localScale = Vector3.one;
        }

        public T GetModule<T>() where T : IGameModule
        {
            m_GameModules.TryGetValue(typeof(T), out var module);
            if (module == null)
            {
                module = IGameModule.NewModule<T>();
                m_GameModules.Add(typeof(T), module);
            }
            return module as T;
        }
    }

    public abstract class IGameModule : MonoBehaviour
    {
        public static T NewModule<T>() where T : IGameModule
        {
            if(GameModuleComponent.MM == null)
                throw new System.Exception("请先初始化GameModuleComponent");
            var t = new GameObject(typeof(T).Name).AddComponent<T>();
            Transform transform1;
            (transform1 = t.transform).SetParent(GameModuleComponent.MM.transform);
            var tt = transform1;
            tt.localScale = Vector3.one;
            tt.localPosition = Vector3.zero;
            return t;
        }
    }
}