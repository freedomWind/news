using NewsFramework.Core;
using NewsFramework.Services.Content.Runtime;
using NewsFramework.Services.Media;
using NewsFramework.UI.Pages;
using UnityEngine;

namespace NewsFramework
{
    public sealed class AppBootstrap : MonoBehaviour
    {
        public static EventBus EventBus { get; private set; }
        public static ContentRuntimeServices Services { get; private set; }

        private static bool started;

        [SerializeField]
        private ContentRuntimeConfig contentRuntimeConfig = new ContentRuntimeConfig();

        [SerializeField]
        private MediaServerConfig mediaServerConfig = new MediaServerConfig();

        internal void SetConfig(ContentRuntimeConfig runtimeConfig, MediaServerConfig serverConfig)
        {
            contentRuntimeConfig = runtimeConfig ?? new ContentRuntimeConfig();
            mediaServerConfig = serverConfig ?? new MediaServerConfig();
        }

        private void Start()
        {
            if (started) return;
            started = true;

            EventBus = new EventBus();
            Services = ContentRuntimeServiceFactory.Create(contentRuntimeConfig);
            gameObject.AddComponent<HomePage>().Build(contentRuntimeConfig, mediaServerConfig);
        }
    }
}
