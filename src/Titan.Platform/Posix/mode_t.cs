namespace Titan.Platform.Posix;

[Flags]
public enum mode_t
{
    S_IRWXU = 0x0000700,    /* RWX mask for owner */
    S_IRUSR = 0x0000400,    /* R for owner */
    S_IWUSR = 0x0000200,    /* W for owner */
    S_IXUSR = 0x0000100,    /* X for owner */
    S_IRWXG = 0x0000070,    /* RWX mask for group */
    S_IRGRP = 0x0000040,    /* R for group */
    S_IWGRP = 0x0000020,    /* W for group */
    S_IXGRP = 0x0000010,    /* X for group */
    S_IRWXO = 0x0000007,    /* RWX mask for other */
    S_IROTH = 0x0000004,    /* R for other */
    S_IWOTH = 0x0000002,    /* W for other */
    S_IXOTH = 0x0000001,    /* X for other */
    S_ISUID = 0x0004000,    /* set user id on execution */
    S_ISGID = 0x0002000,    /* set group id on execution */
    S_ISVTX = 0x0001000,    /* save swapped text even after use */
}
