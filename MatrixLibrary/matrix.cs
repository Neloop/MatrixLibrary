#define UNCHECKED_BOUNDARIES

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLibrary
{
    /// <summary>
    /// General exception of this library which is thrown in functions which works with Matrix objects.
    /// </summary>
    public class MatrixLibraryException : Exception
    {
        /// <summary>
        /// Basic parameterless constructor with no further information about exception.
        /// </summary>
        public MatrixLibraryException() { }
        /// <summary>
        /// Initialize exception with additional informations about it.
        /// </summary>
        /// <param name="message">Further informations about exception.</param>
        public MatrixLibraryException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of exception with a additional informations and a reference to the inner exception.
        /// </summary>
        /// <param name="message">Further informations about exception.</param>
        /// <param name="inner">Inner exception which caused this exception to be thrown.</param>
        public MatrixLibraryException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>
    /// More specified exception than MatrixLibraryException which is thrown in methods which compute EigenValues.
    /// </summary>
    public class EigenValuesNotFoundException : Exception
    {
        /// <summary>
        /// Initialize a basic exception object with no further information about causes of throwing.
        /// </summary>
        public EigenValuesNotFoundException() { }
        /// <summary>
        /// Initialize new exception with additional informations about the problem.
        /// </summary>
        /// <param name="message">Further informations about exception.</param>
        public EigenValuesNotFoundException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of exception with a additional informations and a reference to the inner exception.
        /// </summary>
        /// <param name="message">Further informations about exception.</param>
        /// <param name="inner">Inner exception which caused this exception to be thrown.</param>
        public EigenValuesNotFoundException(string message, Exception inner) : base(message, inner) { }
    }
    
    /// <summary>
    /// Its a read-only container which stores array of eigen values. These values cannot be modified or deleted a has to be given to the class during construction.
    /// </summary>
    /// <typeparam name="T">Type of number stored in this container. Must comply IMatrixNumber interface and must have parameterless constructor.</typeparam>
    public class EigenValues<T> where T : IMatrixNumber, new()
    {
        T[] EigenValuesInternal;

        /// <summary>
        /// Only possible constructor which initializes new instance of this class with specified eigen values.
        /// </summary>
        /// <param name="input">Simple array of numbers which will be stored in this class.</param>
        public EigenValues(T[] input)
        {
            EigenValuesInternal = new T[input.Length];

            for (int i = 0; i < input.Length; i++)
            {
                EigenValuesInternal[i] = (T)input[i].Copy();
            }
        }

        /// <summary>
        /// Get one eigen value which is on specified index in internal array.
        /// </summary>
        /// <param name="i">Index in internal array, it start on 0.</param>
        /// <returns>Returns a copy of eigen value on specified index.</returns>
        public T GetEigenValue(int i)
        {
            return (T)EigenValuesInternal[i].Copy();
        }

        /// <summary>
        /// Returns number of eigen values stored in this object.
        /// </summary>
        /// <returns>Integral value which reprezents number of values stored in this object.</returns>
        public int Count()
        {
            return EigenValuesInternal.Length;
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <typeparam name="T">Type of numbers which will be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
    public class Matrix<T> where T : IMatrixNumber, new()
    {
        private T[] MatrixInternal;
        /// <summary>
        /// 
        /// </summary>
        public int Rows { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public int Cols { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        public Matrix(int[,] input)
        {
            Rows = input.GetLength(0);
            Cols = input.GetLength(1);
            MatrixInternal = new T[Rows * Cols * PaddingBetweenNumbers + PaddingFromBegin];

            Parallel.ForEach(GetRowsChunks(), (pair) => 
            {
                for (int i = pair.Item1; i < pair.Item2; i++)
                {
                    for (int j = 0; j < Cols; j++)
                    {
                        MatrixInternal[(i * Cols + j) * PaddingBetweenNumbers + PaddingFromBegin] = (T)new T().AddInt(input[i, j]);
                    }
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        public Matrix(double[,] input)
        {
            Rows = input.GetLength(0);
            Cols = input.GetLength(1);
            MatrixInternal = new T[Rows * Cols * PaddingBetweenNumbers + PaddingFromBegin];

            Parallel.ForEach(GetRowsChunks(), (pair) =>
            {
                for (int i = pair.Item1; i < pair.Item2; i++)
                {
                    for (int j = 0; j < Cols; j++)
                    {
                        MatrixInternal[(i * Cols + j) * PaddingBetweenNumbers + PaddingFromBegin] = (T)new T().AddDouble(input[i, j]);
                    }
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        public Matrix(T[,] input)
        {
            Rows = input.GetLength(0);
            Cols = input.GetLength(1);
            MatrixInternal = new T[Rows * Cols * PaddingBetweenNumbers + PaddingFromBegin];

            Parallel.ForEach(GetRowsChunks(), (pair) =>
            {
                for (int i = pair.Item1; i < pair.Item2; i++)
                {
                    for (int j = 0; j < Cols; j++)
                    {
                        MatrixInternal[(i * Cols + j) * PaddingBetweenNumbers + PaddingFromBegin] = (T)input[i, j].Copy();
                    }
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <param name="initialize"></param>
        private Matrix(int rows, int cols, bool initialize)
        {
            Rows = rows;
            Cols = cols;
            MatrixInternal = new T[Rows * Cols * PaddingBetweenNumbers + PaddingFromBegin];

            if (initialize == true)
            {
                Parallel.ForEach(GetRowsChunks(), (pair) =>
                {
                    for (int i = pair.Item1; i < pair.Item2; ++i)
                    {
                        for (int j = 0; j < cols; j++)
                        {
                            MatrixInternal[(i * Cols + j) * PaddingBetweenNumbers + PaddingFromBegin] = new T();
                        }
                    }
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        public Matrix(int rows, int cols) : this(rows, cols, true) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        public Matrix(Matrix<T> matrix)
        {
            if (matrix == null) { throw new MatrixLibraryException("Given matrix reference was null!"); }

            Rows = matrix.Rows;
            Cols = matrix.Cols;
            MatrixInternal = new T[Rows * Cols * PaddingBetweenNumbers + PaddingFromBegin];

            Parallel.ForEach(GetRowsChunks(), (pair) =>
            {
                for (int i = pair.Item1; i < pair.Item2; i++)
                {
                    for (int j = 0; j < Cols; j++)
                    {
                        MatrixInternal[(i * Cols + j) * PaddingBetweenNumbers + PaddingFromBegin] = (T)matrix.GetNumber(i, j).Copy();
                    }
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="col"></param>
        /// <param name="column"></param>
        public Matrix(Matrix<T> matrix, int col, Matrix<T> column)
        {
            if (matrix == null) { throw new MatrixLibraryException("Given matrix reference was null!"); }
            if (matrix.Rows != column.Rows) { throw new MatrixLibraryException("Given column has different number of rows than matrix!"); }
            if (col < 0 || col >= matrix.Cols) { throw new MatrixLibraryException("Number of column to replace is invalid!"); }

            Rows = matrix.Rows;
            Cols = matrix.Cols;
            MatrixInternal = new T[Rows * Cols * PaddingBetweenNumbers + PaddingFromBegin];

            Parallel.ForEach(GetRowsChunks(), (pair) =>
            {
                for (int i = pair.Item1; i < pair.Item2; i++)
                {
                    for (int j = 0; j < Cols; j++)
                    {
                        if (j != col) { MatrixInternal[(i * Cols + j) * PaddingBetweenNumbers + PaddingFromBegin] = (T)matrix.GetNumber(i, j).Copy(); }
                        else { MatrixInternal[(i * Cols + j) * PaddingBetweenNumbers + PaddingFromBegin] = (T)column.GetNumber(i, 0).Copy(); }
                    }
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public T this[int i, int j]
        {
            get { return MatrixInternal[(i * Cols + j) * PaddingBetweenNumbers + PaddingFromBegin]; }
            set { MatrixInternal[(i * Cols + j) * PaddingBetweenNumbers + PaddingFromBegin] = (T)value.Copy(); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public T GetNumber(int i, int j)
        {
#if !UNCHECKED_BOUNDARIES
            if (i >= Rows || j >= Cols) { throw new MatrixLibraryException("Bad indices specified!"); }
#endif

            return MatrixInternal[(i * Cols + j) * PaddingBetweenNumbers + PaddingFromBegin];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public T[] GetRow(int row)
        {
#if !UNCHECKED_BOUNDARIES
            if (row >= Rows) { throw new MatrixLibraryException("Bad index of row specified!"); }
#endif

            T[] result = new T[Cols];
            for (int j = 0; j < Cols; j++)
            {
                result[j] = MatrixInternal[(row * Cols + j) * PaddingBetweenNumbers + PaddingFromBegin];
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="col"></param>
        /// <returns></returns>
        public T[] GetCol(int col)
        {
#if !UNCHECKED_BOUNDARIES
            if (col >= Cols) { throw new MatrixLibraryException("Bad index of column specified!"); }
#endif

            T[] result = new T[Rows];
            for (int i = 0; i < Rows; i++)
            {
                result[i] = MatrixInternal[(i * Cols + col) * PaddingBetweenNumbers + PaddingFromBegin];
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="number"></param>
        public void WriteNumber(int i, int j, T number)
        {
#if !UNCHECKED_BOUNDARIES
            if (i >= Rows || j >= Cols) { throw new MatrixLibraryException("Bad indices specified!"); }
#endif

            MatrixInternal[(i * Cols + j) * PaddingBetweenNumbers + PaddingFromBegin] = (T)number.Copy();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="row"></param>
        public void WriteRow(int i, T[] row)
        {
#if !UNCHECKED_BOUNDARIES
            if (row.Length != Cols) { throw new MatrixLibraryException("Row does not have the same number of cols as matrix!"); }
            if (i >= Rows) { throw new MatrixLibraryException("Bad index of row specified!"); }
#endif

            for (int j = 0; j < row.Length; ++j)
            {
                MatrixInternal[(i * Cols + j) * PaddingBetweenNumbers + PaddingFromBegin] = (T)row[j].Copy();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="j"></param>
        /// <param name="col"></param>
        public void WriteCol(int j, T[] col)
        {
#if !UNCHECKED_BOUNDARIES
            if (col.Length != Rows) { throw new MatrixLibraryException("Col does not have the same number of rows as matrix!"); }
            if (j >= Cols) { throw new MatrixLibraryException("Bad index of column specified!"); }
#endif

            for (int i = 0; i < col.Length; ++i)
            {
                MatrixInternal[(i * Cols + j) * PaddingBetweenNumbers + PaddingFromBegin] = (T)col[i].Copy();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="firstRow"></param>
        /// <param name="firstCol"></param>
        /// <param name="secondRow"></param>
        /// <param name="secondCol"></param>
        public void SwapElements(int firstRow, int firstCol, int secondRow, int secondCol)
        {
#if !UNCHECKED_BOUNDARIES
            if (firstRow >= Rows || secondRow >= Rows || firstCol >= Cols || secondCol >= Cols) { throw new MatrixLibraryException("Bad indices specified!"); }
#endif

            T temp = MatrixInternal[(firstRow * Cols + firstCol) * PaddingBetweenNumbers + PaddingFromBegin];
            MatrixInternal[(firstRow * Cols + firstCol) * PaddingBetweenNumbers + PaddingFromBegin] = 
                MatrixInternal[(secondRow * Cols + secondCol) * PaddingBetweenNumbers + PaddingFromBegin];
            MatrixInternal[(secondRow * Cols + secondCol) * PaddingBetweenNumbers + PaddingFromBegin] = temp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetHalfOfRows()
        {
            if ((Rows % 2) == 0) { return Rows / 2; }
            else { return (Rows / 2) + 1; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetHalfOfCols()
        {
            if ((Cols % 2) == 0) { return Cols / 2; }
            else { return (Cols / 2) + 1; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tuple<int, int>> GetRowsChunks()
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

                    //Console.WriteLine("{0} {1}", i, end);
                    yield return new Tuple<int, int>(i, end);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPos"></param>
        /// <returns></returns>
        public IEnumerable<Tuple<int, int>> GetRowsChunks(int startPos)
        {
            if (startPos >= Rows) { yield return new Tuple<int, int>(Rows, Rows); }

            int chunkCount = 2 * Environment.ProcessorCount;
            int chunkLength = (Rows - startPos) / chunkCount;

            if (chunkCount >= (Rows - startPos)) { yield return new Tuple<int, int>(startPos, Rows); }
            else
            {
                for (int i = startPos; i < Rows; i += chunkLength)
                {
                    int end = i + chunkLength;
                    if (end > Rows) { end = Rows; }

                    yield return new Tuple<int, int>(i, end);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public IEnumerable<Tuple<int, int>> GetRowsChunks(int startPos, int length)
        {
            if ((startPos + length) > Rows || startPos >= Rows) { yield return new Tuple<int, int>(Rows, Rows); }

            int chunkCount = 2 * Environment.ProcessorCount;
            int chunkLength = length / chunkCount;

            if (chunkCount >= length) { yield return new Tuple<int, int>(startPos, startPos + length); }
            else
            {
                for (int i = startPos; i < (startPos + length); i += chunkLength)
                {
                    int end = i + chunkLength;
                    if (end > (startPos + length)) { end = startPos + length; }

                    yield return new Tuple<int, int>(i, end);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tuple<int, int>> GetHalfRowsChunks()
        {
            int halfOfRows;
            if ((Rows % 2) == 0) { halfOfRows = Rows / 2; }
            else { halfOfRows = (Rows / 2) + 1; }

            int chunkCount = 2 * Environment.ProcessorCount;
            int chunkLength = halfOfRows / chunkCount;

            if (chunkCount >= halfOfRows) { yield return new Tuple<int, int>(0, halfOfRows); }
            else
            {
                for (int i = 0; i < halfOfRows; i += chunkLength)
                {
                    int end = i + chunkLength;
                    if (end > halfOfRows) { end = halfOfRows; }

                    yield return new Tuple<int, int>(i, end);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tuple<int, int>> GetColsChunks()
        {
            int chunkCount = 2 * Environment.ProcessorCount;
            int chunkLength = Cols / chunkCount;

            if (chunkCount >= Cols) { yield return new Tuple<int, int>(0, Cols); }
            else
            {
                for (int i = 0; i < Cols; i += chunkLength)
                {
                    int end = i + chunkLength;
                    if (end > Cols) { end = Cols; }

                    yield return new Tuple<int, int>(i, end);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPos"></param>
        /// <returns></returns>
        public IEnumerable<Tuple<int, int>> GetColsChunks(int startPos)
        {
            if (startPos >= Cols) { yield return new Tuple<int, int>(Cols, Cols); }

            int chunkCount = 2 * Environment.ProcessorCount;
            int chunkLength = (Cols - startPos) / chunkCount;

            if (chunkCount >= (Cols - startPos)) { yield return new Tuple<int, int>(startPos, Cols); }
            else
            {
                for (int i = startPos; i < Cols; i += chunkLength)
                {
                    int end = i + chunkLength;
                    if (end > Cols) { end = Cols; }

                    yield return new Tuple<int, int>(i, end);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public IEnumerable<Tuple<int, int>> GetColsChunks(int startPos, int length)
        {
            if ((startPos + length) > Cols || startPos >= Cols) { yield return new Tuple<int, int>(Cols, Cols); }

            int chunkCount = 2 * Environment.ProcessorCount;
            int chunkLength = length / chunkCount;

            if (chunkCount >= length) { yield return new Tuple<int, int>(startPos, startPos + length); }
            else
            {
                for (int i = startPos; i < (startPos + length); i += chunkLength)
                {
                    int end = i + chunkLength;
                    if (end > (startPos + length)) { end = startPos + length; }

                    yield return new Tuple<int, int>(i, end);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tuple<int, int>> GetHalfColsChunks()
        {
            int halfOfCols;
            if ((Cols % 2) == 0) { halfOfCols = Cols / 2; }
            else { halfOfCols = (Cols / 2) + 1; }

            int chunkCount = 2 * Environment.ProcessorCount;
            int chunkLength = halfOfCols / chunkCount;

            if (chunkCount >= halfOfCols) { yield return new Tuple<int, int>(0, halfOfCols); }
            else
            {
                for (int i = 0; i < halfOfCols; i += chunkLength)
                {
                    int end = i + chunkLength;
                    if (end > halfOfCols) { end = halfOfCols; }

                    yield return new Tuple<int, int>(i, end);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <returns></returns>
        public static Matrix<T> GetUninitializedMatrix(int rows, int cols)
        {
            return new Matrix<T>(rows, cols, false);
        }

        private static int PaddingFromBegin = 0; // Taken from this: http://blog.mischel.com/2011/12/29/more-about-cache-contention/
        private static int PaddingBetweenNumbers = 1;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dimension"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="cols"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < Rows; ++i)
            {
                for (int j = 0; j < Cols; ++j)
                {
                    result.Append(string.Format("{0,4} ", MatrixInternal[(i * Cols + j) * PaddingBetweenNumbers + PaddingFromBegin].ToDouble()));
                }
                result.AppendLine();
            }

            return result.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return (this == (Matrix<T>)obj);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Matrix<T> operator +(Matrix<T> first, Matrix<T> second)
        {
            return ClassicOperations.Addition(first, second);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Matrix<T> operator -(Matrix<T> first, Matrix<T> second)
        {
            return ClassicOperations.Subtraction(first, second);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Matrix<T> operator *(Matrix<T> first, Matrix<T> second)
        {
            return ClassicOperations.Multiplication(first, second);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool operator ==(Matrix<T> first, Matrix<T> second)
        {
            if (ReferenceEquals(first, second)) { return true; }
            if (ReferenceEquals(first, null) || ReferenceEquals(second, null)) { return false; }

            bool result = true;
            if (first.Rows == second.Rows && first.Cols == second.Cols)
            {
                for (int i = 0; i < first.Rows; i++)
                {
                    for (int j = 0; j < first.Cols; j++)
                    {
                        if (!first.GetNumber(i, j).__IsEqual(second.GetNumber(i, j)))
                        {
                            Console.WriteLine("!!! Different Numbers: {0}; {1}", first.GetNumber(i, j), second.GetNumber(i, j));
                            result = false;
                        }
                    }
                }
            }
            else { result = false; }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool operator !=(Matrix<T> first, Matrix<T> second)
        {
            if (ReferenceEquals(first, second)) { return false; }
            if (ReferenceEquals(first, null) || ReferenceEquals(second, null)) { return false; }

            bool result = false;
            if (first.Rows == second.Rows && first.Cols == second.Cols)
            {
                for (int i = 0; i < first.Rows; i++)
                {
                    for (int j = 0; j < first.Cols; j++)
                    {
                        if (first.GetNumber(i, j).__IsEqual(second.GetNumber(i, j))) { result = true; break; }
                    }
                    if (result == true) { break; }
                }
            }
            else { result = true; }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        public static T[] DoVectorOperation(T[] first, T[] second, Func<IMatrixNumber, IMatrixNumber, IMatrixNumber> operation)
        {
            if (first.Length != second.Length) { throw new MatrixLibraryException("Vectors do not have the same length!"); }

            T[] result = new T[first.Length];

            for (int i = 0; i < first.Length; ++i)
            {
                result[i] = (T)operation((IMatrixNumber)first[i], (IMatrixNumber)second[i]);
            }

            return result;
        }
    }
}