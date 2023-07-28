using System.Drawing;

namespace MT4API
{
    class MT4APIColorConverter
    {
        public static Color ConvertFromMT4Color(int color)
        {
            return Color.FromArgb((byte)(color), (byte)(color >> 8), (byte)(color >> 16));
        }

        public static int ConvertToMT4Color(Color? color)
        {
            return color == null || color == Color.Empty ? 0xffffff : (Color.FromArgb(color.Value.B, color.Value.G, color.Value.R).ToArgb() & 0xffffff);
        }
    }
}
