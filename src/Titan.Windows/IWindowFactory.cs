namespace Titan.Windows
{
    public interface IWindowFactory
    {
        IWindow Create(uint width, uint height, string title);
    }
}
