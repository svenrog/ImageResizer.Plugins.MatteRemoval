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
            const double sector = 1.0 / 60.0;

            var r = l;
            var g = l;
            var b = l;

            double f, p, q, t;
            int j;

            if (s == 0) return new Rgb(r, g, b);

            h *= sector;
            j = (int)h;
            f = h - j;
            p = l * (1.0 - s);
            q = l * (1.0 - s * f);
            t = l * (1.0 - s * (1.0 - f));

            switch (j) 
            {
                case 0:
                    r = l; g = t; b = p; break;

                case 1:
                    r = q; g = l; b = p; break;

                case 2:
                    r = p; g = l; b = t; break;

                case 3:
                    r = p; g = q; b = l; break;

                case 4:
                    r = t; g = p; b = l; break;

                default:
                    r = l; g = p; b = q; break;
            }

            return new Rgb(r, g, b);
        }

        public static Hsl RgbToHsl(Rgb rgb)
        {
            return RgbToHsl(rgb.R, rgb.G, rgb.B);
        }

        public static Hsl RgbToHsl(double r, double g, double b) 
        {
            var h = 0.0;
            var s = 0.0;
            var l = 0.0;

            var max = Math.Max(r, Math.Max(g, b));
            var min = Math.Max(r, Math.Min(g, b));

            l = max;

            if (l == 0) return new Hsl(0, 0, 0);

            r /= max;
            g /= max;
            b /= max;

            max = Math.Max(r, Math.Max(g, b));
            min = Math.Max(r, Math.Min(g, b));

            s = (max - min);

            if (s == 0) return new Hsl(0, 0, 0);

            var si = 1 / s;

            r = (r - min) * s;
            g = (g - min) * s;
            b = (b - min) * s;

            max = Math.Max(r, Math.Max(g, b));

            if (max == r) 
            {
                h = 60.0 * (g - b);
                if (h < 0.0) h += 360.0;
            }
            else if (max == g) 
            {
                h = 180.0 * (b - r);
            }
            else 
            {
                h = 300.0 * (r - g);
            }

            return new Hsl(h, s, l);
        }

        

    }
}
