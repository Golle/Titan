using System.Diagnostics;
using Titan.Windows;
using Titan.Windows.D3D12;

namespace Titan.Graphics.D3D12Take2;

internal unsafe struct D3D12CommandList
{
    private ComPtr<ID3D12GraphicsCommandList> _commandList;
    public static D3D12CommandList Create(ID3D12Device4* device, D3D12_COMMAND_LIST_TYPE type)
    {
        D3D12CommandList commandList = default;
        var hr = device->CreateCommandList1(0, type, D3D12_COMMAND_LIST_FLAGS.D3D12_COMMAND_LIST_FLAG_NONE, typeof(ID3D12GraphicsCommandList).GUID, (void**)commandList._commandList.GetAddressOf());
        Debug.Assert(Common.SUCCEEDED(hr), $"Failed to create {nameof(ID3D12GraphicsCommandList)} with HRESULT {hr}");
        return commandList;
    }

    public void Reset(ID3D12CommandAllocator* allocator)
    {
        var hr = _commandList.Get()->Reset(allocator, null /*Do we want initial pipeline state?*/);
        Debug.Assert(Common.SUCCEEDED(hr), $"Reset command list failed with HRESULT {hr}");
    }

    public void Releae()
    {
        //NOTE(Jens): needs a fence?

        _commandList.Reset();
    }
}
