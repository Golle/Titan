namespace Titan.Windows.Win32;

public unsafe struct SECURITY_ATTRIBUTES
{
    public DWORD nLength;
    public void* lpSecurityDescriptor;
    public int bInheritHandle;
}
