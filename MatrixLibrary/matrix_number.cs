using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLibrary
{
    public interface IMatrixNumberBase
    {
        IMatrixNumberBase AddInt(int add);
        IMatrixNumberBase AddDouble(double add);
        IMatrixNumberBase Copy();
        void CopyFrom(IMatrixNumberBase number);
        double ToDouble();
        bool IsZero();
        bool IsOne();

        IMatrixNumberBase __Addition(IMatrixNumberBase second);
        IMatrixNumberBase __Subtraction(IMatrixNumberBase second);
        IMatrixNumberBase __Multiplication(IMatrixNumberBase second);
        IMatrixNumberBase __Division(IMatrixNumberBase second);
        IMatrixNumberBase __Exponentiate(int exponent);
        IMatrixNumberBase __SquareRoot();
        IMatrixNumberBase __Negate();
        bool __IsEqual(IMatrixNumberBase second);
        bool __IsLessThan(IMatrixNumberBase second);
        bool __IsGreaterThan(IMatrixNumberBase second);
        string __ToString();
    }

    public struct MatrixNumberInt : IMatrixNumberBase
    {
        public int Number { get; private set; }
        public MatrixNumberInt(int number) : this() { Number = number; }

        public IMatrixNumberBase Copy()
        {
            return new MatrixNumberInt(Number);
        }
        public void CopyFrom(IMatrixNumberBase number)
        {
            if (ReferenceEquals(number, null)) { return; }

            if (number is MatrixNumberInt)
            {
                MatrixNumberInt tmpNumber = (MatrixNumberInt)number;
                Number = tmpNumber.Number;
            }
            else
            {
                Number = (int)number.ToDouble();
            }
        }
        public string __ToString()
        {
            return Number.ToString();
        }
        public override string ToString()
        {
            return __ToString();
        }
        public double ToDouble()
        {
            return (double)Number;
        }

        public bool IsZero()
        {
            return Number == 0;
        }
        public bool IsOne()
        {
            return Number == 1;
        }
        public IMatrixNumberBase AddInt(int add)
        {
            Number += add;
            return this;
        }
        public IMatrixNumberBase AddDouble(double add)
        {
            Number += (int)add;
            return this;
        }

        public IMatrixNumberBase __Addition(IMatrixNumberBase second)
        {
            if (ReferenceEquals(second, null)) { return Copy(); }
            return new MatrixNumberInt(Number + (int)second.ToDouble());
        }
        public IMatrixNumberBase __Subtraction(IMatrixNumberBase second)
        {
            if (ReferenceEquals(second, null)) { return Copy(); }
            return new MatrixNumberInt(Number - (int)second.ToDouble());
        }
        public IMatrixNumberBase __Multiplication(IMatrixNumberBase second)
        {
            if (ReferenceEquals(second, null)) { return Copy(); }
            return new MatrixNumberInt(Number * (int)second.ToDouble());
        }
        public IMatrixNumberBase __Division(IMatrixNumberBase second)
        {
            if (ReferenceEquals(second, null)) { return Copy(); }
            return new MatrixNumberInt(Number / (int)second.ToDouble());
        }
        public IMatrixNumberBase __Exponentiate(int exponent)
        {
            return new MatrixNumberInt((int)Math.Pow((double)Number, (double)exponent));
        }
        public IMatrixNumberBase __Negate()
        {
            return new MatrixNumberInt(-Number);
        }
        public IMatrixNumberBase __SquareRoot()
        {
            throw new NotImplementedException();
        }
        public bool __IsEqual(IMatrixNumberBase second)
        {
            if (ReferenceEquals(this, second)) { return true; }
            if (ReferenceEquals(this, null) || ReferenceEquals(second, null)) { return false; }
            return Number == second.ToDouble();
        }
        public bool __IsLessThan(IMatrixNumberBase second)
        {
            if (ReferenceEquals(this, second)) { return false; }
            if (ReferenceEquals(this, null) || ReferenceEquals(second, null)) { return false; }
            return Number < second.ToDouble();
        }
        public bool __IsGreaterThan(IMatrixNumberBase second)
        {
            if (ReferenceEquals(this, second)) { return false; }
            if (ReferenceEquals(this, null) || ReferenceEquals(second, null)) { return false; }
            return Number > second.ToDouble();
        }
    }

    public struct MatrixNumber : IMatrixNumberBase  // reprezentuje jednotlivé prvky v matici pomocí zlomků, jednotlivé zlomky nelze upravovat, pouze zobrazovat
    {
        public MatrixNumber(int number) : this()
        {
            Numerator = number;
            if (number != 0) { Denominator = 1; }
            else { Denominator = 0; }
            RealPart = 0;
        }
        public MatrixNumber(double number) : this()
        {
            Numerator = 0;
            Denominator = 0;
            RealPart = number;
            Regulate();
        }
        public MatrixNumber(int numerator, int denominator) : this()
        {
            Numerator = numerator;
            Denominator = denominator;
            RealPart = 0;
        }
        public MatrixNumber(MatrixNumber source) : this()
        {
            Numerator = source.Numerator;
            Denominator = source.Denominator;
            RealPart = source.RealPart;
        }


        public double ToDouble() // Vrací hodnotu objektu Cislo v reálném čísle
        {
            double result;
            if (Numerator != 0 && Denominator != 0)
            {
                result = (double)Numerator / (double)Denominator;
                result += RealPart;
            }
            else
            {
                result = RealPart;
            }
            return result;
        }
        public IMatrixNumberBase Copy()
        {
            MatrixNumber copy = new MatrixNumber(this);
            return copy;
        }
        public void CopyFrom(IMatrixNumberBase number)
        {
            if (ReferenceEquals(number, null)) { return; }

            if (number is MatrixNumber)
            {
                MatrixNumber tmpNumber = (MatrixNumber)number;
                Numerator = tmpNumber.Numerator;
                Denominator = tmpNumber.Denominator;
                RealPart = tmpNumber.RealPart;
            }
            else
            {
                Numerator = 0;
                Denominator = 0;
                RealPart = number.ToDouble();
                Regulate();
            }
        }

        public void Regulate()
        {
            if (RealPart != 0) { DecomposeRealPart(); }
            int divisor;
            divisor = GCD(Numerator, Denominator);
            if (divisor != 0 && divisor != 1)
            {
                int tmpNumerator = Numerator;
                try { Numerator /= divisor; }
                catch (OverflowException)
                {
                    RealPart = (double)Numerator / (double)Denominator;
                    Numerator = 0;
                    Denominator = 0;
                    DecomposeRealPart();
                }
                try { Denominator /= divisor; }
                catch (OverflowException)
                {
                    RealPart = (double)tmpNumerator / (double)Denominator;
                    Numerator = 0;
                    Denominator = 0;
                    DecomposeRealPart();
                }

                RealPart /= (double)divisor;
            }
            if (Denominator == 0 || Numerator == 0)
            {
                Numerator = 0;
                Denominator = 0;
            }
            if (Denominator < 0)
            {
                Numerator = Numerator * (-1);
                Denominator = Denominator * (-1);
            }

            if (Numerator > HalfOfMaxInt || Numerator < HalfOfMinInt || Denominator > HalfOfMaxInt || Denominator < HalfOfMinInt)
            {
                RealPart = (double)Numerator / (double)Denominator;
                DecomposeRealPart();
            }
        }
        private void DecomposeRealPart()
        {
            int tmp, tmp_2;

            tmp = (int)RealPart;
            tmp_2 = (int)Math.Ceiling(RealPart);
            if (RealPart == (double)tmp)
            {
                if (Denominator != 0)
                {
                    tmp *= Denominator;
                    Numerator += tmp;
                    RealPart = 0;
                }
                else
                {
                    Numerator = tmp;
                    Denominator = 1;
                    RealPart = 0;
                }
            }
            if (RealPart > ((double)tmp - Accuracy) && RealPart < ((double)tmp + Accuracy))
            {
                if (Denominator != 0)
                {
                    tmp *= Denominator;
                    Numerator += tmp;
                    RealPart = 0;
                }
                else
                {
                    Numerator = tmp;
                    Denominator = 1;
                    RealPart = 0;
                }
            }
            if (RealPart > ((double)tmp_2 - Accuracy) && RealPart < ((double)tmp_2 + Accuracy))
            {
                if (Denominator != 0)
                {
                    tmp_2 *= Denominator;
                    Numerator += tmp_2;
                    RealPart = 0;
                }
                else
                {
                    Numerator = tmp_2;
                    Denominator = 1;
                    RealPart = 0;
                }
            }
        }

        /* Private set properties */
        public int Numerator { get; private set; }
        public int Denominator { get; private set; }
        public double RealPart { get; private set; } // Iracionální čísla (přičítá se ke zlomku) reprezentovaná doublem

        public IMatrixNumberBase AddInt(int add)
        {
            MatrixNumber tmp = (MatrixNumber)__Addition(new MatrixNumber(add));
            Numerator = tmp.Numerator;
            Denominator = tmp.Denominator;
            RealPart = tmp.RealPart;
            return this;
        }
        public IMatrixNumberBase AddDouble(double add)
        {
            MatrixNumber tmp = (MatrixNumber)__Addition(new MatrixNumber(add));
            Numerator = tmp.Numerator;
            Denominator = tmp.Denominator;
            RealPart = tmp.RealPart;
            return this;
        }

        static private MatrixNumber Zero = new MatrixNumber();
        public bool IsZero()
        {
            return (this.__IsEqual(Zero));
        }

        static private MatrixNumber One = new MatrixNumber(1);
        public bool IsOne()
        {
            return (this.__IsEqual(One));
        }

        public IMatrixNumberBase __Addition(IMatrixNumberBase second)
        {
            if (ReferenceEquals(second, null)) { return Copy(); }

            MatrixNumber result = new MatrixNumber();

            if (second is MatrixNumber)
            {
                MatrixNumber tmpSecond = (MatrixNumber)second;

                if (Numerator != 0 && tmpSecond.Numerator != 0)
                {
                    result.Numerator = Numerator * tmpSecond.Denominator;
                    result.Numerator += (tmpSecond.Numerator * Denominator);
                    result.Denominator = Denominator * tmpSecond.Denominator;
                    result.RealPart = RealPart + tmpSecond.RealPart;
                }
                else if (Numerator == 0)
                {
                    result.Numerator = tmpSecond.Numerator;
                    result.Denominator = tmpSecond.Denominator;
                    result.RealPart = RealPart + tmpSecond.RealPart;
                }
                else // tmpSecond.Numerator == 0
                {
                    result.Numerator = Numerator;
                    result.Denominator = Denominator;
                    result.RealPart = RealPart + tmpSecond.RealPart;
                }
            }
            else
            {
                result.RealPart = ToDouble() + second.ToDouble();
            }

            result.Regulate();
            return result;
        }
        public IMatrixNumberBase __Subtraction(IMatrixNumberBase second)
        {
            if (ReferenceEquals(second, null)) { return Copy(); }

            MatrixNumber result = new MatrixNumber();

            if (second is MatrixNumber)
            {
                MatrixNumber tmpSecond = (MatrixNumber)second;
                if (Numerator != 0 && tmpSecond.Numerator != 0)
                {
                    result.Numerator = Numerator * tmpSecond.Denominator;
                    result.Numerator -= (tmpSecond.Numerator * Denominator);
                    result.Denominator = Denominator * tmpSecond.Denominator;
                    result.RealPart = RealPart - tmpSecond.RealPart;
                }
                else if (Numerator == 0)
                {
                    result.Numerator = -tmpSecond.Numerator;
                    result.Denominator = tmpSecond.Denominator;
                    result.RealPart = RealPart - tmpSecond.RealPart;
                }
                else // tmpSecond.Numerator == 0
                {
                    result.Numerator = Numerator;
                    result.Denominator = Denominator;
                    result.RealPart = RealPart - tmpSecond.RealPart;
                }
            }
            else
            {
                result.RealPart = ToDouble() - second.ToDouble();
            }

            result.Regulate();
            return result;
        }
        public IMatrixNumberBase __Multiplication(IMatrixNumberBase second)
        {
            if (ReferenceEquals(second, null)) { return Copy(); }

            MatrixNumber result = new MatrixNumber();

            if (second is MatrixNumber)
            {
                MatrixNumber tmp = (MatrixNumber)second;

                result.Numerator = Numerator * tmp.Numerator;
                result.Denominator = Denominator * tmp.Denominator;

                double fraction1, fraction2;
                if (Numerator != 0 && Denominator != 0) { fraction1 = (double)Numerator / (double)Denominator; }
                else { fraction1 = 0; }
                if (tmp.Numerator != 0 && tmp.Denominator != 0) { fraction2 = (double)tmp.Numerator / (double)tmp.Denominator; }
                else { fraction2 = 0; }

                result.RealPart = fraction1 * tmp.RealPart;
                result.RealPart += (RealPart * fraction2);
                result.RealPart += (RealPart * tmp.RealPart);
            }
            else
            {
                result.RealPart = ToDouble() * second.ToDouble();
            }

            result.Regulate();
            return result;
        }
        public IMatrixNumberBase __Division(IMatrixNumberBase second)
        {
            if (ReferenceEquals(second, null)) { return Copy(); }

            MatrixNumber result = new MatrixNumber();

            if (second is MatrixNumber)
            {
                MatrixNumber tmpSecond = (MatrixNumber)second;

                if (RealPart == 0 && tmpSecond.RealPart == 0)
                {
                    result.Numerator = Numerator * tmpSecond.Denominator;
                    result.Denominator = Denominator * tmpSecond.Numerator;
                }
                else
                {
                    if (Numerator != 0 || Denominator != 0 || tmpSecond.Numerator != 0 || tmpSecond.Denominator != 0)
                    {
                        if (tmpSecond.RealPart == 0)
                        {
                            double tmp;
                            tmp = (double)Denominator * RealPart;
                            result.Numerator = Numerator * tmpSecond.Denominator;
                            result.Denominator = Denominator * tmpSecond.Numerator;
                            if (Denominator != 0 && tmpSecond.Numerator != 0)
                            {
                                result.RealPart = (tmp * (double)tmpSecond.Denominator) / ((double)Denominator * (double)tmpSecond.Numerator);
                            }
                            else { result.RealPart = 0; }
                        }
                        else
                        {
                            double tmp_1, tmp_2;
                            if (Numerator != 0 && Denominator != 0)
                            {
                                tmp_1 = ((double)Numerator / (double)Denominator) + RealPart;
                            }
                            else { tmp_1 = RealPart; }

                            if (Numerator != 0 && tmpSecond.Denominator != 0)
                            {
                                tmp_2 = ((double)tmpSecond.Numerator / (double)tmpSecond.Denominator) + tmpSecond.RealPart;
                            }
                            else { tmp_2 = tmpSecond.RealPart; }

                            if (tmp_1 != 0 && tmp_2 != 0) { result.RealPart = tmp_1 / tmp_2; }
                            else { result.RealPart = 0; }
                        }
                    }
                    else
                    {
                        if (RealPart != 0 && tmpSecond.RealPart != 0) { result.RealPart = RealPart / tmpSecond.RealPart; }
                        else { result.RealPart = 0; }
                    }
                }
            }
            else
            {
                result.RealPart = ToDouble() / second.ToDouble();
            }

            result.Regulate();
            return result;
        }
        public IMatrixNumberBase __Exponentiate(int exponent)
        {
            MatrixNumber result = new MatrixNumber();

            result.Numerator = Numerator;
            result.Denominator = Denominator;
            result.RealPart = RealPart;
            for (int i = 2; i <= exponent; i++)
            {
                double fraction_res, fraction_num, tmp;
                if (result.Numerator != 0 && result.Denominator != 0) { fraction_res = (double)result.Numerator / (double)result.Denominator; }
                else { fraction_res = 0; }
                if (Numerator != 0 && Denominator != 0) { fraction_num = (double)Numerator / (double)Denominator; }
                else { fraction_num = 0; }

                tmp = fraction_res * RealPart;
                tmp += (result.RealPart * fraction_num);
                tmp += (result.RealPart * RealPart);
                result.RealPart = tmp;

                result.Numerator *= Numerator;
                result.Denominator *= Denominator;
            }

            result.Regulate();
            return result;
        }
        public IMatrixNumberBase __SquareRoot()
        {
            MatrixNumber result = new MatrixNumber();

            if (RealPart == 0)
            {
                double sqrt_double_numerator, sqrt_double_denominator;
                int sqrt_int_numerator, sqrt_int_denominator;
                if (Numerator >= 0)
                {
                    sqrt_double_numerator = Math.Sqrt((double)Numerator);
                    sqrt_int_numerator = (int)Math.Sqrt(Numerator);
                }
                else
                {
                    sqrt_double_numerator = -Math.Sqrt((double)-Numerator);
                    sqrt_int_numerator = -(int)Math.Sqrt(-Numerator);
                }


                if (Denominator >= 0)
                {
                    sqrt_double_denominator = Math.Sqrt((double)Denominator);
                    sqrt_int_denominator = (int)Math.Sqrt(Denominator);
                }
                else
                {
                    sqrt_double_denominator = -Math.Sqrt((double)-Denominator);
                    sqrt_int_denominator = -(int)Math.Sqrt(-Denominator);
                }

                if (sqrt_double_numerator == (double)sqrt_int_numerator && sqrt_double_denominator == (double)sqrt_int_denominator)
                {
                    result.Numerator = sqrt_int_numerator;
                    result.Denominator = sqrt_int_denominator;
                }
                else
                {
                    if (sqrt_double_numerator != 0 && sqrt_double_denominator != 0)
                    {
                        result.RealPart = sqrt_double_numerator / sqrt_double_denominator;
                    }
                    else { result.RealPart = 0; }
                }
            }
            else
            {
                if (Numerator == 0 || Denominator == 0)
                {
                    if (result.RealPart >= 0) { result.RealPart = Math.Sqrt(RealPart); }
                    else { result.RealPart = -Math.Sqrt(-result.RealPart); }
                }
                else
                {
                    result.RealPart = (double)Numerator / (double)Denominator;
                    result.RealPart += RealPart;
                    if (result.RealPart >= 0) { result.RealPart = Math.Sqrt(result.RealPart); }
                    else { result.RealPart = -Math.Sqrt(-result.RealPart); }
                }
            }

            result.Regulate();
            return result;
        }
        public IMatrixNumberBase __Negate()
        {
            MatrixNumber result = new MatrixNumber();
            result.Numerator = (-1) * Numerator;
            result.RealPart = (double)(-1) * RealPart;
            result.Denominator = Denominator;
            return result;
        }
        public bool __IsEqual(IMatrixNumberBase second)
        {
            if (ReferenceEquals(this, second)) { return true; }
            if (ReferenceEquals(this, null) || ReferenceEquals(second, null)) { return false; }

            if (second is MatrixNumber)
            {
                MatrixNumber tmp = (MatrixNumber)second;
                if (Numerator != tmp.Numerator) { return false; }
                if (Denominator != tmp.Denominator) { return false; }
                if (RealPart < (tmp.RealPart - Accuracy) || RealPart > (tmp.RealPart + Accuracy)) { return false; }
                return true;
            }
            else
            {
                double first_d, second_d;
                first_d = ToDouble();
                second_d = second.ToDouble();
                if (first_d >= (second_d - Accuracy) && first_d <= (second_d + Accuracy)) { return true; }
                else { return false; }
            }
        }
        public bool __IsLessThan(IMatrixNumberBase second)
        {
            if (ReferenceEquals(this, second)) { return false; }
            if (ReferenceEquals(this, null) || ReferenceEquals(second, null)) { return false; }

            double first_d, second_d;
            first_d = ToDouble();
            second_d = second.ToDouble();

            if (first_d < second_d) { return true; }
            else { return false; }
        }
        public bool __IsGreaterThan(IMatrixNumberBase second)
        {
            if (ReferenceEquals(this, second)) { return false; }
            if (ReferenceEquals(this, null) || ReferenceEquals(second, null)) { return false; }

            double first_d, second_d;
            first_d = ToDouble();
            second_d = second.ToDouble();

            if (first_d > second_d) { return true; }
            else { return false; }
        }
        public string __ToString()
        {
            return ToDouble().ToString();
        }

        public override string ToString()
        {
            return __ToString();
        }
        public override bool Equals(object obj)
        {
            return (this.__IsEqual((MatrixNumber)obj));
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private static int GCD(int first, int second)
        {
            int tmp;
            if (second == 1 || first == 1) { return 1; }
            if (first == 0 || second == 0) { return 1; }
            if (second > first)
            {
                tmp = second;
                second = first;
                first = tmp;
            }
            while (second != 0)
            {
                tmp = first % second;
                first = second;
                second = tmp;
            }
            return first;
        }
        private static double Accuracy = 0.0000001;
        private static int HalfOfMaxInt = int.MaxValue / 2;
        private static int HalfOfMinInt = int.MinValue / 2;

        public static MatrixNumber operator +(MatrixNumber first, MatrixNumber second)
        {
            return (MatrixNumber)first.__Addition(second);
        }
        public static MatrixNumber operator -(MatrixNumber first, MatrixNumber second)
        {
            return (MatrixNumber)first.__Subtraction(second);
        }

        public static bool operator ==(MatrixNumber first, MatrixNumber second)
        {
            return first.__IsEqual(second);
        }
        public static bool operator !=(MatrixNumber first, MatrixNumber second)
        {
            return !first.__IsEqual(second);
        }
    }
}