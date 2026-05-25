namespace NewsFramework.Services.Media
{
    public static class MediaAssetServiceFactory
    {
        private static IMediaAssetService shared;

        public static IMediaAssetService CreateSharedDefault()
        {
            return CreateSharedDefault(null);
        }

        public static IMediaAssetService CreateSharedDefault(MediaServerConfig mediaServerConfig)
        {
            if (shared == null)
            {
                shared = new UnityMediaAssetService(new MemoryMediaAssetCache(), mediaServerConfig);
            }

            return shared;
        }

        public static IMediaAssetService Create(MediaServerConfig mediaServerConfig)
        {
            return new UnityMediaAssetService(new MemoryMediaAssetCache(), mediaServerConfig);
        }
    }
}
