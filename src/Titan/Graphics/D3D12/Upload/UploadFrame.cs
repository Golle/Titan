using System.Runtime.CompilerServices;
using Titan.Platform.Win32;
using Titan.Platform.Win32.D3D12;

namespace Titan.Graphics.D3D12.Upload;

internal unsafe struct UploadFrame
{
    public ComPtr<ID3D12CommandAllocator> Allocator;
    public ComPtr<ID3D12GraphicsCommandList> CommandList;
    public ulong FenceValue;
    public UploadState State;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WaitAndReset(HANDLE fenceEvent, ID3D12Fence* fence)
    {
        if (fence->GetCompletedValue() < FenceValue)
        {
            fence->SetEventOnCompletion(FenceValue, fenceEvent);
            Kernel32.WaitForSingleObject(fenceEvent, Win32Common.INFINITE);
        }
        State = UploadState.Available;
        FenceValue = 0;
    }
}