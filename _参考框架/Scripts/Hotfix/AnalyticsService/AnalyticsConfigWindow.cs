#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Hotfix.AnalyticsService
{
    public class AnalyticsConfigWindow : EditorWindow
    {
        private AnalyticsManager analyticsManager;
        private Vector2 scrollPosition;

        [MenuItem("Tools/Analytics Configuration")]
        public static void ShowWindow()
        {
            GetWindow<AnalyticsConfigWindow>("Analytics Config");
        }

        private void OnEnable()
        {
            // 查找场景中的AnalyticsManager实例
            analyticsManager = FindObjectOfType<AnalyticsManager>();
            if (analyticsManager == null)
            {
                // 如果不存在，则查找预制体或创建一个
                var managerPrefab = Resources.Load<GameObject>("AnalyticsManager");
                if (managerPrefab != null)
                {
                    var managerObj = Instantiate(managerPrefab);
                    analyticsManager = managerObj.GetComponent<AnalyticsManager>();
                }
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("Analytics Configuration", EditorStyles.boldLabel);
        
            if (analyticsManager == null)
            {
                EditorGUILayout.HelpBox("AnalyticsManager not found in scene. Please add one.", MessageType.Warning);
                if (GUILayout.Button("Create Analytics Manager"))
                {
                    CreateAnalyticsManager();
                }
                return;
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUI.BeginChangeCheck();

            // 服务提供商选择
            analyticsManager.currentProvider = (AnalyticsProvider)EditorGUILayout.EnumPopup(
                new GUIContent("Provider", "Select which analytics service to use"), 
                analyticsManager.currentProvider
            );

            // 自建服务配置
            if (analyticsManager.currentProvider == AnalyticsProvider.SelfHosted)
            {
                EditorGUILayout.Space();
                GUILayout.Label("Self-Hosted Settings", EditorStyles.boldLabel);
                analyticsManager.selfHostedEndpoint = EditorGUILayout.TextField(
                    new GUIContent("Endpoint", "The URL to send analytics data to"), 
                    analyticsManager.selfHostedEndpoint
                );
            }

            EditorGUILayout.Space();
            GUILayout.Label("General Settings", EditorStyles.boldLabel);

            // 启用状态
            analyticsManager.isEnabled = EditorGUILayout.Toggle(
                new GUIContent("Enabled", "Whether analytics collection is active"), 
                analyticsManager.isEnabled
            );

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(analyticsManager);
                if (analyticsManager.GetEnabled() != analyticsManager.isEnabled)
                {
                    analyticsManager.SetEnabled(analyticsManager.isEnabled);
                }
            }

            EditorGUILayout.Space();
            GUILayout.Label("Test Events", EditorStyles.boldLabel);

            if (GUILayout.Button("Test Track Event"))
            {
                var properties = new Dictionary<string, object>
                {
                    { "test_value", UnityEngine.Random.Range(1, 100) },
                    { "timestamp", DateTime.Now.ToString("HH:mm:ss") }
                };
                analyticsManager.Track("test_event", properties);
            }

            if (GUILayout.Button("Test Identify User"))
            {
                analyticsManager.Identify($"user_{UnityEngine.Random.Range(1000, 9999)}", 
                    new Dictionary<string, object> { { "level", UnityEngine.Random.Range(1, 10) } });
            }

            if (GUILayout.Button("Test Flush"))
            {
                analyticsManager.Flush();
            }

            EditorGUILayout.EndScrollView();
        }

        private void CreateAnalyticsManager()
        {
            GameObject managerObj = new GameObject("AnalyticsManager");
            analyticsManager = managerObj.AddComponent<AnalyticsManager>();
        
            // 保存到场景
            if (!EditorApplication.isPlaying)
            {
                Undo.RegisterCreatedObjectUndo(managerObj, "Create AnalyticsManager");
            }
        
            Selection.activeGameObject = managerObj;
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}
#endif