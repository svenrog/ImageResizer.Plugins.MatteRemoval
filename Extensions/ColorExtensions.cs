using System;
using System.Drawing;
using ImageResizer.Plugins.MatteRemoval.Primitives;

namespace ImageResizer.Plugins.MatteRemoval.Extensions
{
    public static class ColorExtensions
    {
        public static Rgb HslToRgb(Hsl hsl)
        {
            return HslToRgb(hsl.H, hsl.S, hsl.L);
        }

        public static Rgb HslToRgb(double h, double s, double l)
        {
            const double sector = 1 / 60;

            if (s == 0) return new Rgb(l * 255.0, l * 255.0, l * 255.0);

            var r = l;
            var g = l;
            var b = l;

            double sp = h * sector;
            int sn = (int)sp;
            
            double fs = sp - sn;

            double p = l * (1.0 - s);
            double q = l * (1.0 - (s * fs));
            double t = l * (1.0 - (s * (1.0 - fs)));

            switch (sn)
            {
                case 0:
                    r = b;
                    g = t;
                    b = p;
                        break;
                case 1:
                    r = q;
                    g = b;
                    b = p;
                        break;
                case 2:
                    r = p;
                    g = b;
                    b = t;
                        break;
                case 3:
                    r = p;
                    g = q;
                    b = l;
                        break;
                case 4:
                    r = t;
                    g = p;
                    b = l;
                        break;
                case 5:
                    r = b;
                    g = p;
                    b = q;
                        break;
            }

            return new Rgb(r * 255.0, g * 255.0, b * 255.0);
        }

        public static Hsl RgbToHsl(Rgb rgb)
        {
            return RgbToHsl(rgb.R, rgb.G, rgb.B);
        }

        public static Hsl RgbToHsl(double r, double g, double b) 
        {
            const double inv = 1 / 255.0;

            r *= inv;
            g *= inv;
            b *= inv;

            var max = Math.Max(r, Math.Max(g, b));
            var min = Math.Min(r, Math.Min(g, b));

            var h = 0.0;

            if (max == min)
            {
                h = 0.0;
            }
            if (max == r && g >= b)
            {
                h = 60 * (g - b) / (max - min);
            }
            else if (max == r && g < b)
            {
                h = 60 * (g - b) / (max - min) + 360;
            }
            else if (max == g)
            {
                h = 60 * (b - r) / (max - min) + 120;
            }
            else if (max == b)
            {
                h = 60 * (r - g) / (max - min) + 240;
            }

            var s = (max == 0) ? 0.0 : (1.0 - (min / max));

            return new Hsl(h, s, max);
        }      
    }
}
