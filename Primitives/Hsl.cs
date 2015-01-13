using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageResizer.Plugins.MatteRemoval.Primitives
{
    public struct Hsl : IEquatable<Hsl>
    {
        private const int HashPrimeOne = 13;
        private const int HashPrimeTwo = 31;
        private const double Precision = 0.001;
        private const int InvertedPrecision = (int)(1 / Precision);

        public readonly double H;
        public readonly double S;
        public readonly double L;

        public Hsl(double h, double s, double l)
        {
            H = h;
            S = s;
            L = l;
        }

        public bool Equals(Hsl other)
        {
            return Math.Abs(other.H - H) < Precision &&
                   Math.Abs(other.S - S) < Precision &&
                   Math.Abs(other.L - L) < Precision;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Hsl)) return false;

            var other = (Hsl)obj;

            return Math.Abs(other.H - H) < Precision &&
                   Math.Abs(other.S - S) < Precision &&
                   Math.Abs(other.L - L) < Precision;
        }

        public override int GetHashCode()
        {
            var hash = HashPrimeOne;

            hash *= HashPrimeTwo + (int)(H * InvertedPrecision);
            hash *= HashPrimeTwo + (int)(S * InvertedPrecision);
            hash *= HashPrimeTwo + (int)(L * InvertedPrecision);

            return hash;
        }
    }
}
