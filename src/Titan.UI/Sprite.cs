using Titan.Graphics;
using Titan.Graphics.Loaders.Atlas;

namespace Titan.UI
{
    public class Sprite
    {
        public string Identifier { get; set; }
        public int Index { get; set; }
        public Margins Margins { get; set; }
        public Color Color { get; set; } = Color.White;
    }
}
