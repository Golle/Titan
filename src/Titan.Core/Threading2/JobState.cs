namespace Titan.Core.Threading2;

public static class JobState
{
    public const int Available = 0;
    public const int Claimed = 1;
    public const int Waiting = 2;
    public const int Running = 3;
    public const int Completed = 4;
}
