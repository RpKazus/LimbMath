using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LimbMath
{
    public class Fraction : IEquatable<Fraction>
    {
        public long Numerator { get; set; }
        public long Denominator { get; set; }
        public enum fraction_type { Unabbreviated, Abbreviated, AbbreviatedAndExtracted };
        public static long MakeType = 2;
        public Fraction ()
        {
            Numerator = 1;
            Denominator = 1;
            
        }
        public Fraction (long numerator, long denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
        }
        public Fraction (Limb limb)
        {
            if(limb.MathName == "/")
            {
                Fraction frac1 = new Fraction(limb.Values[0]);
                for (int i = 1; i < limb.Values.Count; i++)
                {
                    Fraction frac2 = new Fraction(limb.Values[i]);
                    frac1 /= frac2;
                }
                Numerator = frac1.Numerator;
                Denominator = frac1.Denominator;
            }
            else if(limb.Values.Count == 0)
            {
                Numerator = Convert.ToInt32(limb.MathName);
                Denominator = 1;
            }
        }
        public Fraction (decimal number)
        {
            long count = 1;
            while (number % 1 != 0)
            {
                count *= 10;
                number *= 10;
            }
            Numerator = Convert.ToInt64(number);
            Denominator = count;
            Simpler();
        }

        #region Унарки
        public static Fraction operator -(Fraction a)
        {
            a.Numerator *= -1;
            return a;
        }
        public static Fraction operator ++(Fraction a)
        {
            a.Numerator += a.Denominator;
            return a;
        }
        public static Fraction operator --(Fraction a)
        {
            a.Numerator -= a.Denominator;
            return a;
        }
        #endregion

        #region Бинарки

        public static Fraction operator +(Fraction f1, Fraction f2)
        {
            return new Fraction(f1.Numerator * f2.Denominator + f2.Numerator * f1.Denominator, f1.Denominator * f2.Denominator).ToSimple();
        }

        public static Fraction operator -(Fraction f1, Fraction f2)
        {
            return new Fraction(f1.Numerator * f2.Denominator - f2.Numerator * f1.Denominator, f1.Denominator * f2.Denominator).ToSimple();
        }

        public static Fraction operator *(Fraction f1, Fraction f2)
        {
            return new Fraction(f1.Numerator * f2.Numerator, f1.Denominator * f2.Denominator).ToSimple();
        }

        public static Fraction operator /(Fraction f1, Fraction f2)
        {
            return new Fraction(f1.Numerator * f2.Denominator, f1.Denominator * f2.Numerator).ToSimple();
        }
        public static Fraction operator ^(Fraction f1, Fraction f2)
        {
            double res = Convert.ToDouble(f2.Numerator) / Convert.ToDouble(f2.Denominator);
            double num = Math.Pow(f1.Numerator, res);
            double den = Math.Pow(f1.Denominator, res);
            return new Fraction(Convert.ToDecimal(num) / Convert.ToDecimal(den)).ToSimple();
        }
        #endregion

        #region Равенста, отношения

        public static bool operator ==(Fraction f1, Fraction f2)
        {
            return (double)f1.Numerator / f1.Denominator == (double)f2.Numerator / f2.Denominator;
        }

        public static bool operator !=(Fraction f1, Fraction f2)
        {
            return (double)f1.Numerator / f1.Denominator != (double)f2.Numerator / f2.Denominator;
        }

        public static bool operator >(Fraction f1, Fraction f2)
        {
            return (double)f1.Numerator / f1.Denominator > (double)f2.Numerator / f2.Denominator;
        }

        public static bool operator <(Fraction f1, Fraction f2)
        {
            return (double)f1.Numerator / f1.Denominator < (double)f2.Numerator / f2.Denominator;
        }

        public static bool operator >=(Fraction f1, Fraction f2)
        {
            return (double)f1.Numerator / f1.Denominator >= (double)f2.Numerator / f2.Denominator;
        }

        public static bool operator <=(Fraction f1, Fraction f2)
        {
            return (double)f1.Numerator / f1.Denominator <= (double)f2.Numerator / f2.Denominator;
        }

        #endregion

        public override string ToString()
        {
            try
            {
                return ((decimal)Numerator / (decimal)Denominator > 0 ? "" : "-") + Math.Abs(Numerator) + "/" + Math.Abs(Denominator);
            }
            catch (OverflowException)
            {
                return "";
            }
        }
        public decimal ToDecimal()
        {
            return Numerator / Denominator;
        }
        public Limb ToLimb()
        {
            Simpler();
            bool IsDigit = true;
            foreach (char ch in Denominator.ToString())
                if (ch != '1' && ch != '0' && ch != ',')
                    IsDigit = false;
            if (Numerator % Denominator == 0 || IsDigit)
                return new Limb((Convert.ToDecimal(Numerator) / Convert.ToDecimal(Denominator)).ToString());
            else if (MakeType == 2)
            {
                Limb root = new Limb();
                root.MathName = "+";
                root.Values.Add(new Limb(Convert.ToInt32(Numerator / Denominator).ToString()));
                root.Values.Add(new Limb() {MathName = "/", Values = new List<Limb>() {new Limb((Numerator % Denominator).ToString()), new Limb(Denominator.ToString()) } });
                return root;
            }
            else
            {
                Limb root = new Limb();
                root.MathName = "/";
                root.Values = new List<Limb>() { new Limb(Numerator.ToString()), new Limb(Denominator.ToString())};
                return root;
            }
        }
        private void Simpler()
        {
            if (MakeType != 0 && Numerator != 0 && Denominator != 0)
            {
                long N = NOD;
                long oldNum = Numerator;
                Numerator = (N % Denominator == 0) ? N / Denominator : N;
                Denominator = (N % Denominator == 0) ? N / oldNum : Denominator;
            }
        }
        public Fraction ToSimple()
        {
            if (MakeType != 0 && Numerator != 0 && Denominator != 0)
            {
                long N = NOD;
                long oldNum = Numerator;
                long Numer = (N % Denominator == 0) ? N / Denominator : N;
                long Demer = (N % Denominator == 0) ? N / oldNum : Denominator;
                return new Fraction(Numer, Demer);
            }
            return this;
        }
        public long NOD
        {
            get
            {
                long n = Numerator;
                long d = Denominator;
                bool was = false;
                if (n < 0)
                {
                    n *= -1;
                    was = !was;
                }
                if (d < 0)
                {
                    d *= -1;
                    was = !was;
                }
                if (n.ToString()[n.ToString().Length - 1] != '0' && n.ToString()[n.ToString().Length - 1] != '5' && (d.ToString()[d.ToString().Length - 1] == '0' || d.ToString()[d.ToString().Length - 1] == '5'))
                    return n;
                    while (n != d)
                    if (n > d) d += Denominator;
                    else n += Numerator;
                if (was)
                    n *= -1;
                return n;
            }
        }
        public bool Equals(Fraction other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return (double)Numerator / Denominator == (double)other.Numerator / other.Denominator;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == typeof(Fraction) && Equals((Fraction)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Convert.ToInt32((Numerator * 397) ^ Denominator);
            }
        }
    }
}
