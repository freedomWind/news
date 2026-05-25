using NewsFramework.Services.Content.Runtime;
using NewsFramework.Services.Media;
using UnityEngine;

namespace NewsFramework
{
    public sealed class HomeDemoBootstrap : MonoBehaviour
    {
        private static bool autoStarted;

        [SerializeField]
        private ContentRuntimeConfig contentRuntimeConfig = new ContentRuntimeConfig();

        [SerializeField]
        private MediaServerConfig mediaServerConfig = new MediaServerConfig();

        public void Configure(ContentRuntimeConfig runtimeConfig, MediaServerConfig serverConfig = null)
        {
            contentRuntimeConfig = runtimeConfig ?? new ContentRuntimeConfig();
            mediaServerConfig = serverConfig ?? new MediaServerConfig();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AutoStart()
        {
            if (autoStarted || FindFirstObjectByType<HomeDemoBootstrap>() != null)
            {
                return;
            }

            autoStarted = true;
            new GameObject(nameof(HomeDemoBootstrap)).AddComponent<HomeDemoBootstrap>();
        }

        private void Start()
        {
            var bootstrap = gameObject.AddComponent<AppBootstrap>();
            bootstrap.SetConfig(contentRuntimeConfig, mediaServerConfig);
        }
    }
}
