using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Plugins.MatteRemoval.Primitives
{
    public struct Rgb : IEquatable<Rgb>
    {
        private const int HashPrimeOne = 5;
        private const int HashPrimeTwo = 23;

        public readonly byte R;
        public readonly byte G;
        public readonly byte B;

        public Rgb(double r, double g, double b)
        {
            const double shift = 0.5;

            R = (byte)(r + shift);
            G = (byte)(g + shift);
            B = (byte)(b + shift);
        }

        public Rgb(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public bool Equals(Rgb other)
        {
            return other.R == R &&
                   other.G == G &&
                   other.B == B;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Rgb)) return false;

            var other = (Rgb)obj;

            return other.R == R &&
                   other.G == G &&
                   other.B == B;
        }

        public override int GetHashCode()
        {
            var hash = HashPrimeOne;

            hash *= HashPrimeTwo + R;
            hash *= HashPrimeTwo + G;
            hash *= HashPrimeTwo + B;

            return hash;
        }
    }
}
