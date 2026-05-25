using TMPro;
using UnityEngine;

namespace NewsFramework.UI.Theme
{
    public static class AppTheme
    {
        public static readonly Color PageBackground = Rgb(246, 245, 241);
        public static readonly Color Surface = Rgb(255, 255, 252);
        public static readonly Color SurfaceMuted = Rgb(244, 243, 239);
        public static readonly Color Hairline = Rgb(226, 223, 216);
        public static readonly Color PrimaryText = Rgb(42, 35, 25);
        public static readonly Color SecondaryText = Rgb(125, 115, 99);
        public static readonly Color Accent = Rgb(154, 52, 48);
        public static readonly Color DarkBar = Rgb(47, 37, 23);
        public static readonly Color TabInactive = Rgb(126, 119, 109);

        public const float ScreenWidth = 375f;
        public const float ScreenHeight = 812f;
        public const float PagePadding = 16f;
        public const float BottomBarHeight = 64f;
        public const float TopBarHeight = 56f;
        public const float CardRadius = 8f;

        private static TMP_FontAsset fontAsset;
        private static TMP_FontAsset fontAssetBold;

        public static TMP_FontAsset FontAsset
        {
            get
            {
                if (fontAsset != null)
                {
                    return fontAsset;
                }

                fontAsset = Resources.Load<TMP_FontAsset>("Font/SourceHanSansSC-Normal SDF");
                if (fontAsset != null)
                {
                    return fontAsset;
                }

                fontAsset = TMP_Settings.defaultFontAsset;
                if (fontAsset != null)
                {
                    return fontAsset;
                }

                fontAsset = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
                return fontAsset;
            }
        }

        public static TMP_FontAsset FontAssetBold
        {
            get
            {
                if (fontAssetBold != null)
                {
                    return fontAssetBold;
                }

                fontAssetBold = Resources.Load<TMP_FontAsset>("Font/SourceHanSansSC-Bold SDF");
                if (fontAssetBold != null)
                {
                    return fontAssetBold;
                }

                return FontAsset;
            }
        }

        public static Color Rgb(byte r, byte g, byte b)
        {
            return new Color32(r, g, b, 255);
        }

        public static Color Rgba(byte r, byte g, byte b, byte a)
        {
            return new Color32(r, g, b, a);
        }
    }
}
