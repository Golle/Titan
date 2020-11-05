using System.Runtime.CompilerServices;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.Common;

namespace Titan.Graphics.D3D11
{
    public unsafe class DeferredContext : ImmediateContext
    {
        public DeferredContext(IGraphicsDevice device)
        {
            CheckAndThrow(device.Ptr->CreateDeferredContext(0, Context.GetAddressOf()), "CreateDeferredContext");
        }

        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CommandList FinishCommandList(bool restoreDeferredContextState = false)
        {
            ID3D11CommandList* pCommandList;
            CheckAndThrow(Context.Get()->FinishCommandList(restoreDeferredContextState ? 1 : 0, &pCommandList), "FinishCommandList");
            return new CommandList(pCommandList);
        }

        
    }
}
