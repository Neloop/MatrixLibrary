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
    /// Represents a classical mathematic structure a matrix. Stores individual elements and information about matrix such as count of rows and cols.
    /// </summary>
    /// <remarks>
    /// Datas are not stored in two-dimensional array, but in single array, index in this field is computed through indices of row and col.
    /// Matrix is not dynamic, rows and cols cannot be deleted or added after construction.
    /// This class and all its methods are not thread safe.
    /// </remarks>
    /// <typeparam name="T">Type of numbers which will be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
    public class Matrix<T> where T : IMatrixNumber, new()
    {
        private T[] MatrixInternal;
        /// <summary>
        /// Property which stores count of rows in this matrix.
        /// </summary>
        public int Rows { get; private set; }
        /// <summary>
        /// Tells how many columns are in this matrix.
        /// </summary>
        public int Cols { get; private set; }
        
        /// <summary>
        /// Initializes new instance of Matrix, which will have same data as in <paramref name="input"/> and same number of rows and cols.
        /// </summary>
        /// <param name="input">Source of the data for Matrix.</param>
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
        /// Initializes new instance of Matrix class with same internal data as in <paramref name="input"/>.
        /// </summary>
        /// <param name="input">Source for the new instance of Matrix.</param>
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
        /// Initializes new instance of Matrix class which source data is <paramref name="input"/>.
        /// </summary>
        /// <param name="input">Source data which will be copied to the new Matrix object.</param>
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

        private Matrix(int rows, int cols, bool initialize)
        {
            if (rows < 0) { throw new MatrixLibraryException("Count of rows is negative or zero!"); }
            if (cols < 0) { throw new MatrixLibraryException("Count of columns is negative or zero!"); }

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
        /// Initializes new instance of Matrix class which will have specified number of <paramref name="rows"/> and <paramref name="cols"/>.
        /// </summary>
        /// <remarks>
        /// All elements of Matrix will be initialized with their default parameterless constructor, which should be equal to integral zero.
        /// </remarks>
        /// <param name="rows">Number of rows in new Matrix object.</param>
        /// <param name="cols">Number of cols in new Matrix object.</param>
        public Matrix(int rows, int cols) : this(rows, cols, true) { }

        /// <summary>
        /// Classic copy constructor which initializes new Matrix object with the same internal data as <paramref name="matrix"/>.
        /// </summary>
        /// <param name="matrix">Source of data for new instance of Matrix class.</param>
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
        /// Initializes new instance of Matrix class which will have data from <paramref name="matrix"/>, but column with index <paramref name="col"/> will have data from <paramref name="column"/>.
        /// </summary>
        /// <param name="matrix">Source of almost all data for new object.</param>
        /// <param name="col">Index of column which will be replaced with <paramref name="column"/> in new Matrix.</param>
        /// <param name="column">Column which will be on specified <paramref name="col"/> in new Matrix.</param>
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
        /// Indexer which will get or set internal number on specified indices <paramref name="i"/> and <paramref name="j"/>.
        /// </summary>
        /// <param name="i">Specify row of internal element.</param>
        /// <param name="j">Specify column of internal element.</param>
        /// <returns>Getter returns reference to the internal number.</returns>
        public T this[int i, int j]
        {
            get { return MatrixInternal[(i * Cols + j) * PaddingBetweenNumbers + PaddingFromBegin]; }
            set { MatrixInternal[(i * Cols + j) * PaddingBetweenNumbers + PaddingFromBegin] = (T)value.Copy(); }
        }

        /// <summary>
        /// Locate and returns internal element on specified indices <paramref name="i"/> and <paramref name="j"/>.
        /// </summary>
        /// <param name="i">Specify row of internal element.</param>
        /// <param name="j">Specify column of internal element.</param>
        /// <returns>Reference to internal number located in array.</returns>
        public T GetNumber(int i, int j)
        {
#if !UNCHECKED_BOUNDARIES
            if (i >= Rows || j >= Cols) { throw new MatrixLibraryException("Bad indices specified!"); }
#endif

            return MatrixInternal[(i * Cols + j) * PaddingBetweenNumbers + PaddingFromBegin];
        }
        /// <summary>
        /// Gets array which represents row on specified index <paramref name="row"/> in this Matrix. Array is newly created and contains references to elements.
        /// </summary>
        /// <param name="row">Specify row which will be returned.</param>
        /// <returns>Reference to single array which represents row in this Matrix.</returns>
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
        /// Gets an array which represents column on specified index <paramref name="col"/>. Array is newly created and contains references to elements.
        /// </summary>
        /// <param name="col">Specify column which will be returned.</param>
        /// <returns>Reference to newly created array representating column.</returns>
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
        /// To specified indices <paramref name="i"/> and <paramref name="j"/> in this Matrix writes new element <paramref name="number"/>.
        /// </summary>
        /// <remarks>
        /// New element will be copied and stored in this Matrix.
        /// </remarks>
        /// <param name="i">Specify row on which new element will be written.</param>
        /// <param name="j">Specify column on which will be new element written.</param>
        /// <param name="number">New element which will be stored on specified indices.</param>
        public void WriteNumber(int i, int j, T number)
        {
#if !UNCHECKED_BOUNDARIES
            if (i >= Rows || j >= Cols) { throw new MatrixLibraryException("Bad indices specified!"); }
#endif

            MatrixInternal[(i * Cols + j) * PaddingBetweenNumbers + PaddingFromBegin] = (T)number.Copy();
        }
        /// <summary>
        /// Writes given array <paramref name="row"/> on specified index of row <paramref name="i"/>.
        /// </summary>
        /// <remarks>
        /// Elements from <paramref name="row"/> are copied into this Matrix.
        /// </remarks>
        /// <param name="i">Index of row in this Matrix on which will be written array <paramref name="row"/>.</param>
        /// <param name="row">Single array of elements which will be stored on specified index.</param>
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
        /// Writes given array <paramref name="col"/> on specified index of column <paramref name="j"/>.
        /// </summary>
        /// <remarks>
        /// Elements from <paramref name="col"/> are copied into this Matrix.
        /// </remarks>
        /// <param name="j">Index of column in this Matrix on which will be written array <paramref name="col"/>.</param>
        /// <param name="col">Single array of elements which will be stored on specified index of column.</param>
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
        /// Swap two elements from this Matrix between each other.
        /// </summary>
        /// <param name="firstRow">Index of row of first element.</param>
        /// <param name="firstCol">Index of column of first element.</param>
        /// <param name="secondRow">Index of row of second element.</param>
        /// <param name="secondCol">Index of column of second element.</param>
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
        /// Computes half of number of rows and returns it.
        /// </summary>
        /// <returns>Always positive number.</returns>
        public int GetHalfOfRows()
        {
            if ((Rows % 2) == 0) { return Rows / 2; }
            else { return (Rows / 2) + 1; }
        }
        /// <summary>
        /// Computes half of number of columns and returns it.
        /// </summary>
        /// <returns>Always positive number.</returns>
        public int GetHalfOfCols()
        {
            if ((Cols % 2) == 0) { return Cols / 2; }
            else { return (Cols / 2) + 1; }
        }

        /// <summary>
        /// Partitioning function which split number of rows to 2*ProcessorCount chunks.
        /// </summary>
        /// <returns>Array of couples which represents start index and end index.</returns>
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
        /// Partitioning function which split number of rows starting on <paramref name="startPos"/> to 2*ProcessorCount chunks.
        /// </summary>
        /// <param name="startPos">Start position of rows to split.</param>
        /// <returns>Array of couples which represents start index and end index.</returns>
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
        /// Partitioning function which split rows starting on <paramref name="startPos"/> in lenght of <paramref name="length"/> to 2*ProcessorCount chunks.
        /// </summary>
        /// <param name="startPos">Start position of rows to split.</param>
        /// <param name="length">Count of elements to split.</param>
        /// <returns>Array of couples which represents start index and end index.</returns>
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
        /// Partitioning function which split half of number of rows to 2*ProcessorCount chunks.
        /// </summary>
        /// <returns>Array of couples which represents start index and end index.</returns>
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
        /// Partitioning function which split number of columns to 2*ProcessorCount chunks.
        /// </summary>
        /// <returns>Array of couples which represents start index and end index.</returns>
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
        /// Partitioning function which split number of columns starting on <paramref name="startPos"/> to 2*ProcessorCount chunks.
        /// </summary>
        /// <param name="startPos">Start position of rows to split.</param>
        /// <returns>Array of couples which represents start index and end index.</returns>
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
        /// Partitioning function which split number of columns starting on <paramref name="startPos"/> in length of <paramref name="length"/> to 2*ProcessorCount chunks.
        /// </summary>
        /// <param name="startPos">Start position of rows to split.</param>
        /// <param name="length"></param>
        /// <returns>Array of couples which represents start index and end index.</returns>
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
        /// Partitioning function which split half of number of columns to 2*ProcessorCount chunks.
        /// </summary>
        /// <returns>Array of couples which represents start index and end index.</returns>
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
        /// Creates new instance of Matrix which elements will be unitialized (parametersless constructor is not called).
        /// </summary>
        /// <param name="rows">Number of rows in new Matrix object.</param>
        /// <param name="cols">Number of columns in new Matrix object.</param>
        /// <returns>Returns newly created instance of unitialized Matrix.</returns>
        public static Matrix<T> GetUninitializedMatrix(int rows, int cols)
        {
            return new Matrix<T>(rows, cols, false);
        }

        /// <summary>
        /// Creates new instance of Matrix with same number of rows and cols specified by <paramref name="dimension"/>. All elements will be zeroes, except for main diagonal, there will be ones.
        /// </summary>
        /// <param name="dimension">Number of rows and cols in new Matrix.</param>
        /// <returns>Returns newly created instance of diagonal Matrix.</returns>
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
        /// Creates new instance of Matrix with specified number of <paramref name="rows"/> and <paramref name="cols"/>. All elements will be zeroes.
        /// </summary>
        /// <param name="rows">Number of rows in new Matrix.</param>
        /// <param name="cols">Number of columns in new Matrix.</param>
        /// <returns>Created Matrix with all zero elements.</returns>
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

        private static int PaddingFromBegin = 0; // Taken from this: http://blog.mischel.com/2011/12/29/more-about-cache-contention/
        private static int PaddingBetweenNumbers = 1;

        /// <summary>
        /// Converts internal datas of Matrix to textual representation.
        /// </summary>
        /// <returns>String which represents this class.</returns>
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
        /// Compare this object and <paramref name="obj"/> on equality.
        /// </summary>
        /// <param name="obj">Second compared object.</param>
        /// <returns>True if current object and <paramref name="obj"/> are equals, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            return (this == (Matrix<T>)obj);
        }
        /// <summary>
        /// Overriden inherited function from object. 
        /// </summary>
        /// <returns>Hashed integer function.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Sums <paramref name="first"/> and <paramref name="second"/> Matrix objects and returns result.
        /// </summary>
        /// <remarks>
        /// Internally ClassicOperations.Addition() function is called.
        /// </remarks>
        /// <param name="first">First Matrix summand.</param>
        /// <param name="second">Second Matrix summand.</param>
        /// <returns>Result of addition of <paramref name="first"/> and <paramref name="second"/>.</returns>
        public static Matrix<T> operator +(Matrix<T> first, Matrix<T> second)
        {
            return ClassicOperations.Addition(first, second);
        }
        /// <summary>
        /// Subtract <paramref name="second"/> from <paramref name="first"/> and returns result.
        /// </summary>
        /// <remarks>
        /// Internally ClassicOperations.Subtraction() function is called.
        /// </remarks>
        /// <param name="first">Minuend.</param>
        /// <param name="second">Subtrahend.</param>
        /// <returns>Result of subtraction of <paramref name="first"/> and <paramref name="second"/>.</returns>
        public static Matrix<T> operator -(Matrix<T> first, Matrix<T> second)
        {
            return ClassicOperations.Subtraction(first, second);
        }
        /// <summary>
        /// Multiplicate <paramref name="first"/> and <paramref name="second"/> Matrix object and returns result.
        /// </summary>
        /// <remarks>
        /// Internally ClassicOperations.Multiplication() function is called.
        /// </remarks>
        /// <param name="first">Multiplicant.</param>
        /// <param name="second">Multiplier.</param>
        /// <returns>Result of multiplication of <paramref name="first"/> and <paramref name="second"/>.</returns>
        public static Matrix<T> operator *(Matrix<T> first, Matrix<T> second)
        {
            return ClassicOperations.Multiplication(first, second);
        }
        /// <summary>
        /// Determines whether elements of <paramref name="first"/> Matrix and <paramref name="second"/> Matrix have same values or not.
        /// </summary>
        /// <param name="first">First compared object.</param>
        /// <param name="second">Second compared object.</param>
        /// <returns>True if <paramref name="first"/> and <paramref name="second"/> have same elements, false otherwise.</returns>
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
        /// Determines whether elements of <paramref name="first"/> Matrix and <paramref name="second"/> Matrix do not have same values.
        /// </summary>
        /// <param name="first">First compared element.</param>
        /// <param name="second">Second compared element.</param>
        /// <returns>True if <paramref name="first"/> and <paramref name="second"/> do not have same elements, false otherwise.</returns>
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
        /// On elements in <paramref name="first"/> vector and <paramref name="second"/> vector will be executed <paramref name="operation"/>.
        /// </summary>
        /// <remarks>
        /// First parameter <paramref name="first"/> and <paramref name="second"/> parameter must have same number of elements.
        /// </remarks>
        /// <param name="first">First parameter to defined <paramref name="operation"/>.</param>
        /// <param name="second">Second parameter to defined <paramref name="operation"/>.</param>
        /// <param name="operation">Operation which will be executed on every couple of parallel elements from <paramref name="first"/> and <paramref name="second"/> array.</param>
        /// <returns>Single array which is result of given <paramref name="operation"/>. This array will have same number of elements as <paramref name="first"/> and <paramref name="second"/> array.</returns>
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