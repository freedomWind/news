using System;
using System.Collections.Generic;
using NewsFramework.Data.Blocks;
using NewsFramework.UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NewsFramework.UI.Blocks
{
    public sealed class BlockRenderer : MonoBehaviour
    {
        [SerializeField] private RectTransform contentRoot;

        private BlockRegistry registry;
        private Action<BlockActionData> onAction;
        private BlockRenderContext context;

        public void Initialize(
            RectTransform root,
            BlockRegistry blockRegistry,
            Action<BlockActionData> actionHandler,
            BlockRenderContext renderContext = null)
        {
            contentRoot = root;
            registry = blockRegistry;
            onAction = actionHandler;
            context = renderContext ?? BlockRenderContext.CreateDefault();
        }

        public void Render(PageData page)
        {
            Clear();

            if (page == null || page.blocks == null || contentRoot == null)
            {
                return;
            }

            if (registry == null)
            {
                registry = BlockRegistry.CreateDefault();
            }

            foreach (var block in page.blocks)
            {
                RenderBlock(block);
            }
        }

        public void Append(IEnumerable<BlockData> blocks)
        {
            if (blocks == null || contentRoot == null)
            {
                return;
            }

            if (registry == null)
            {
                registry = BlockRegistry.CreateDefault();
            }

            foreach (var block in blocks)
            {
                RenderBlock(block);
            }
        }

        public void Clear()
        {
            if (contentRoot == null)
            {
                return;
            }

            for (var i = contentRoot.childCount - 1; i >= 0; i--)
            {
                var child = contentRoot.GetChild(i);
                if (Application.isPlaying)
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }

        private void CreateMargin(float height)
        {
            if (height <= 0f || contentRoot == null)
            {
                return;
            }

            var margin = AppUIFactory.CreateRect("BlockMargin", contentRoot);
            AppUIFactory.AddLayoutElement(margin.gameObject, height);
        }

        private void RenderBlock(BlockData block)
        {
            if (block == null)
            {
                return;
            }

            CreateMargin(block.marginTop);
            var view = registry.Create(block, contentRoot);
            view.SetContext(context);
            view.Bind(block, onAction);
            CreateMargin(block.marginBottom);
        }
    }
}
