using Titan.Core.Logging;

namespace Titan.Windows.Win32
{
    internal class Win32WindowFactory : IWindowFactory
    {
        private readonly ILog _log;
        private readonly IWindowEventHandler _eventHandler;

        public Win32WindowFactory(ILog log, IWindowEventHandler eventHandler)
        {
            _log = log;
            _eventHandler = eventHandler;
        }

        public IWindow Create(int width, int height, string title)
        {
            _log.Debug("Creating Window: \"{0}\" Size: {1} x {2}", title, width, height);
            return new Win32Window(width, height, title, _eventHandler);
        }
    }
}
