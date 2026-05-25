using NewsFramework.Services.Media;

namespace NewsFramework.UI.Blocks
{
    public sealed class BlockRenderContext
    {
        public IMediaAssetService MediaAssetService { get; private set; }

        public static BlockRenderContext CreateDefault()
        {
            return CreateDefault(null);
        }

        public static BlockRenderContext CreateDefault(MediaServerConfig mediaServerConfig)
        {
            return new BlockRenderContext
            {
                MediaAssetService = MediaAssetServiceFactory.Create(mediaServerConfig)
            };
        }
    }
}
