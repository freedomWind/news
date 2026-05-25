using NewsFramework.Data.Features;
using NewsFramework.UI.Base;
using NewsFramework.UI.Theme;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.Features
{
    public sealed class ProfileHeaderSectionView : FeatureSectionViewBase
    {
        private TextMeshProUGUI avatarLabel;
        private TextMeshProUGUI nameLabel;
        private TextMeshProUGUI titleLabel;

        public static FeatureSectionViewBase Create(Transform parent)
        {
            var root = AppUIFactory.CreateRect("ProfileHeader", parent);
            var view = root.gameObject.AddComponent<ProfileHeaderSectionView>();
            AppUIFactory.AddLayoutElement(root.gameObject, 96f);
            AppUIFactory.AddHorizontalLayout(root.gameObject, 14f, new RectOffset(4, 4, 10, 10), TextAnchor.MiddleLeft);

            var avatar = AppUIFactory.CreateImage("Avatar", root, AppTheme.Surface);
            AppUIFactory.AddLayoutElement(avatar.gameObject, 72f, 72f);
            view.avatarLabel = AppUIFactory.CreateText("AvatarLabel", avatar.transform, string.Empty, 26f, AppTheme.Accent, FontStyles.Bold, TextAlignmentOptions.Center);
            AppUIFactory.Stretch(view.avatarLabel.rectTransform);

            var column = AppUIFactory.CreateRect("TextColumn", root);
            var columnLayout = AppUIFactory.AddLayoutElement(column.gameObject, 72f);
            columnLayout.flexibleWidth = 1f;
            AppUIFactory.AddVerticalLayout(column.gameObject, 6f, new RectOffset(0, 0, 8, 0), TextAnchor.MiddleLeft);

            view.nameLabel = AppUIFactory.CreateText("Name", column, string.Empty, 26f, AppTheme.PrimaryText, FontStyles.Bold);
            AppUIFactory.AddLayoutElement(view.nameLabel.gameObject, 34f);
            view.titleLabel = AppUIFactory.CreateText("Title", column, string.Empty, 13f, AppTheme.Accent, FontStyles.Bold);
            AppUIFactory.AddLayoutElement(view.titleLabel.gameObject, 22f);
            return view;
        }

        protected override void OnBind(FeatureSectionData data)
        {
            var item = data.items != null && data.items.Count > 0 ? data.items[0] : null;
            avatarLabel.text = item != null && !string.IsNullOrEmpty(item.icon) ? item.icon : "棋";
            nameLabel.text = item != null ? item.title : data.title;
            titleLabel.text = item != null ? item.badge : data.subtitle;
        }
    }
}
