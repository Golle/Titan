namespace Titan.Platform.Posix;

/// <summary>
/// This is the struct called `stat`, but due to conflicts I've change the name to PosixStat.
/// </summary>
public unsafe struct PosixStat
{
    public ulong st_dev;
    public ulong st_ino;
    public ulong st_nlink;
    public uint st_mode;
    public uint st_uid;
    public uint st_gid;
    public uint __pad0;
    public ulong st_rdev;
    public ulong st_size;
    public ulong st_atime;
    public ulong st_atime_nsec;
    public ulong st_mtime;
    public ulong st_mtime_nsec;
    public ulong st_ctime;
    public ulong st_ctime_nsec;
    public ulong st_blksize;
    public long st_blocks;
    public fixed ulong __unused[3];
};
