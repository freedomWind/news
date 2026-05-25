using NewsFramework;
using NewsFramework.Services.Content.Http;
using NewsFramework.Services.Content.Runtime;
using NewsFramework.Services.Media;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NewsFramework.Editor
{
    public static class HomeDemoSceneBuilder
    {
        private const string ScenePath = "Assets/App/Scenes/HomeDemo.unity";
        private const string ContentApiSmokeScenePath = "Assets/App/Scenes/HomeDemoContentApi.unity";
        private const string DefaultContentApiBaseUrl = "http://localhost:5235";
        private const string DefaultMediaApiBaseUrl = "http://localhost:5234";

        [MenuItem("NewsFramework/Build Home Demo Scene")]
        public static void BuildHomeDemoScene()
        {
            BuildScene(ScenePath, "HomeDemo", new ContentRuntimeConfig(), new MediaServerConfig());
        }

        [MenuItem("NewsFramework/Build Content.Api Smoke Scene")]
        public static void BuildContentApiSmokeScene()
        {
            BuildScene(
                ContentApiSmokeScenePath,
                "HomeDemoContentApi",
                CreateContentApiRuntimeConfig(),
                new MediaServerConfig
                {
                    baseUrl = DefaultMediaApiBaseUrl
                });
        }

        private static void BuildScene(
            string scenePath,
            string sceneName,
            ContentRuntimeConfig runtimeConfig,
            MediaServerConfig mediaServerConfig)
        {
            EnsureSceneDirectory(scenePath);

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = sceneName;

            var bootstrap = new GameObject("HomeDemoBootstrap");
            bootstrap.AddComponent<HomeDemoBootstrap>().Configure(runtimeConfig, mediaServerConfig);

            EditorSceneManager.SaveScene(scene, scenePath);
            AssetDatabase.Refresh();
            Debug.Log("Home demo scene saved to " + scenePath);
        }

        private static void EnsureSceneDirectory(string scenePath)
        {
            var directory = System.IO.Path.GetDirectoryName(scenePath)?.Replace("\\", "/");
            if (!string.IsNullOrEmpty(directory) && !AssetDatabase.IsValidFolder(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
                AssetDatabase.Refresh();
            }
        }

        private static ContentRuntimeConfig CreateContentApiRuntimeConfig()
        {
            return new ContentRuntimeConfig
            {
                mode = ContentRuntimeMode.Http,
                feedId = "home",
                feedPageSize = 6,
                httpConfig = new ContentHttpConfig
                {
                    baseUrl = DefaultContentApiBaseUrl,
                    timeoutSeconds = 10,
                    maxRetryCount = 1
                }
            };
        }
    }
}
