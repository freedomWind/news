using System;
using NewsFramework.Data.Blocks;
using NewsFramework.Data.Features;
using UnityEngine;

namespace NewsFramework.UI.Features
{
    public abstract class FeatureSectionViewBase : MonoBehaviour, IFeatureSectionView
    {
        protected FeatureSectionData Data { get; private set; }
        protected Action<BlockActionData> OnAction { get; private set; }

        public void Bind(FeatureSectionData data, Action<BlockActionData> onAction)
        {
            Data = data;
            OnAction = onAction;
            OnBind(data);
        }

        protected abstract void OnBind(FeatureSectionData data);

        protected void TriggerSectionAction()
        {
            TriggerAction(Data != null ? Data.action : null);
        }

        protected void TriggerItemAction(FeatureItemData item)
        {
            TriggerAction(item != null ? item.action : null);
        }

        private void TriggerAction(BlockActionData action)
        {
            if (action == null || string.IsNullOrEmpty(action.type) || action.type == "none")
            {
                return;
            }

            OnAction?.Invoke(action);
        }
    }
}
