using System;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

namespace Titan.Graphics.D3D11
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Color
    {
        public float R;
        public float G;
        public float B;
        public float A;

        public Color(float r, float g, float b, float a = 1f)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public static readonly Color Red = new(1f, 0, 0);
        public static readonly Color Green = new(0, 1f, 0);
        public static readonly Color Blue = new(0, 0, 1f);
        public static readonly Color White = new(1f, 1f, 1f);
        public static readonly Color Black = new(0f, 0, 0);
        public static readonly Color Zero = new(0f, 0, 0, 0);

        public static Color Parse(string value)
        {
            if (!IsValid(value))
            {
                throw new FormatException("The color must be of format '#RRGGBBAA'");
            }

            // This could be done with parsing the entire string and then using bitshift to select each color and divide by 255. But keep it simple now since it wont be used in a critical section
            return new Color
            {
                R = int.Parse(value.AsSpan(1, 2), NumberStyles.HexNumber) / 255f,
                G = int.Parse(value.AsSpan(3, 2), NumberStyles.HexNumber) / 255f,
                B = int.Parse(value.AsSpan(5, 2), NumberStyles.HexNumber) / 255f,
                A = value.Length == 9 ? int.Parse(value.AsSpan(7, 2), NumberStyles.HexNumber) / 255f : 0f
            };
        }

        public static bool TryParse(string value, out Color color)
        {
            if (!IsValid(value))
            {
                color = Zero;
                return false;
            }

            color = new Color
            {
                R = int.Parse(value.AsSpan(1, 2), NumberStyles.HexNumber) / 255f,
                G = int.Parse(value.AsSpan(3, 2), NumberStyles.HexNumber) / 255f,
                B = int.Parse(value.AsSpan(5, 2), NumberStyles.HexNumber) / 255f,
                A = value.Length == 9 ? int.Parse(value.AsSpan(7, 2), NumberStyles.HexNumber) / 255f : 0f
            };
            return true;
        }

        private static bool IsValid(string value)
        {
            if (value.Length != 9 && value.Length != 7)
            {
                return false;
            }

            if (value.Count(v => v == '#') != 1 || value[0] != '#')
            {
                return false;
            }

            foreach (var c in value.AsSpan(1, value.Length-1))
            {
                var val = (int) c;
                // Validate each character, should be within 0-9, a-f and A-F.
                if (val < 30 || (val > 57 && val < 65) || (val > 70 && val < 97) || val > 102)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
