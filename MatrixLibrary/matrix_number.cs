using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLibrary
{
    /// <summary>
    /// Interface for classes or structures which Matrix class and related functions can use.
    /// </summary>
    /// <remarks>
    /// This interface declare mostly function for calculation with internal datas of derived classes or structures.
    /// Derived classes or structures should have default constructors which initializes internal data to equivalent of integral zero.
    /// </remarks>
    public interface IMatrixNumber
    {
        /// <summary>
        /// Add integer value to internal data.
        /// </summary>
        /// <param name="add">Value which will be added to current object.</param>
        /// <returns>Usually returns this object, which has modified internal data.</returns>
        IMatrixNumber AddInt(int add);
        /// <summary>
        /// To current object add double value.
        /// </summary>
        /// <param name="add">Value which will be added to current object.</param>
        /// <returns>Returns this object.</returns>
        IMatrixNumber AddDouble(double add);
        /// <summary>
        /// Creates new object of the same type as current class and copy internal data from current object to newly created one.
        /// </summary>
        /// <returns>New object which complies IMatrixNumber interface.</returns>
        IMatrixNumber Copy();
        /// <summary>
        /// From <paramref name="number"/> copy internal data to current object and rewrite old data.
        /// </summary>
        /// <param name="number">Source for the copying.</param>
        void CopyFrom(IMatrixNumber number);
        /// <summary>
        /// Converts internal data to one single real number.
        /// </summary>
        /// <returns>Double which reprezents internal datas.</returns>
        double ToDouble();
        /// <summary>
        /// Determine whether this object and its internal datas equals to integral zero.
        /// </summary>
        /// <returns>True if its equal with zero, false otherwise.</returns>
        bool IsZero();
        /// <summary>
        /// Find if this object and its data is equal to integral reprezentation of one.
        /// </summary>
        /// <returns>True, if this object is equal to one, or false otherwise.</returns>
        bool IsOne();

        /// <summary>
        /// To the newly created instance of number place result of addition of current object and <paramref name="second"/>.
        /// </summary>
        /// <param name="second">Summand.</param>
        /// <returns>Created instance of summed number.</returns>
        IMatrixNumber __Addition(IMatrixNumber second);
        /// <summary>
        /// Creates and returns new instance of current class and place result of subtraction of current object and <paramref name="second"/>.
        /// </summary>
        /// <param name="second">Subtrahent.</param>
        /// <returns>New instance of subtracted number.</returns>
        IMatrixNumber __Subtraction(IMatrixNumber second);
        /// <summary>
        /// To the new instance of current class place result of multiplication od current object and <paramref name="second"/>.
        /// </summary>
        /// <param name="second">Multiplier.</param>
        /// <returns>Created instance of multiplicated number.</returns>
        IMatrixNumber __Multiplication(IMatrixNumber second);
        /// <summary>
        /// To the new instance of current class place division of current object and <paramref name="second"/>.
        /// </summary>
        /// <param name="second">Divisor.</param>
        /// <returns>Created instance of divided number.</returns>
        IMatrixNumber __Division(IMatrixNumber second);
        /// <summary>
        /// Creates new instance of current class and its value will be exponentiated compared to current object.
        /// </summary>
        /// <param name="exponent">Exponent which will be used.</param>
        /// <returns>New instance which has exponentiated value.</returns>
        IMatrixNumber __Exponentiate(int exponent);
        /// <summary>
        /// Creates and returns new instance of current class which data will be square rooted compared to current object.
        /// </summary>
        /// <returns>New instance of square rooted number.</returns>
        IMatrixNumber __SquareRoot();
        /// <summary>
        /// Creates new instance of current class which will have opposite values of internal data than current object.
        /// </summary>
        /// <returns>New created object fulfilling IMatrixNumber interface.</returns>
        IMatrixNumber __Negate();
        /// <summary>
        /// Compare current object and <paramref name="second"/> on equality.
        /// </summary>
        /// <param name="second">Object to compare</param>
        /// <returns>True, if internal data of current object and <paramref name="second"/> is equal.</returns>
        bool __IsEqual(IMatrixNumber second);
        /// <summary>
        /// Compare current object with parameter <paramref name="second"/> and returns true if its lesser.
        /// </summary>
        /// <param name="second">Object to compare.</param>
        /// <returns>True, if current object is less than <paramref name="second"/>, false otherwise.</returns>
        bool __IsLessThan(IMatrixNumber second);
        /// <summary>
        /// Compare current object to <paramref name="second"/> and tells if its greater.
        /// </summary>
        /// <param name="second">Compared object.</param>
        /// <returns>True, if current object is greater than <paramref name="second"/>, false otherwise.</returns>
        bool __IsGreaterThan(IMatrixNumber second);
        /// <summary>
        /// Internal to string method, which should be called within overriden ToString() method.
        /// </summary>
        /// <returns>Textual reprezentation of internal data.</returns>
        string __ToString();
    }

    /// <summary>
    /// Particular implementation of IMatrixNumber interface.
    /// Its a structure which stores only one integer as internal reprezentation of number.
    /// </summary>
    /// <remarks>
    /// Considering that this is a structure, gives us default parameterless constructor, which initializes internal data to integral zero.
    /// </remarks>
    public struct MatrixNumberInt : IMatrixNumber
    {
        /// <summary>
        /// Internal data of this class consist only of this integer value.
        /// </summary>
        public int Number { get; private set; }
        /// <summary>
        /// Constructor which takes integer as parameter which will be stored in internal integer.
        /// </summary>
        /// <param name="number">Initial number value of structure.</param>
        public MatrixNumberInt(int number) : this() { Number = number; }

        /// <summary>
        /// Creates new instance of this structure with the same data as current struct.
        /// Because its a structure all what have to be done is returns this statement.
        /// </summary>
        /// <returns>Returns this statement, which will create new instance, because its a structure.</returns>
        public IMatrixNumber Copy()
        {
            return this;
        }
        /// <summary>
        /// Copies internal reprezentation of given <paramref name="number"/> to current object.
        /// Original data will be forgotten.
        /// </summary>
        /// <param name="number">Source object.</param>
        public void CopyFrom(IMatrixNumber number)
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
        /// <summary>
        /// Internal to string function which is used in actual overriden ToString() function.
        /// </summary>
        /// <returns>Textual reprezentation of internal data.</returns>
        public string __ToString()
        {
            return Number.ToString();
        }
        /// <summary>
        /// Classical ToString() method which is inherited from object. Inside it, __ToString() function is called.
        /// </summary>
        /// <returns>Textual reprezentation of internal data.</returns>
        public override string ToString()
        {
            return __ToString();
        }
        /// <summary>
        /// Casts internal integer to double and returns it.
        /// </summary>
        /// <returns>Integer casted to double.</returns>
        public double ToDouble()
        {
            return (double)Number;
        }

        /// <summary>
        /// Determine whether internal integer value is equal to zero.
        /// </summary>
        /// <returns>True, if its equal to zero, false otherwise.</returns>
        public bool IsZero()
        {
            return Number == 0;
        }
        /// <summary>
        /// Tells, if internal integer is equal to one or not.
        /// </summary>
        /// <returns>True, if its equal to one, false otherwise.</returns>
        public bool IsOne()
        {
            return Number == 1;
        }
        /// <summary>
        /// To internal integer adds value of parameter <paramref name="add"/>.
        /// </summary>
        /// <param name="add">Value to be added.</param>
        /// <returns>This object, which will be copied, because its a structure.</returns>
        public IMatrixNumber AddInt(int add)
        {
            Number += add;
            return this;
        }
        /// <summary>
        /// Internal value will be increased by value in <paramref name="add"/> parameter.
        /// </summary>
        /// <param name="add">Double value which will be added.</param>
        /// <returns>This copied object (Its a structure).</returns>
        public IMatrixNumber AddDouble(double add)
        {
            Number += (int)add;
            return this;
        }

        /// <summary>
        /// Takes current structure and <paramref name="second"/> and tries to summarize them.
        /// On <paramref name="second"/> object is called ToDouble() function, whose result is added to current internal integer.
        /// </summary>
        /// <param name="second">Summand.</param>
        /// <returns>New instance of this class in which will be stored result of addition.</returns>
        public IMatrixNumber __Addition(IMatrixNumber second)
        {
            if (ReferenceEquals(second, null)) { return Copy(); }
            return new MatrixNumberInt(Number + (int)second.ToDouble());
        }
        /// <summary>
        /// From current structure is subtracted internal value of <paramref name="second"/> object and result is stored in new structure.
        /// To get data from <paramref name="second"/> is used ToDouble() method.
        /// </summary>
        /// <param name="second">Subtrahend.</param>
        /// <returns>Newly created MatrixNumberInt structure.</returns>
        public IMatrixNumber __Subtraction(IMatrixNumber second)
        {
            if (ReferenceEquals(second, null)) { return Copy(); }
            return new MatrixNumberInt(Number - (int)second.ToDouble());
        }
        /// <summary>
        /// Creates new MatrixNumberInt structure in which will be result of multiplication of current structure and <paramref name="second"/>.
        /// Data from <paramref name="second"/> is obtain by calling ToDouble() function.
        /// </summary>
        /// <param name="second">Multiplier.</param>
        /// <returns>MatrixNumberInt structure with result of multiplication.</returns>
        public IMatrixNumber __Multiplication(IMatrixNumber second)
        {
            if (ReferenceEquals(second, null)) { return Copy(); }
            return new MatrixNumberInt(Number * (int)second.ToDouble());
        }
        /// <summary>
        /// To the new MatrixNumberInt add result of division of current structure and <paramref name="second"/> object.
        /// Data from <paramref name="second"/> is received through ToDouble() function. 
        /// </summary>
        /// <param name="second">Divisor.</param>
        /// <returns>New MatrixNumberInt structure with result of division.</returns>
        public IMatrixNumber __Division(IMatrixNumber second)
        {
            if (ReferenceEquals(second, null)) { return Copy(); }
            return new MatrixNumberInt(Number / (int)second.ToDouble());
        }
        /// <summary>
        /// Newly created structure is filled with exponentiation of current structure and <paramref name="exponent"/> parameter.
        /// After that is created structure returned.
        /// </summary>
        /// <param name="exponent">Exponent which will be used to exponentiate current internal value.</param>
        /// <returns>Newly created structure.</returns>
        public IMatrixNumber __Exponentiate(int exponent)
        {
            return new MatrixNumberInt((int)Math.Pow((double)Number, (double)exponent));
        }
        /// <summary>
        /// Creates new instance of current structure and fill it with opposite integral value of current internal data.
        /// </summary>
        /// <returns>Opposite number.</returns>
        public IMatrixNumber __Negate()
        {
            return new MatrixNumberInt(-Number);
        }
        /// <summary>
        /// Current internal value is square rooted and new value is stored in newly created MatrixNumberInt structure.
        /// </summary>
        /// <returns>New structure with square rooted internal integer.</returns>
        public IMatrixNumber __SquareRoot()
        {
            return new MatrixNumberInt((int)Math.Sqrt(Number));
        }
        /// <summary>
        /// Detect equality of current internal data and data from <paramref name="second"/>.
        /// Value from <paramref name="second"/> is obtain from ToDouble() method.
        /// </summary>
        /// <param name="second">Second parameter.</param>
        /// <returns>True, if values is equal, false otherwise.</returns>
        public bool __IsEqual(IMatrixNumber second)
        {
            if (ReferenceEquals(this, second)) { return true; }
            if (ReferenceEquals(this, null) || ReferenceEquals(second, null)) { return false; }
            return Number == second.ToDouble();
        }
        /// <summary>
        /// Determine whether current structure is less than <paramref name="second"/> object.
        /// Value from <paramref name="second"/> is received through ToDouble() function.
        /// </summary>
        /// <param name="second">Parameter to compare to.</param>
        /// <returns>True, if current value is less than value in <paramref name="second"/> object.</returns>
        public bool __IsLessThan(IMatrixNumber second)
        {
            if (ReferenceEquals(this, second)) { return false; }
            if (ReferenceEquals(this, null) || ReferenceEquals(second, null)) { return false; }
            return Number < second.ToDouble();
        }
        /// <summary>
        /// Find if current internal data is greater than in <paramref name="second"/> parameter.
        /// Internal data from <paramref name="second"/> is gained through ToDouble() method.
        /// </summary>
        /// <param name="second"></param>
        /// <returns>True if internal integral value is greater than value in <paramref name="second"/>.</returns>
        public bool __IsGreaterThan(IMatrixNumber second)
        {
            if (ReferenceEquals(this, second)) { return false; }
            if (ReferenceEquals(this, null) || ReferenceEquals(second, null)) { return false; }
            return Number > second.ToDouble();
        }
    }

    /// <summary>
    /// Implementation of IMatrixNumber interface. This structure stores three values as internal data.
    /// </summary>
    /// <remarks>
    /// First two integral values are Numerator and Denominator. Third one is double RealPart.
    /// This data reprezents fraction and remainder (Number = Numerator/Denominator + RealPart).
    /// </remarks>
    public struct MatrixNumber : IMatrixNumber
    {
        /// <summary>
        /// Creates new struct whose internal datas will have value of <paramref name="number"/> parameter.
        /// </summary>
        /// <param name="number">Default value of newly created MatrixNumber.</param>
        public MatrixNumber(int number) : this()
        {
            Numerator = number;
            if (number != 0) { Denominator = 1; }
            else { Denominator = 0; }
            RealPart = 0;
        }
        /// <summary>
        /// Constructor which will fill <paramref name="number"/> to internal structure data.
        /// </summary>
        /// <param name="number">Default value of newly created MatrixNumber.</param>
        public MatrixNumber(double number) : this()
        {
            Numerator = 0;
            Denominator = 0;
            RealPart = number;
            Regulate();
        }
        /// <summary>
        /// Specific constructor, which will store given <paramref name="numerator"/> and <paramref name="denominator"/> in suitable internal datas.
        /// </summary>
        /// <param name="numerator">Number which will be given to internal Numerator.</param>
        /// <param name="denominator">Number given to internal Denominator.</param>
        public MatrixNumber(int numerator, int denominator) : this()
        {
            Numerator = numerator;
            Denominator = denominator;
            RealPart = 0;
        }
        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="source">Structure from which will be internal data copied.</param>
        public MatrixNumber(MatrixNumber source) : this()
        {
            Numerator = source.Numerator;
            Denominator = source.Denominator;
            RealPart = source.RealPart;
        }

        /// <summary>
        /// Computes real value of internal datas.
        /// </summary>
        /// <returns>Double which reprezents internal data.</returns>
        public double ToDouble()
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
        /// <summary>
        /// Copies internal data to the newly created MatrixNumber structure.
        /// Because this is a structure, than its good enough to return this statement.
        /// </summary>
        /// <returns>Returns new instance of structure.</returns>
        public IMatrixNumber Copy()
        {
            return this;
        }
        /// <summary>
        /// Datas from parameter <paramref name="number"/> copies to the current structure.
        /// </summary>
        /// <param name="number">Source of the data.</param>
        public void CopyFrom(IMatrixNumber number)
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

        /// <summary>
        /// This function regulates the reprezentation of number in this structure to the more suitable form.
        /// </summary>
        /// <remarks>
        /// First of all its called private method DecomposeRealPart(), which determines if RealPart member is close enough to integral value.
        /// If it is, than the RealPart is set to zero and integral value is stored in Numerator member.
        /// After DecomposeRealPart() function is inspected the fraction Numerator/Denominator and if it has greatest common divisor not equal to zero or one, than both of them is divided with the common divisor.
        /// </remarks>
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
        /// <summary>
        /// Reprezents classic numerator in fraction.
        /// </summary>
        public int Numerator { get; private set; }
        /// <summary>
        /// Reprezents classic denominator in fraction.
        /// </summary>
        public int Denominator { get; private set; }
        /// <summary>
        /// Reprezents real part which is added to fraction which is constisting of Numerator/Denominator.
        /// </summary>
        public double RealPart { get; private set; } // Iracionální čísla (přičítá se ke zlomku) reprezentovaná doublem

        /// <summary>
        /// To current structure add value of parameter <paramref name="add"/>.
        /// </summary>
        /// <param name="add">Value which will be added to current structure.</param>
        /// <returns>This statement is returned.</returns>
        public IMatrixNumber AddInt(int add)
        {
            MatrixNumber tmp = (MatrixNumber)__Addition(new MatrixNumber(add));
            Numerator = tmp.Numerator;
            Denominator = tmp.Denominator;
            RealPart = tmp.RealPart;
            return this;
        }
        /// <summary>
        /// Value of parameter <paramref name="add"/> will be added to current structure.
        /// </summary>
        /// <param name="add">Real value which will be added to current structure.</param>
        /// <returns>This statement is returned.</returns>
        public IMatrixNumber AddDouble(double add)
        {
            MatrixNumber tmp = (MatrixNumber)__Addition(new MatrixNumber(add));
            Numerator = tmp.Numerator;
            Denominator = tmp.Denominator;
            RealPart = tmp.RealPart;
            return this;
        }

        static private MatrixNumber Zero = new MatrixNumber();
        /// <summary>
        /// Determines whether current structure is equal to zero or not.
        /// </summary>
        /// <returns>True if its equal to zero, false otherwise.</returns>
        public bool IsZero()
        {
            return (this.__IsEqual(Zero));
        }

        static private MatrixNumber One = new MatrixNumber(1);
        /// <summary>
        /// Determines whether current structure is equal to integral one or not.
        /// </summary>
        /// <returns>True if its equal to one, false otherwise.</returns>
        public bool IsOne()
        {
            return (this.__IsEqual(One));
        }

        /// <summary>
        /// Takes current structure and <paramref name="second"/> and summarize them.
        /// </summary>
        /// <remarks>
        /// If <paramref name="second"/> is MatrixNumber, then its summed internal values as it should be.
        /// Otherwise to current RealPart member is added the result from ToDouble() method on <paramref name="second"/>.
        /// </remarks>
        /// <param name="second">Summand.</param>
        /// <returns>Returns result of addition, which is MatrixNumber structure.</returns>
        public IMatrixNumber __Addition(IMatrixNumber second)
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
        /// <summary>
        /// Creates new MatrixNumber structure which will contain subtraction of current structure and <paramref name="second"/>.
        /// </summary>
        /// <remarks>
        /// If <paramref name="second"/> is MatrixNumber, then internal values are subtracted in the way of fractions.
        /// Otherwise current RealPart member is subtracted from the result of ToDouble() method on <paramref name="second"/>.
        /// </remarks>
        /// <param name="second">Subtrahend which complies IMatrixNumber interface.</param>
        /// <returns>Newly created MatrixNumber structure is returned.</returns>
        public IMatrixNumber __Subtraction(IMatrixNumber second)
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
        /// <summary>
        /// Return new MatrixNumber with multiplication of current structure and <paramref name="second"/>.
        /// </summary>
        /// <remarks>
        /// If <paramref name="second"/> is MatrixNumber, then internal values are multiplied in the way of fractions.
        /// Otherwise current RealPart member is multiplied with the result of ToDouble() method on <paramref name="second"/>.
        /// </remarks>
        /// <param name="second"></param>
        /// <returns></returns>
        public IMatrixNumber __Multiplication(IMatrixNumber second)
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
        /// <summary>
        /// Returns new MatrixNumber in which will be stored division of current structure and <paramref name="second"/>.
        /// </summary>
        /// <remarks>
        /// If <paramref name="second"/> is MatrixNumber, then internal values are divided in the way of fractions.
        /// Otherwise current RealPart member is divided by the result of ToDouble() method on <paramref name="second"/>.
        /// </remarks>
        /// <param name="second">Divisor.</param>
        /// <returns>New instance of MatrixNumber with divided number.</returns>
        public IMatrixNumber __Division(IMatrixNumber second)
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
        /// <summary>
        /// Takes datas from current structure and exponentiates it with given <paramref name="exponent"/>.
        /// </summary>
        /// <param name="exponent">Exponent which will be used in exponentiation.</param>
        /// <returns>Exponentiated MatrixNumber which is newly created.</returns>
        public IMatrixNumber __Exponentiate(int exponent)
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
        /// <summary>
        /// Returns square rooted data from current structure in new instance.
        /// </summary>
        /// <returns>Instance of MatrixNumber with square rooted value in it.</returns>
        public IMatrixNumber __SquareRoot()
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
        /// <summary>
        /// Create new structure and fill it with opposite value than current structure have.
        /// </summary>
        /// <returns>Newly created structure fulfilling IMatrixNumber interface.</returns>
        public IMatrixNumber __Negate()
        {
            MatrixNumber result = new MatrixNumber();
            result.Numerator = (-1) * Numerator;
            result.RealPart = (double)(-1) * RealPart;
            result.Denominator = Denominator;
            return result;
        }
        /// <summary>
        /// Determines whether current structure and <paramref name="second"/> is equal or not.
        /// </summary>
        /// <param name="second">Second comparand.</param>
        /// <returns>True if current structure is equal to <paramref name="second"/>, false otherwise.</returns>
        public bool __IsEqual(IMatrixNumber second)
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
        /// <summary>
        /// Determines whether current structure is less than <paramref name="second"/> or not.
        /// </summary>
        /// <param name="second">Second comparand.</param>
        /// <returns>True if current structure is less than <paramref name="second"/>, false otherwise.</returns>
        public bool __IsLessThan(IMatrixNumber second)
        {
            if (ReferenceEquals(this, second)) { return false; }
            if (ReferenceEquals(this, null) || ReferenceEquals(second, null)) { return false; }

            double first_d, second_d;
            first_d = ToDouble();
            second_d = second.ToDouble();

            if (first_d < second_d) { return true; }
            else { return false; }
        }
        /// <summary>
        /// Determines whether current structure is greater than <paramref name="second"/> or not.
        /// </summary>
        /// <param name="second">Second comparand.</param>
        /// <returns>True if current structure is greater, false otherwise.</returns>
        public bool __IsGreaterThan(IMatrixNumber second)
        {
            if (ReferenceEquals(this, second)) { return false; }
            if (ReferenceEquals(this, null) || ReferenceEquals(second, null)) { return false; }

            double first_d, second_d;
            first_d = ToDouble();
            second_d = second.ToDouble();

            if (first_d > second_d) { return true; }
            else { return false; }
        }
        /// <summary>
        /// Returns reprezentation of current structure.
        /// </summary>
        /// <returns>String value with textual reprezentation.</returns>
        public string __ToString()
        {
            return ToDouble().ToString();
        }

        /// <summary>
        /// Implementation of function inherited from object. It returns textual reprezentation of current structure.
        /// </summary>
        /// <remarks>
        /// Within this method __ToString() function is called.
        /// </remarks>
        /// <returns>Textual reprezentation of current structure.</returns>
        public override string ToString()
        {
            return __ToString();
        }
        /// <summary>
        /// Implementation of inherited function from object.
        /// </summary>
        /// <remarks>
        /// Function __IsEqual() on current structure is called inside this method.
        /// </remarks>
        /// <param name="obj">Compared object.</param>
        /// <returns>True if object are same, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            return (this.__IsEqual((MatrixNumber)obj));
        }
        /// <summary>
        /// Implementation of function inherited from object.
        /// </summary>
        /// <returns>Hashed integer value.</returns>
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

        /// <summary>
        /// Summarize <paramref name="first"/> and <paramref name="second"/> and returns result;
        /// </summary>
        /// <remarks>
        /// Inside this method function __Addition() on <paramref name="first"/> structure is called.
        /// </remarks>
        /// <param name="first">First summand.</param>
        /// <param name="second">Second summand.</param>
        /// <returns>MatrixNumber structure with sum of <paramref name="first"/> and <paramref name="second"/>.</returns>
        public static MatrixNumber operator +(MatrixNumber first, MatrixNumber second)
        {
            return (MatrixNumber)first.__Addition(second);
        }
        /// <summary>
        /// Subtract <paramref name="first"/> and <paramref name="second"/> and returns result.
        /// </summary>
        /// <remarks>
        /// In this method is called function __Subtraction() on <paramref name="first"/> structure.
        /// </remarks>
        /// <param name="first">Minuend.</param>
        /// <param name="second">Subtrahend.</param>
        /// <returns>This structure which is result of subtraction of <paramref name="first"/> and <paramref name="second"/>.</returns>
        public static MatrixNumber operator -(MatrixNumber first, MatrixNumber second)
        {
            return (MatrixNumber)first.__Subtraction(second);
        }

        /// <summary>
        /// Determines whether <paramref name="first"/> and <paramref name="second"/> are equal.
        /// </summary>
        /// <remarks>
        /// Within this method __IsEqual() function is called.
        /// </remarks>
        /// <param name="first">First compared parameter.</param>
        /// <param name="second">Second compared parameter</param>
        /// <returns>True if <paramref name="first"/> and <paramref name="second"/> are equal, false otherwise.</returns>
        public static bool operator ==(MatrixNumber first, MatrixNumber second)
        {
            return first.__IsEqual(second);
        }
        /// <summary>
        /// Determines whether <paramref name="first"/> and <paramref name="second"/> are not equal.
        /// </summary>
        /// <remarks>
        /// Within this method __IsEqual() function is called.
        /// </remarks>
        /// <param name="first">First compared parameter.</param>
        /// <param name="second">Second compared parameter.</param>
        /// <returns>True if <paramref name="first"/> and <paramref name="second"/> are not equal, false otherwise.</returns>
        public static bool operator !=(MatrixNumber first, MatrixNumber second)
        {
            return !first.__IsEqual(second);
        }
    }
}