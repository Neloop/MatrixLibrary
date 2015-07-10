using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLibrary
{
    /// <summary>
    /// Enumeration which is used to better representation for result of matrix definity.
    /// </summary>
    public enum DefinityClassification
    {
        /// <summary>
        /// Matrix is indefinite.
        /// </summary>
        Indefinite,
        /// <summary>
        /// Matrix is positive-definite.
        /// </summary>
        PositiveDefinite,
        /// <summary>
        /// Matrix is negative-definite.
        /// </summary>
        NegativeDefinite
    };

    /// <summary>
    /// Namespace of parallel extension methods which represents properties which matrix might have had.
    /// </summary>
    public static class ParallelPropertiesExtensions
    {
        /// <summary>
        /// Tells if given <paramref name="matrix"/> is invertible/regular.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Matrix which will be inspected if its invertible.</param>
        /// <returns>True if <paramref name="matrix"/> is invertible, false otherwise.</returns>
        public static bool IsInvertibleParallel<T>(this Matrix<T> matrix) where T : IMatrixNumber, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            bool result = true;
            int cols = matrix.Cols;

            if (matrix.Rows == cols)
            {
                Matrix<T> temporaryM = ParallelAlteringOperationsExtensions.GaussParallel(matrix);
                Parallel.ForEach(temporaryM.GetRowsChunks(), (pair, loopState) =>
                {
                    for (int i = pair.Item1; i < pair.Item2; i++)
                    {
                        int zeroes = 0;

                        for (int j = 0; j < cols; j++)
                        {
                            if (!temporaryM.GetNumber(i, j).IsZero()) { break; }
                            else { zeroes++; }
                        }
                        if (zeroes == cols) { result = false; loopState.Stop(); }
                    }
                });
            }
            else
            {
                return false;
            }

            return result;
        }

        /// <summary>
        /// Determines what rank given <paramref name="matrix"/> object has.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Source Matrix object which rank is wanted.</param>
        /// <returns>Integral value representing number of independent vectors in <paramref name="matrix"/>.</returns>
        public static int RankParallel<T>(this Matrix<T> matrix) where T : IMatrixNumber, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            object resultLock = new object();
            int result = matrix.Rows;

            Matrix<T> gauss = ParallelAlteringOperationsExtensions.GaussParallel(matrix);
            int cols = gauss.Cols;

            Parallel.ForEach(gauss.GetRowsChunks(), (pair) =>
            {
                for (int i = pair.Item1; i < pair.Item2; i++)
                {
                    int zeroes = 0;
                    for (int j = 0; j < cols; j++)
                    {
                        if (gauss.GetNumber(i, j).IsZero()) { zeroes++; }
                    }
                    if (zeroes == gauss.Cols) { lock (resultLock) { result--; } }
                }
            });

            return result;
        }

        /// <summary>
        /// Determines whether object <paramref name="matrix"/> is orthogonal or not.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Matrix class which will be analyzed for orthogonality.</param>
        /// <returns>True if given <paramref name="matrix"/> is orthogonal, false otherwise.</returns>
        public static bool IsOrthogonalParallel<T>(this Matrix<T> matrix) where T : IMatrixNumber, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            bool result = true;

            if (matrix.Rows == matrix.Cols)
            {
                Matrix<T> transposition = ParallelAlteringOperationsExtensions.TranspositionParallel(matrix);
                Matrix<T> multiplied = ParallelClassicOperations.Multiplication(transposition, matrix);
                int cols_mult = multiplied.Cols;

                Parallel.ForEach(multiplied.GetRowsChunks(), (pair, loopState) =>
                {
                    for (int i = pair.Item1; i < pair.Item2; i++)
                    {
                        for (int j = 0; j < cols_mult; j++)
                        {
                            if (i == j)
                            {
                                if (!multiplied.GetNumber(i, j).IsOne()) { result = false; loopState.Stop(); }
                            }
                            else
                            {
                                if (!multiplied.GetNumber(i, j).IsZero()) { result = false; loopState.Stop(); }
                            }
                        }
                    }
                });
            }
            else { return false; }

            return result;
        }

        /// <summary>
        /// Detect if given <paramref name="matrix"/> is positive-definite, negative-definite or indefinite.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Matrix object which is source for computation of this method.</param>
        /// <returns>Enumeration which values reprezents state of definity of <paramref name="matrix"/>.</returns>
        public static DefinityClassification DefinityParallel<T>(this Matrix<T> matrix) where T : IMatrixNumber, new()
        {
            /*
             * Neurčuje semi-definitnost (pozitivní/negativní)
             * Využívá se Sylvestrovo kriterium
             * Pokud je vrácena 
             * 0: Indefinitní
             * 1: Pozitivně definitní
             * 2: Negatině definitní
             * 
             * */

            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            DefinityClassification result = DefinityClassification.Indefinite;

            if (matrix.Rows == matrix.Cols)
            {
                T[] determinant = new T[matrix.Rows];

                Parallel.ForEach(matrix.GetRowsChunks(), (pair) =>
                {
                    for (int i = pair.Item1; i < pair.Item2; i++)
                    {
                        Matrix<T> det = Matrix<T>.GetUninitializedMatrix(i + 1, i + 1);
                        for (int k = 0; k < (i + 1); k++)
                        {
                            for (int l = 0; l < (i + 1); l++)
                            {
                                det.WriteNumber(k, l, matrix.GetNumber(k, l));
                            }
                        }
                        determinant[i] = ComputationsExtensions.Determinant(det);
                    }
                });

                object positiveLock = new object();
                int positive = 0;
                bool negative = true;
                T zero = new T();
                Parallel.ForEach(matrix.GetRowsChunks(), (pair) =>
                {
                    for (int i = pair.Item1; i < pair.Item2; i++)
                    {
                        if (determinant[i].__IsGreaterThan(zero)) { lock (positiveLock) { positive++; } }
                        if ((i % 2) == 0 && (determinant[i].__IsGreaterThan(zero) || determinant[i].__IsEqual(zero))) { negative = false; }
                        if ((i % 2) == 1 && (determinant[i].__IsLessThan(zero) || determinant[i].__IsEqual(zero))) { negative = false; }
                    }
                });

                if (positive == matrix.Rows) { result = DefinityClassification.PositiveDefinite; }
                else if (negative == true) { result = DefinityClassification.NegativeDefinite; }
            }

            return result;
        }
    }

    /// <summary>
    /// Namespace of extension methods which represents properties which matrix might have had.
    /// </summary>
    public static class PropertiesExtensions
    {
        /// <summary>
        /// Tells if given <paramref name="matrix"/> is invertible/regular.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Matrix which will be inspected if its invertible.</param>
        /// <returns>True if <paramref name="matrix"/> is invertible, false otherwise.</returns>
        public static bool IsInvertible<T>(this Matrix<T> matrix) where T : IMatrixNumber, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            if (matrix.Rows == matrix.Cols)
            {
                int rows = matrix.Rows;
                int cols = matrix.Cols;

                Matrix<T> temporaryM = AlteringOperationsExtensions.Gauss(matrix);
                for (int i = 0; i < rows; i++)
                {
                    int zeroes = 0;

                    for (int j = 0; j < cols; j++)
                    {
                        if (!temporaryM.GetNumber(i, j).IsZero()) { break; }
                        else { zeroes++; }
                    }
                    if (zeroes == cols) { return false; }
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines what rank given <paramref name="matrix"/> object has.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Source Matrix object which rank is wanted.</param>
        /// <returns>Integral value representing number of independent vectors in <paramref name="matrix"/>.</returns>
        public static int Rank<T>(this Matrix<T> matrix) where T : IMatrixNumber, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            int result = matrix.Rows;

            Matrix<T> gauss = AlteringOperationsExtensions.Gauss(matrix);
            int rows = gauss.Rows;
            int cols = gauss.Cols;

            for (int i = 0; i < rows; i++)
            {
                int zeroes = 0;
                for (int j = 0; j < cols; j++)
                {
                    if (gauss.GetNumber(i, j).IsZero()) { zeroes++; }
                }
                if (zeroes == gauss.Cols) { result--; }
            }

            return result;
        }

        /// <summary>
        /// Determines whether object <paramref name="matrix"/> is orthogonal or not.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Matrix class which will be analyzed for orthogonality.</param>
        /// <returns>True if given <paramref name="matrix"/> is orthogonal, false otherwise.</returns>
        public static bool IsOrthogonal<T>(this Matrix<T> matrix) where T : IMatrixNumber, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            if (matrix.Rows == matrix.Cols)
            {
                Matrix<T> transposition = AlteringOperationsExtensions.Transposition(matrix);
                Matrix<T> multiplied = ClassicOperations.Multiplication(transposition, matrix);
                int rows_mult = multiplied.Rows;
                int cols_mult = multiplied.Cols;

                for (int i = 0; i < rows_mult; i++)
                {
                    for (int j = 0; j < cols_mult; j++)
                    {
                        if (i == j)
                        {
                            if (!multiplied.GetNumber(i, j).IsOne()) { return false; }
                        }
                        else
                        {
                            if (!multiplied.GetNumber(i, j).IsZero()) { return false; }
                        }
                    }
                }
            }
            else { return false; }

            return true;
        }

        /// <summary>
        /// Detect if given <paramref name="matrix"/> is positive-definite, negative-definite or indefinite.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Matrix object which is source for computation of this method.</param>
        /// <returns>Enumeration which values reprezents state of definity of <paramref name="matrix"/>.</returns>
        public static DefinityClassification Definity<T>(this Matrix<T> matrix) where T : IMatrixNumber, new()
        {
            /*
             * Neurčuje semi-definitnost (pozitivní/negativní)
             * Využívá se Sylvestrovo kriterium
             * Pokud je vrácena 
             * 0: Indefinitní
             * 1: Pozitivně definitní
             * 2: Negatině definitní
             * 
             * */

            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            DefinityClassification result = DefinityClassification.Indefinite;

            if (matrix.Rows == matrix.Cols)
            {
                int rows = matrix.Rows;
                T[] determinant = new T[rows];

                for (int i = 0; i < rows; i++)
                {
                    Matrix<T> det = Matrix<T>.GetUninitializedMatrix(i + 1, i + 1);
                    for (int k = 0; k < (i + 1); k++)
                    {
                        for (int l = 0; l < (i + 1); l++)
                        {
                            det.WriteNumber(k, l, matrix.GetNumber(k, l));
                        }
                    }
                    determinant[i] = ComputationsExtensions.Determinant(det);
                }

                int positive = 0;
                bool negative = true;
                T zero = new T();
                for (int i = 0; i < rows; i++)
                {
                    if (determinant[i].__IsGreaterThan(zero)) { positive++; }
                    if ((i % 2) == 0 && (determinant[i].__IsGreaterThan(zero) || determinant[i].__IsEqual(zero))) { negative = false; }
                    if ((i % 2) == 1 && (determinant[i].__IsLessThan(zero) || determinant[i].__IsEqual(zero))) { negative = false; }
                }

                if (positive == rows) { result = DefinityClassification.PositiveDefinite; }
                else if (negative == true) { result = DefinityClassification.NegativeDefinite; }
            }

            return result;
        }
    }
}
