namespace Tools.FontBuilder.BitmapFonts
{
    public struct Info {
        public string Name { get; set; }
        public int Size { get; set; }
        public bool Bold { get; set; }
        public bool Italic { get; set; }
        public string Charset { get; set; }
        public int Unicode { get; set; }
        public int StretchH { get; set; }
        public int Smooth { get; set; }
        public int AA { get; set; }
        public Padding Padding { get; set; }
        public Spacing Spacing { get; set; }
    }
}