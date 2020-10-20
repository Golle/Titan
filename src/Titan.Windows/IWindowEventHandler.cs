using Titan.Core.Logging;

namespace Titan.Windows
{
    internal interface IWindowEventHandler
    {
        void OnCreate();
        void OnClose();
    }

    internal class WindowEventHandler : IWindowEventHandler
    {
        public WindowEventHandler()
        {
            
        }

        public void OnCreate()
        {
            LOGGER.Debug("Window created");
        }

        public void OnClose()
        {
            LOGGER.Debug("Window closed");
        }
    }
}
