namespace Tools.FontBuilder.BitmapFonts
{
    public class BitmapFont
    {
        public Info Info { get; set; }
        public CommonSettings Common { get; set; }
        public Page Page { get; set; }

        public BitmapChar[] Chars { get; set; }
        public byte[] Bitmap { get; set; }
        public Kerning[] Kernings { get; set; }
    }
}
