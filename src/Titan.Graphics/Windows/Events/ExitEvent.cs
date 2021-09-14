using Titan.Core.Messaging;

namespace Titan.Graphics.Windows.Events
{
    public readonly struct ExitEvent
    {
        public static readonly short Id = EventId<ExitEvent>.Value;

        public readonly int ExitCode;
        public ExitEvent(int exitCode = 0) => ExitCode = exitCode;
    }
}
