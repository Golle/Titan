using System;
using System.Runtime.CompilerServices;
using Titan.GraphicsV2.D3D11;

namespace Titan.GraphicsV2.Rendering
{
    internal class RenderPass
    {
        private readonly Action<Context>[] _actions;

        public RenderPass(Action<Context>[] actions)
        {
            _actions = actions;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Execute(Context context)
        {
            foreach (var action in _actions)
            {
                action(context);
            }
        }
    }
}
