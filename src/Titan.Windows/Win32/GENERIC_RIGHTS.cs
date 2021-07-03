// ReSharper disable InconsistentNaming
namespace Titan.Windows.Win32
{
    public enum GENERIC_RIGHTS : uint
    {
        GENERIC_READ = (0x80000000),
        GENERIC_WRITE = (0x40000000),
        GENERIC_EXECUTE = (0x20000000),
        GENERIC_ALL = (0x10000000)
    }
}
