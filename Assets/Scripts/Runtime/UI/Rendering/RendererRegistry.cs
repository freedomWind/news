using System.Collections.Generic;
using UnityEngine;

namespace NewsFramework.UI.Rendering
{
    public sealed class RendererRegistry<TDescriptor>
    {
        private readonly Dictionary<string, TDescriptor> descriptors =
            new Dictionary<string, TDescriptor>();

        public void Register(string type, TDescriptor descriptor)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                Debug.LogWarning("Renderer type cannot be empty.");
                return;
            }

            descriptors[type] = descriptor;
        }

        public bool TryGet(string type, out TDescriptor descriptor)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                descriptor = default;
                return false;
            }

            return descriptors.TryGetValue(type, out descriptor);
        }
    }
}
