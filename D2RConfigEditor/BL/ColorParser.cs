using System;
using System.Drawing;

namespace D2RConfigEditor.BL
{
    public interface IColorParser
    {
        Color ParseColorFromRGBString(string rgbString);
    }

    public class ColorParser : IColorParser
    {
        public Color ParseColorFromRGBString(string rgbString)
        {
            string[] values = rgbString.Replace(" ", "").Split(",");
            if (values.Length < 3 || values.Length > 4)
                return Color.FromArgb(0);
            if (values.Length == 3)
                try
                {
                    return Color.FromArgb(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]));
                }
                catch (ArgumentException)
                {
                    return Color.FromArgb(0);
                }
            if (values.Length == 4)
                try
                {
                    return Color.FromArgb(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]));
                }
                catch (ArgumentException)
                {
                    return Color.FromArgb(0);
                }
            return Color.FromArgb(0);
        }
    }
}
