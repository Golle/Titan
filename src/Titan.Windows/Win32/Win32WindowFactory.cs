using Titan.Core.Logging;

namespace Titan.Windows.Win32
{
    internal class Win32WindowFactory : IWindowFactory
    {
        private readonly ILog _log;

        public Win32WindowFactory(ILog log)
        {
            _log = log;
        }

        public IWindow Create(int width, int height, string title)
        {
            _log.Debug("Creating Window: \"{0}\" Size: {1} x {2}", title, width, height);
            return new Win32Window(width, height, title);
        }
    }
}
