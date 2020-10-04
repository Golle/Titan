namespace Titan.Windows
{
    public interface IWindowFactory
    {
        IWindow Create(int width, int height, string title);
    }
}
