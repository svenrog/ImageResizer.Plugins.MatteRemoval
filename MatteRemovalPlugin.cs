using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using ImageResizer.Resizing;
using ImageResizer.Configuration;
using ImageResizer.ExtensionMethods;
using ImageResizer.Plugins.MatteRemoval.Primitives;
using ImageResizer.Plugins.MatteRemoval.Extensions;

namespace ImageResizer.Plugins.MatteRemoval
{
    public class MatteRemovalPlugin : BuilderExtension, IPlugin, IQuerystringPlugin
    {
        public IPlugin Install(Config c)
        {
            c.Plugins.add_plugin(this);
            return this;
        }

        public bool Uninstall(Config c)
        {
            c.Plugins.remove_plugin(this);
            return true;
        }

        public IEnumerable<string> GetSupportedQuerystringKeys()
        {
            return new[]
            {
                "matte"
            };
        }

        protected override RequestedAction PostRenderImage(ImageState state)
        {
            if (state.destBitmap == null)
                return RequestedAction.None;

            var settings = GetSettings(state);

            if (settings == null)
                return RequestedAction.None;

            Bitmap bitmap = null;

            try
            {
                bitmap = state.destBitmap;

                var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                ApplyMatteTransform(data, settings);

                bitmap.UnlockBits(data);
            }
            finally
            {
                if (bitmap != null & bitmap != state.destBitmap)
                    bitmap.Dispose();
            }

            return RequestedAction.None;
        }

        protected Settings GetSettings(ImageState state)
        {
            int amount = NameValueCollectionExtensions.Get(state.settings, "matte", new int?(0)) ?? 0;

            if (amount == 0) return null;
            if (amount > 100) return null;
            if (amount < -100) return null;
            
            var settings = new Settings
            {
                Amount = amount
            };

            return settings;
        }

        private unsafe void ApplyMatteTransform(BitmapData bitmap, Settings settings)
        {
            const int byteSize = 4;
            const double invByte = 1.0 / byte.MaxValue;

            float amount = settings.Amount * 0.01f;

            for (var y = 0; y < bitmap.Height; y++)
            {
                var row = (byte*)bitmap.Scan0 + (y * bitmap.Stride);

                for (var x = 0; x < bitmap.Width; x++)
                {
                    var p = x * byteSize;
                    var a = row[p + 3];

                    if (a == byte.MinValue) continue;
                    if (a == byte.MaxValue) continue;

                    var b = row[p];
                    var g = row[p + 1];
                    var r = row[p + 2];

                    var hsl = ColorExtensions.RgbToHsl(r, g, b);

                    var z = amount * (1.0 - a * invByte);
                    var l = hsl.L + z;

                    if (l < 0.0) l = 0.0;
                    if (l > 1.0) l = 1.0;

                    var rgb = ColorExtensions.HslToRgb(hsl.H, hsl.S, l);

                    row[p]     = rgb.B;    //Blue  0-255
                    row[p + 1] = rgb.G;    //Green 0-255
                    row[p + 2] = rgb.R;    //Red   0-255
                }
            }
        }      
       
    }
}
