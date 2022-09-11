namespace Titan.FileSystem;

public struct FileHandle
{
    public nint Handle;
    public bool IsValid => Handle != 0;

    public static readonly FileHandle Invalid = default;

    public static implicit operator FileHandle(nint handle) => new() { Handle = handle };

    public static implicit operator FileHandle(nuint handle) => new() { Handle = (nint)handle };

    public static implicit operator nint(FileHandle handle) => handle.Handle;
    public static implicit operator nuint(FileHandle handle) => (nuint)handle.Handle;
}
