namespace Tools.FontBuilder.BitmapFonts
{
    public static class BitmapFontParser
    {
        public static BitmapFont ReadFromFile(string path)
        {
            var info = new Info();
            var common = new CommonSettings();
            var page = new Page();
            BitmapChar[] chars = null;
            Kerning[] kernings = null;
            var charIndex = 0;
            var kerningIndex = 0;
            using var file = File.OpenText(path);
            
            string line;
            while((line = file.ReadLine()) != null)
            {
                var values = line.SplitQuotedStrings(' ');
                if (values.Length == 0)
                {
                    continue;
                }

                var valueSpan = new ReadOnlySpan<string>(values, 1, values.Length - 1);
                switch (values[0])
                {
                    case "info":
                        ParseInfo(valueSpan, ref info);
                        break;
                    case "common":
                        ParseCommon(valueSpan, ref common);
                        break;
                    case "page":
                        ParsePage(valueSpan, ref page);
                        break;
                    case "chars":
                        chars = new BitmapChar[int.Parse(valueSpan[0].Split("=")[1])];
                        break;
                    case "kernings":
                        kernings = new Kerning[int.Parse(valueSpan[0].Split("=")[1])];
                        break;
                    case "char":
                        if (chars == null)
                        {
                            throw new InvalidOperationException("Chars has not been initialized, corrupt file");
                        }
                        ParseChar(valueSpan, ref chars[charIndex++]);
                        break;
                    case "kerning":
                        if (kernings == null)
                        {
                            throw new InvalidOperationException("Kernings has not been initialized, corrupt file");
                        }
                        ParseKerning(valueSpan, ref kernings[kerningIndex++]);
                        break;
                }
            }

            // Only supports a single bitmap
            return new BitmapFont
            {
                Info = info,
                Common = common,
                Chars = chars,
                Page = page,
                Kernings = kernings,
                Bitmap = File.ReadAllBytes(Path.Combine(Path.GetDirectoryName(path), page.File))
            };

            static void ParseKerning(ReadOnlySpan<string> values, ref Kerning kerning)
            {
                foreach (var valuePair in values)
                {
                    var splitValues = valuePair.Split("=", StringSplitOptions.TrimEntries);
                    var key = splitValues[0];
                    var value = splitValues[1];
                    switch (key)
                    {
                        case "first": kerning.First = (char)int.Parse(value); break;
                        case "second": kerning.Second = (char)int.Parse(value); break;
                        case "amount": kerning.Amount = int.Parse(value); break;
                    }
                }
            }

            static void ParseChar(ReadOnlySpan<string> values, ref BitmapChar character)
            {
                foreach (var valuePair in values)
                {
                    var splitValues = valuePair.Split("=", StringSplitOptions.TrimEntries);
                    var key = splitValues[0];
                    var value = splitValues[1];
                    switch (key)
                    {
                        case "id": character.Id = int.Parse(value); break;
                        case "x": character.X = int.Parse(value); break;
                        case "y": character.Y = int.Parse(value); break;
                        case "width": character.Width= int.Parse(value); break;
                        case "height": character.Height= int.Parse(value); break;
                        case "xoffset": character.XOffset = int.Parse(value); break;
                        case "yoffset": character.YOffset= int.Parse(value); break;
                        case "xadvance": character.XAdvance = int.Parse(value); break;
                        case "page": character.Page= int.Parse(value); break;
                        case "chnl": character.Channel = int.Parse(value); break;
                    }
                }
            }

            static void ParsePage(ReadOnlySpan<string> values, ref Page page)
            {
                foreach (var valuePair in values)
                {
                    var splitValues = valuePair.Split("=", StringSplitOptions.TrimEntries);
                    var key = splitValues[0];
                    var value = splitValues[1];
                    switch (key)
                    {
                        case "id": page.Id = int.Parse(value); break;
                        case "file": page.File = value.Trim('"'); break;
                    }
                }
            }

            static void ParseCommon(ReadOnlySpan<string> values, ref CommonSettings common)
            {
                foreach (var valuePair in values)
                {
                    var splitValues = valuePair.Split("=", StringSplitOptions.TrimEntries);
                    var key = splitValues[0];
                    var value = splitValues[1];
                    switch (key)
                    {
                        case "lineHeight": common.LineHeight = int.Parse(value); break;
                        case "base": common.Base = int.Parse(value); break;
                        case "scaleW": common.ScaleW = int.Parse(value); break;
                        case "scaleH": common.ScaleH = int.Parse(value); break;
                        case "pages": common.Pages = int.Parse(value); break;
                        case "packed": common.Packed = int.Parse(value); break;
                    }
                }
            }

            static void ParseInfo(ReadOnlySpan<string> values, ref Info info)
            {
                foreach (var valuePair in values)
                {
                    var splitValues = valuePair.Split("=", StringSplitOptions.TrimEntries);

                    var key = splitValues[0];
                    var value = splitValues[1];
                    switch (key)
                    {
                        case "face":
                            info.Name = value; 
                            break;
                        case "size":
                            info.Size = int.Parse(value);
                            break;
                        case "bold":
                            info.Bold = int.Parse(value) == 1;
                            break;
                        case "italic":
                            info.Italic = int.Parse(value) == 1;
                            break;
                        case "charset":
                            info.Charset = value;
                            break;
                        case "unicode":
                            info.Unicode = int.Parse(value);
                            break;
                        case "stretchH":
                            info.StretchH = int.Parse(value);
                            break;
                        case "smooth":
                            info.Smooth = int.Parse(value);
                            break;
                        case "aa":
                            info.AA = int.Parse(value);
                            break;
                        case "padding":
                            var padding = value.Split(",");
                            info.Padding = new Padding
                            {
                                Top = int.Parse(padding[0]),
                                Bottom = int.Parse(padding[1]),
                                Left = int.Parse(padding[2]),
                                Right = int.Parse(padding[3])
                            };
                            break;
                        case "spacing":
                            var spacing = value.Split(",");
                            info.Spacing = new Spacing
                            {
                                Horizontal = int.Parse(spacing[0]),
                                Vertical = int.Parse(spacing[1])
                            };
                            break;
                    }
                }
            }
        }


        private static string[] SplitQuotedStrings(this string str, char delimiter)
        {
            // this is very slow :) not sure it matters though since it will be run once.
            if (str.IndexOf('"') != -1)
            {
                var parts = new List<string>();
                var partStart = 0;
                do
                {
                    var quoteStart = str.IndexOf('"', partStart);
                    var quoteEnd = str.IndexOf('"', quoteStart + 1);
                    var partEnd = str.IndexOf(delimiter, partStart);

                    if (partEnd == -1)
                    {
                        partEnd = str.Length;
                    }

                    // part has quotes, find the delimiter outside of the quote
                    if (partEnd < quoteEnd && partEnd > quoteStart)
                    {
                        partEnd = str.IndexOf(delimiter, quoteEnd + 1);
                    }

                    parts.Add(str.Substring(partStart, partEnd - partStart).Trim('\"'));
                    partStart = partEnd + 1;

                } while (partStart < str.Length - 1);
                return parts.ToArray();
            }
            // no quotes
            return str.Split(delimiter.ToString(), StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
