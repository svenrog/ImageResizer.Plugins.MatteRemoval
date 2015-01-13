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
            int amount = 0;
           
            if (!int.TryParse(state.settings["matte"], out amount))
                return null;

            if (amount > 100) return null;

            var settings = new Settings
            {
                Amount = (byte)amount
            };

            return settings;
        }

        private const int ByteSize = 4;

        private unsafe void ApplyMatteTransform(BitmapData bitmap, Settings settings)
        {
            float amount = settings.Amount * 0.01f;

            for (var y = 0; y < bitmap.Height; y++)
            {
                var row = (byte*)bitmap.Scan0 + (y * bitmap.Stride);

                for (var x = 0; x < bitmap.Width; x++)
                {
                    var p = x * ByteSize;
                    var a = row[p + 3];

                    //No need to process transparent pixels
                    if (a == 0) continue;

                    //No need to process opaque pixels
                    if (a == byte.MaxValue) continue;

                    //Declare colors when needed
                    var b = row[p];
                    var g = row[p + 1];
                    var r = row[p + 2];

                    var hsl = ColorExtensions.RgbToHsl(r, g, b);
                    var l = hsl.L * amount;
                    
                    if (l < 0) l = 0;
                    if (l > 1) l = 1;

                    var rgb = ColorExtensions.HslToRgb(hsl.H, hsl.S, l);

                    row[p]     = rgb.R;    //Blue  0-255
                    row[p + 1] = rgb.G;    //Green 0-255
                    row[p + 2] = rgb.B;    //Red   0-255
                }
            }
        }      
       
    }
}
