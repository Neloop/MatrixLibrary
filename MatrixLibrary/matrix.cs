﻿//#define NULLS_IN_MATRIX

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLibrary
{
    public class MatrixLibraryException : Exception
    {
        public MatrixLibraryException() { }
        public MatrixLibraryException(string message) : base(message) { }
        public MatrixLibraryException(string message, Exception inner) : base(message, inner) { }
    }

    public class EigenValuesNotFoundException : Exception
    {
        public EigenValuesNotFoundException() { }
        public EigenValuesNotFoundException(string message) : base(message) { }
        public EigenValuesNotFoundException(string message, Exception inner) : base(message, inner) { }
    }

    public abstract class MatrixNumberBase
    {
        public abstract MatrixNumberBase AddInt(int add);
        public abstract MatrixNumberBase AddDouble(double add);
        public abstract MatrixNumberBase Copy();
        public abstract double ToDouble();
        public abstract bool IsZero();
        public abstract bool IsOne();

        public static MatrixNumberBase operator +(MatrixNumberBase first, MatrixNumberBase second)
        {
            if (first == null && second == null) { throw new MatrixLibraryException("Both numbers were nulls"); }
            else if (first == null) { return second.Copy(); }
            else if (second == null) { return first.Copy(); }

            return first.__Addition(second);
        }
        public static MatrixNumberBase operator -(MatrixNumberBase first, MatrixNumberBase second)
        {
            if (first == null && second == null) { throw new MatrixLibraryException("Both numbers were nulls"); }
            else if (first == null) { return second.Copy(); }
            else if (second == null) { return first.Copy(); }

            return first.__Subtraction(second);
        }
        public static MatrixNumberBase operator -(MatrixNumberBase number)
        {
            if (number == null) { throw new MatrixLibraryException("Number is null"); }

            return number.__Negate();
        }
        public static MatrixNumberBase operator *(MatrixNumberBase first, MatrixNumberBase second)
        {
            if (first == null && second == null) { throw new MatrixLibraryException("Both numbers were nulls"); }
            else if (first == null) { return second.Copy(); }
            else if (second == null) { return first.Copy(); }

            return first.__Multiplication(second);
        }
        public static MatrixNumberBase operator /(MatrixNumberBase first, MatrixNumberBase second)
        {
            if (first == null && second == null) { throw new MatrixLibraryException("Both numbers were nulls"); }
            else if (first == null) { return second.Copy(); }
            else if (second == null) { return first.Copy(); }

            return first.__Division(second);
        }
        public static bool operator ==(MatrixNumberBase first, MatrixNumberBase second)
        {
            if (ReferenceEquals(first, second)) { return true; }
            if (ReferenceEquals(first, null) || ReferenceEquals(first, null)) { return false; }

            return first.__IsEqual(second);
        }
        public static bool operator !=(MatrixNumberBase first, MatrixNumberBase second)
        {
            if (ReferenceEquals(first, second)) { return true; }
            if (ReferenceEquals(first, null) || ReferenceEquals(first, null)) { return false; }

            return !first.__IsEqual(second);
        }
        public static bool operator <=(MatrixNumberBase first, MatrixNumberBase second)
        {
            if (first == null && second == null) { throw new MatrixLibraryException("Both numbers were nulls"); }
            else if (first == null || second == null) { return false; }

            if (first.__IsEqual(second) == true) { return true; }
            else { return first.__IsLessThan(second); }
        }
        public static bool operator >=(MatrixNumberBase first, MatrixNumberBase second)
        {
            if (first == null && second == null) { throw new MatrixLibraryException("Both numbers were nulls"); }
            else if (first == null || second == null) { return false; }

            if (first.__IsEqual(second) == true) { return true; }
            else { return first.__IsGreaterThan(second); }
        }
        public static bool operator <(MatrixNumberBase first, MatrixNumberBase second)
        {
            if (first == null && second == null) { throw new MatrixLibraryException("Both numbers were nulls"); }
            else if (first == null || second == null) { return false; }

            return first.__IsLessThan(second);
        }
        public static bool operator >(MatrixNumberBase first, MatrixNumberBase second)
        {
            if (first == null && second == null) { throw new MatrixLibraryException("Both numbers were nulls"); }
            else if (first == null || second == null) { return false; }

            return first.__IsGreaterThan(second);
        }
        public override bool Equals(object obj)
        {
            return (this == (MatrixNumberBase)obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return __ToString();
        }

        public abstract MatrixNumberBase __Addition(MatrixNumberBase second);
        public abstract MatrixNumberBase __Subtraction(MatrixNumberBase second);
        public abstract MatrixNumberBase __Multiplication(MatrixNumberBase second);
        public abstract MatrixNumberBase __Division(MatrixNumberBase second);
        public abstract MatrixNumberBase __Exponentiate(int exponent);
        public abstract MatrixNumberBase __SquareRoot();
        public abstract MatrixNumberBase __Negate();
        public abstract bool __IsEqual(MatrixNumberBase second);
        public abstract bool __IsLessThan(MatrixNumberBase second);
        public abstract bool __IsGreaterThan(MatrixNumberBase second);
        public abstract string __ToString();
    }

    public class MatrixNumber : MatrixNumberBase  // reprezentuje jednotlivé prvky v matici pomocí zlomků, jednotlivé zlomky nelze upravovat, pouze zobrazovat
    {
        static MatrixNumber()
        {
            Zero = new MatrixNumber();
            One = new MatrixNumber(1);
        }

        public MatrixNumber()
        {
            Numerator = 0;
            Denominator = 0;
            RealPart = 0;
        }

        public MatrixNumber(int number)
        {
            Numerator = number;
            if(number != 0){ Denominator = 1; }
            else{ Denominator = 0; }
            RealPart = 0;
        }

        public MatrixNumber(double number)
        {
            Numerator = 0;
            Denominator = 0;
            RealPart = number;
            Regulate();
        }

        public MatrixNumber(int numerator, int denominator)
        {
            Numerator = numerator;
            Denominator = denominator;
            RealPart = 0;
        }
        
        public override double ToDouble() // Vrací hodnotu objektu Cislo v reálném čísle
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

        public override MatrixNumberBase Copy()
        {
            MatrixNumber copy = new MatrixNumber();

            copy.Numerator = Numerator;
            copy.Denominator = Denominator;
            copy.RealPart = RealPart;

            return copy;
        }

        public void Regulate()
        {
            if (RealPart != 0) { DecomposeRealPart(); }
            int divisor;
            divisor = GCD(Numerator, Denominator);
            if (divisor != 0 && divisor != 1)
            {
                Numerator /= divisor;
                Denominator /= divisor;
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

        public override MatrixNumberBase AddInt(int add)
        {
            MatrixNumber tmp = (MatrixNumber)__Addition(new MatrixNumber(add));
            Numerator = tmp.Numerator;
            Denominator = tmp.Denominator;
            RealPart = tmp.RealPart;
            return this;
        }

        public override MatrixNumberBase AddDouble(double add)
        {
            MatrixNumber tmp = (MatrixNumber)__Addition(new MatrixNumber(add));
            Numerator = tmp.Numerator;
            Denominator = tmp.Denominator;
            RealPart = tmp.RealPart;
            return this;
        }

        static public MatrixNumber Zero { get; private set; }
        public override bool IsZero()
        {
            return (this == Zero);
        }

        static public MatrixNumber One { get; private set; }
        public override bool IsOne()
        {
            return (this == One);
        }

        public override MatrixNumberBase __Addition(MatrixNumberBase second)
        {
            if (second == null) { return Copy(); }

            MatrixNumber result = new MatrixNumber();

            if (second is MatrixNumber)
            {
                MatrixNumber tmpSecond = (MatrixNumber)second;

                if(Numerator == 0)
                {
                    result.Numerator = tmpSecond.Numerator;
                    result.Denominator = tmpSecond.Denominator;
                    result.RealPart = RealPart + tmpSecond.RealPart;
                }
                else if(tmpSecond.Numerator == 0)
                {
                    result.Numerator = Numerator;
                    result.Denominator = Denominator;
                    result.RealPart = RealPart + tmpSecond.RealPart;
                }
                else
                {
                    result.Numerator = Numerator * tmpSecond.Denominator;
                    result.Numerator += (tmpSecond.Numerator * Denominator);
                    result.Denominator = Denominator * tmpSecond.Denominator;
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
        public override MatrixNumberBase __Subtraction(MatrixNumberBase second)
        {
            if (second == null) { return Copy(); }

            MatrixNumber result = new MatrixNumber();

            if (second is MatrixNumber)
            {
                MatrixNumber tmpSecond = (MatrixNumber)second;
                if (Numerator != 0 && tmpSecond.Numerator != 0)
                {
                    result.Numerator = Numerator * tmpSecond.Denominator;

                    result.Numerator = result.Numerator - (tmpSecond.Numerator * Denominator);

                    result.Denominator = Denominator * tmpSecond.Denominator;

                    result.RealPart = RealPart - tmpSecond.RealPart;
                }
                else
                {
                    if (Numerator == 0)
                    {
                        result.Numerator = -tmpSecond.Numerator;
                        result.Denominator = tmpSecond.Denominator;
                        result.RealPart = RealPart - tmpSecond.RealPart;
                    }
                    else
                    {
                        result.Numerator = Numerator;
                        result.Denominator = Denominator;
                        result.RealPart = RealPart - tmpSecond.RealPart;
                    }
                }
            }
            else
            {
                result.RealPart = ToDouble() - second.ToDouble();
            }
            
            result.Regulate();
            return result;
        }
        public override MatrixNumberBase __Multiplication(MatrixNumberBase second)
        {
            if (second == null) { return Copy(); }

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
                result.RealPart = result.RealPart + (RealPart * fraction2);
                result.RealPart = result.RealPart + (RealPart * tmp.RealPart);
            }
            else
            {
                result.RealPart = ToDouble() * second.ToDouble();
            }

            result.Regulate();
            return result;
        }
        public override MatrixNumberBase __Division(MatrixNumberBase second)
        {
            if (second == null) { return Copy(); }

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
                    if (Numerator == 0 && Denominator == 0 && tmpSecond.Numerator == 0 && tmpSecond.Denominator == 0)
                    {
                        if (RealPart != 0 && tmpSecond.RealPart != 0) { result.RealPart = RealPart / tmpSecond.RealPart; }
                        else { result.RealPart = 0; }
                    }
                    else
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
                }
            }
            else
            {
                result.RealPart = ToDouble() / second.ToDouble();
            }

            result.Regulate();
            return result;
        }
        public override MatrixNumberBase __Exponentiate(int exponent)
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
                tmp = tmp + (result.RealPart * fraction_num);
                tmp = tmp + (result.RealPart * RealPart);
                result.RealPart = tmp;

                result.Numerator *= Numerator;
                result.Denominator *= Denominator;
            }

            result.Regulate();
            return result;
        }
        public override MatrixNumberBase __SquareRoot()
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
        public override MatrixNumberBase __Negate()
        {
            MatrixNumber result = new MatrixNumber();
            result.Numerator = (-1) * Numerator;
            result.RealPart = (double)(-1) * RealPart;
            result.Denominator = Denominator;
            return result;
        }
        public override bool __IsEqual(MatrixNumberBase second)
        {
            if (ReferenceEquals(this, second)) { return true; }
            if (ReferenceEquals(this, null) || ReferenceEquals(second, null)) { return false; }

            if (second is MatrixNumber)
            {
                MatrixNumber tmp = (MatrixNumber)second;
                if (Numerator != tmp.Numerator) { return false; }
                if (Denominator != tmp.Denominator) { return false; }
                if (RealPart != tmp.RealPart) { return false; }
                return true;
            }
            else
            {
                double first_d, second_d;
                first_d = ToDouble();
                second_d = second.ToDouble();
                if (first_d == second_d) { return true; }
                else { return false; }
            }
        }
        public override bool __IsLessThan(MatrixNumberBase second)
        {
            if (ReferenceEquals(this, second)) { return false; }
            if (ReferenceEquals(this, null) || ReferenceEquals(second, null)) { return false; }

            double first_d, second_d;
            first_d = ToDouble();
            second_d = second.ToDouble();

            if (first_d < second_d) { return true; }
            else { return false; }
        }
        public override bool __IsGreaterThan(MatrixNumberBase second)
        {
            if (ReferenceEquals(this, second)) { return false; }
            if (ReferenceEquals(this, null) || ReferenceEquals(second, null)) { return false; }

            double first_d, second_d;
            first_d = ToDouble();
            second_d = second.ToDouble();

            if (first_d > second_d) { return true; }
            else { return false; }
        }
        public override string __ToString()
        {
            return ToDouble().ToString();
        }

        public override string ToString()
        {
            return ToDouble().ToString();
        }
        public override bool Equals(object obj)
        {
            return (this == (MatrixNumber)obj);
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
    }
    
    public class Vl_cisla<T> where T : MatrixNumberBase, new() // slouží víceméně jen k prohlížení, ne k měnění, či počítání
    {
        int pocet;
        T[] vl_cisla;
        public Vl_cisla(T[] vstup, int pocet)
        {
            this.pocet = pocet;
            vl_cisla = new T[pocet];

            for (int i = 0; i < pocet; i++)
            {
                vl_cisla[i] = (T)vstup[i].Copy();
            }
        }
        public T Vrat_cislo(int i)
        {
            return vl_cisla[i];
        }
        public int Pocet()
        {
            return pocet;
        }
    }
    
    public class Matrix<T> where T : MatrixNumberBase, new()
    {
        private T[] MatrixInternal;
        public int Rows { get; private set; }
        public int Cols { get; private set; }

        public Matrix(int[,] input)
        {
            Rows = input.GetLength(0);
            Cols = input.GetLength(1);
            MatrixInternal = new T[Rows * Cols + PaddingFromBegin];

            Parallel.ForEach(GetRowChunks(), (pair) => 
            {
                for (int i = pair.Item1; i < pair.Item2; i++)
                {
                    for (int j = 0; j < Cols; j++)
                    {
                        MatrixInternal[(i * Cols) + j + PaddingFromBegin] = (T)new T().AddInt(input[i, j]);
                    }
                }
            });
        }

        public Matrix(double[,] input)
        {
            Rows = input.GetLength(0);
            Cols = input.GetLength(1);
            MatrixInternal = new T[Rows * Cols + PaddingFromBegin];

            Parallel.ForEach(GetRowChunks(), (pair) =>
            {
                for (int i = pair.Item1; i < pair.Item2; i++)
                {
                    for (int j = 0; j < Cols; j++)
                    {
                        MatrixInternal[(i * Cols) + j + PaddingFromBegin] = (T)new T().AddDouble(input[i, j]);
                    }
                }
            });
        }

        public Matrix(T[,] input)
        {
            Rows = input.GetLength(0);
            Cols = input.GetLength(1);
            MatrixInternal = new T[Rows * Cols + PaddingFromBegin];

            Parallel.ForEach(GetRowChunks(), (pair) =>
            {
                for (int i = pair.Item1; i < pair.Item2; i++)
                {
                    for (int j = 0; j < Cols; j++)
                    {
                        MatrixInternal[(i * Cols) + j + PaddingFromBegin] = (T)input[i, j].Copy();
                    }
                }
            });
        }

        public Matrix(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            MatrixInternal = new T[Rows * Cols + PaddingFromBegin];

#if !NULLS_IN_MATRIX
            Parallel.ForEach(GetRowChunks(), (pair) =>
            {
                for (int i = pair.Item1; i < pair.Item2; ++i)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        MatrixInternal[(i * Cols) + j + PaddingFromBegin] = new T();
                    }
                }
            });
#endif
        }

        public Matrix(Matrix<T> matrix)
        {
            Rows = matrix.Rows;
            Cols = matrix.Cols;
            MatrixInternal = new T[Rows * Cols + PaddingFromBegin];

            Parallel.ForEach(GetRowChunks(), (pair) =>
            {
                for (int i = pair.Item1; i < pair.Item2; i++)
                {
                    for (int j = 0; j < Cols; j++)
                    {
                        MatrixInternal[(i * Cols) + j + PaddingFromBegin] = (T)matrix.GetNumber(i, j).Copy();
                    }
                }
            });
        }

        public T GetNumber(int i, int j)
        {
            if (i >= Rows || j >= Cols) { throw new MatrixLibraryException("Bad indices specified!"); }

#if NULLS_IN_MATRIX
            if (ReferenceEquals(MatrixInternal[(i * Cols) + j + PaddingFromBegin], null)) { MatrixInternal[(i * Cols) + j + PaddingFromBegin] = new T(); }
#endif
            return MatrixInternal[(i * Cols) + j + PaddingFromBegin];
        }
        public T[] GetRow(int row)
        {
            if (row >= Rows) { throw new MatrixLibraryException("Bad index of row specified!"); }

            T[] result = new T[Cols];
            for (int i = 0; i < Cols; i++)
            {
#if NULLS_IN_MATRIX
                if (ReferenceEquals(MatrixInternal[(row * Cols) + i + PaddingFromBegin], null)) { MatrixInternal[(row * Cols) + i + PaddingFromBegin] = new T(); }
#endif
                result[i] = MatrixInternal[(row * Cols) + i + PaddingFromBegin];
            }
            return result;
        }
        public T[] GetCol(int col)
        {
            if (col >= Cols) { throw new MatrixLibraryException("Bad index of column specified!"); }

            T[] result = new T[Rows];
            for (int i = 0; i < Rows; i++)
            {
#if NULLS_IN_MATRIX
                if (ReferenceEquals(MatrixInternal[(i * Cols) + col + PaddingFromBegin], null)) { MatrixInternal[(i * Cols) + col + PaddingFromBegin] = new T(); }
#endif
                result[i] = MatrixInternal[(i * Cols) + col + PaddingFromBegin];
            }
            return result;
        }
        public void WriteNumber(int i, int j, T number)
        {
            MatrixInternal[(i * Cols) + j + PaddingFromBegin] = (T)number.Copy();
        }
        public void WriteRow(int i, T[] row)
        {
            if (row.Length != Cols) { throw new MatrixLibraryException("Row does not have the same number of cols as matrix!"); }
            if (i >= Rows) { throw new MatrixLibraryException("Bad index of row specified!"); }

            for (int j = 0; j < row.Length; ++j)
            {
                MatrixInternal[(i * Cols) + j + PaddingFromBegin] = (T)row[j].Copy();
            }
        }
        public void WriteCol(int j, T[] col)
        {
            if (col.Length != Rows) { throw new MatrixLibraryException("Col does not have the same number of rows as matrix!"); }
            if (j >= Cols) { throw new MatrixLibraryException("Bad index of column specified!"); }

            for (int i = 0; i < col.Length; ++i)
            {
                MatrixInternal[(i * Cols) + j + PaddingFromBegin] = (T)col[i].Copy();
            }
        }
        public void SwapElements(int firstRow, int firstCol, int secondRow, int secondCol)
        {
            if (firstRow >= Rows || secondRow >= Rows || firstCol >= Cols || secondCol >= Cols) { throw new MatrixLibraryException("Bad indices specified!"); }

            T temp = MatrixInternal[(firstRow * Cols) + firstCol + PaddingFromBegin];
            MatrixInternal[(firstRow * Cols) + firstCol + PaddingFromBegin] = MatrixInternal[(secondRow * Cols) + secondCol + PaddingFromBegin];
            MatrixInternal[(secondRow * Cols) + secondCol + PaddingFromBegin] = temp;
        }

        public IEnumerable<Tuple<int, int>> GetRowChunks()
        {
            int chunkCount = 2 * Environment.ProcessorCount;
            int chunkLength = Rows / chunkCount;

            if (chunkCount >= Rows) { yield return new Tuple<int, int>(0, Rows); }
            else
            {
                for (int i = 0; i < Rows; i += chunkLength)
                {
                    int end = i + chunkLength;
                    if (end > Rows) { end = Rows; }

                    yield return new Tuple<int, int>(i, end);
                }
            }
        }

        public static int PaddingFromBegin = 32; // Taken from this: http://blog.mischel.com/2011/12/29/more-about-cache-contention/
        public static Matrix<T> IdentityMatrix(int dimension) // Jednotková matice
        {
            int[,] mat = new int[dimension, dimension];
            for (int i = 0; i < dimension; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    if (i == j) { mat[i, j] = 1; }
                    else { mat[i, j] = 0; }
                }
            }
            Matrix<T> result = new Matrix<T>(mat);

            return result;
        }
        public static Matrix<T> ZeroMatrix(int rows, int cols) // Nulová matice
        {
            int[,] mat = new int[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    mat[i, j] = 0;
                }
            }
            Matrix<T> result = new Matrix<T>(mat);

            return result;
        }

        public override string ToString()
        {
            string result = "";

            for (int i = 0; i < Rows; ++i)
            {
                for (int j = 0; j < Cols; ++j)
                {
                    result += string.Format("{0,4} ", MatrixInternal[(i * Cols) + j + PaddingFromBegin].ToDouble());
                }
                result += System.Environment.NewLine;
            }

            return result;
        }
        public override bool Equals(object obj)
        {
            return (this == (Matrix<T>)obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public static Matrix<T> operator +(Matrix<T> first, Matrix<T> second)
        {
            return ClassicOperations.Addition(first, second);
        }
        public static Matrix<T> operator -(Matrix<T> first, Matrix<T> second)
        {
            return ClassicOperations.Subtraction(first, second);
        }
        public static Matrix<T> operator *(Matrix<T> first, Matrix<T> second)
        {
            return ClassicOperations.Multiplication(first, second);
        }
        public static bool operator ==(Matrix<T> first, Matrix<T> second)
        {
            if (ReferenceEquals(first, second)) { return true; }
            if (ReferenceEquals(first, null) || ReferenceEquals(first, null)) { return false; }

            bool result = true;
            if (first.Rows == second.Rows && first.Cols == second.Cols)
            {
                for (int i = 0; i < first.Rows; i++)
                {
                    for (int j = 0; j < first.Cols; j++)
                    {
                        if (first.GetNumber(i, j) != second.GetNumber(i, j)) { result = false; }
                    }
                }
            }
            else { result = false; }
            return result;
        }
        public static bool operator !=(Matrix<T> first, Matrix<T> second)
        {
            if (ReferenceEquals(first, second)) { return false; }
            if (ReferenceEquals(first, null) || ReferenceEquals(first, null)) { return false; }

            bool result = false;
            if (first.Rows == second.Rows && first.Cols == second.Cols)
            {
                for (int i = 0; i < first.Rows; i++)
                {
                    for (int j = 0; j < first.Cols; j++)
                    {
                        if (first.GetNumber(i, j) == second.GetNumber(i, j)) { result = true; break; }
                    }
                    if (result == true) { break; }
                }
            }
            else { result = true; }
            return result;
        }

        public static T[] DoVectorOperation(T[] first, T[] second, Func<MatrixNumberBase, MatrixNumberBase, MatrixNumberBase> operation)
        {
            if (first.Length != second.Length) { throw new MatrixLibraryException("Vectors do not have the same length!"); }

            T[] result = new T[first.Length];

            for (int i = 0; i < first.Length; ++i)
            {
                result[i] = (T)operation((MatrixNumberBase)first[i], (MatrixNumberBase)second[i]);
            }

            return result;
        }
    }
}