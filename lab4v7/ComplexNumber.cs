using System;

namespace OOP_Lab4
{
    public class ComplexNumber
    {
        private double _real;
        private double _imaginary;

        public double Real
        {
            get => _real;
            set => _real = value;
        }

        public double Imaginary
        {
            get => _imaginary;
            set => _imaginary = value;
        }

        public double this[int index]
        {
            get => index switch
            {
                0 => Real,
                1 => Imaginary,
                _ => throw new IndexOutOfRangeException("Індекс повинен бути 0 (Real) або 1 (Imaginary).")
            };
            set
            {
                switch (index)
                {
                    case 0:
                        Real = value;
                        break;
                    case 1:
                        Imaginary = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Індекс повинен бути 0 (Real) або 1 (Imaginary).");
                }
            }
        }

        public static ComplexNumber I => new ComplexNumber(0, 1);

        public ComplexNumber(double real = 0, double imaginary = 0)
        {
            Real = real;
            Imaginary = imaginary;
        }

        public static ComplexNumber operator +(ComplexNumber a, ComplexNumber b)
        {
            if (a is null || b is null)
                throw new ArgumentNullException("Операнди не можуть бути null.");

            return new ComplexNumber(a.Real + b.Real, a.Imaginary + b.Imaginary);
        }

        public static ComplexNumber operator *(ComplexNumber a, ComplexNumber b)
        {
            if (a is null || b is null)
                throw new ArgumentNullException("Операнди не можуть бути null.");

            double realPart = a.Real * b.Real - a.Imaginary * b.Imaginary;
            double imaginaryPart = a.Real * b.Imaginary + a.Imaginary * b.Real;
            return new ComplexNumber(realPart, imaginaryPart);
        }

        public static bool operator ==(ComplexNumber? a, ComplexNumber? b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(ComplexNumber? a, ComplexNumber? b)
        {
            return !(a == b);
        }

        public override bool Equals(object? obj)
        {
            if (obj is ComplexNumber other)
            {
                return Math.Abs(Real - other.Real) < 1e-9 &&
                       Math.Abs(Imaginary - other.Imaginary) < 1e-9;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Real, Imaginary);
        }

        public override string ToString()
        {
            if (Imaginary == 0) return $"{Real}";
            if (Real == 0) return $"{Imaginary}i";
            
            char sign = Imaginary > 0 ? '+' : '-';
            return $"{Real} {sign} {Math.Abs(Imaginary)}i";
        }
    }
}