using System.Runtime.InteropServices;
using Titan.Platform.Win32;
using Titan.Platform.Win32.D3D12;

namespace Titan.Platform.DXC;

[Guid("7f61fc7d-950d-467f-b3e3-3c02fb49187c")]
public unsafe struct IDXCIncludeHandler : INativeGuid
{
    private void** _vtbl;
    public static Guid* Guid => IID.IID_IDXCIncludeHandler;
}
