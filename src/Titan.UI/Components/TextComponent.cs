using System.Numerics;
using Titan.Core;
using Titan.Graphics.Loaders.Fonts;

namespace Titan.UI.Components
{

    public struct Text
    {
        public string Value;
    }

    public struct TextComponent
    {
        public Handle<Font> Font;
        public Handle<Text> Text;
        public Vector2 V;
    }
}
