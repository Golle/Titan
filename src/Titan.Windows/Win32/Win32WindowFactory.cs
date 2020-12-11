namespace Titan.Windows.Win32
{
    internal class Win32WindowFactory : IWindowFactory
    {
        private readonly IWindowEventHandler _eventHandler;
        public Win32WindowFactory(IWindowEventHandler eventHandler)
        {
            _eventHandler = eventHandler;
        }

        public IWindow Create(uint width, uint height, string title)
        {
            return new Win32Window((int)width, (int)height, title, _eventHandler);
        }
    }
}
