using System;

namespace Titan.Core.Threading
{
    public readonly struct JobDescription
    {
        public readonly Action OnExecute;
        public readonly Action OnComplete;
        public readonly bool AutoReset;
        public JobDescription(Action onExecute, Action onComplete = null, bool autoReset = true)
        {
            OnExecute = onExecute;
            OnComplete = onComplete;
            AutoReset = autoReset;
        }
    }
}
