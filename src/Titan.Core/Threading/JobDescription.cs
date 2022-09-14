namespace Titan.Core.Threading
{
    public readonly struct JobDescription
    {
        public readonly int Metadata;
        public readonly Action OnExecute;
        public readonly Action<int> OnComplete;
        public readonly bool AutoReset;
        public JobDescription(Action onExecute, Action<int> onComplete = null, bool autoReset = true, int metadata = 0)
        {
            Metadata = metadata;
            OnExecute = onExecute;
            OnComplete = onComplete;
            AutoReset = autoReset;
        }
    }
}
