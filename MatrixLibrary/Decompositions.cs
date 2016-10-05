using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLibrary
{
    /// <summary>
    /// Namespace of parallel extensions methods containing functions relating to matrix decompositions.
    /// </summary>
    public static class ParallelDecompositionsExtensions
    {
        /// <summary>
        /// Cholesky decomposition is performed in this function and its result, matrix L, will be returned to caller.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Matrix which will be decomposed, does not have to be symmetric.</param>
        /// <returns>Bottom triangular Matrix L, where <paramref name="matrix"/> = L * L^(T)</returns>
        public static Matrix<T> CholeskyDecompositionParallel<T>(this Matrix<T> matrix) where T : IMatrixNumber, new()
        {
            /*
             * 
             * Cholesky decomposition: A = L * L^(T)
             *  - matrix A has to be positively-definite and of type n*n
             * 
             */

            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result;
            Matrix<T> symmetric = ParallelAlteringOperationsExtensions.SymmetricParallel(matrix);
            if (ParallelPropertiesExtensions.DefinityParallel(symmetric) == DefinityClassification.PositiveDefinite)
            {
                int dim = matrix.Rows;
                result = new Matrix<T>(dim, dim);

                for (int i = 0; i < dim; i++)
                {
                    T multiply = new T();
                    for (int j = 0; j < i; j++)
                    {
                        multiply = (T)(multiply.__Addition(result.GetNumber(i, j).__Exponentiate(2)));
                    }
                    T write = (T)(symmetric.GetNumber(i, i).__Subtraction(multiply));
                    write = (T)write.__SquareRoot();
                    result.WriteNumber(i, i, write);

                    Parallel.ForEach(result.GetRowsChunks(i + 1), (pair) =>
                    {
                        for (int j = pair.Item1; j < pair.Item2; j++)
                        {
                            T tmpMultiply = new T();
                            for (int k = 0; k < i; k++)
                            {
                                tmpMultiply = (T)(tmpMultiply.__Addition(result.GetNumber(i, k).__Multiplication(result.GetNumber(j, k))));
                            }
                            T tmpWrite = (T)(symmetric.GetNumber(j, i).__Subtraction(tmpMultiply));
                            tmpWrite = (T)(tmpWrite.__Division(result.GetNumber(i, i)));
                            result.WriteNumber(j, i, tmpWrite);
                        }
                    });
                }
            }
            else
            {
                throw new MatrixLibraryException("Given matrix is not positive-definite");
            }
            return result;
        }

        /// <summary>
        /// On given <paramref name="matrix"/> object QR decomposition is calculated, Gram-Schmidt algorithm is used.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Source Matrix for computations.</param>
        /// <param name="Q">Output parameter in which Q Matrix will be stored. Q matrix has orthonormal columns.</param>
        /// <param name="R">Output parameter in which R Matrix will be stored. R Matrix is upper triangular.</param>
        /// <returns>Product of R and Q multiplication is returned.</returns>
        public static Matrix<T> QRDecompositionParallel<T>(this Matrix<T> matrix, out Matrix<T> Q, out Matrix<T> R) where T : IMatrixNumber, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result;
            int rows = matrix.Rows;
            int cols = matrix.Cols;
            Matrix<T> tmpMatrix = new Matrix<T>(matrix);
            Matrix<T> tmpQ = Matrix<T>.GetUninitializedMatrix(rows, rows);
            Matrix<T> tmpR = new Matrix<T>(rows, cols);

            for (int k = 0; k < cols && k < rows; ++k)
            {
                T norm = new T();
                for (int i = 0; i < rows; ++i)
                {
                    norm = (T)(norm.__Addition(tmpMatrix.GetNumber(i, k).__Exponentiate(2)));
                }
                norm = (T)norm.__SquareRoot();
                tmpR.WriteNumber(k, k, norm);

                Parallel.ForEach(matrix.GetRowsChunks(), (pair) =>
                {
                    for (int i = pair.Item1; i < pair.Item2; ++i)
                    {
                        tmpQ.WriteNumber(i, k, (T)(tmpMatrix.GetNumber(i, k).__Division(norm)));
                    }
                });

                Parallel.ForEach(matrix.GetColsChunks(k + 1), (pair) =>
                {
                    for (int j = pair.Item1; j < pair.Item2; ++j)
                    {
                        T dotProduct = new T();
                        for (int i = 0; i < rows; ++i)
                        {
                            dotProduct = (T)(dotProduct.__Addition(tmpMatrix.GetNumber(i, j).__Multiplication(tmpQ.GetNumber(i, k))));
                        }
                        tmpR.WriteNumber(k, j, (T)dotProduct);

                        for (int i = 0; i < rows; ++i)
                        {
                            tmpMatrix.WriteNumber(i, j,
                                (T)(tmpMatrix.GetNumber(i, j).__Subtraction(dotProduct.__Multiplication(tmpQ.GetNumber(i, k)))));
                        }
                    }
                });
            }

            result = ParallelClassicOperations.MultiplicationParallel(tmpR, tmpQ);
            R = tmpR;
            Q = tmpQ;

            return result;
        }
    }

    /// <summary>
    /// Namespace of extensions methods containing functions relating to matrix decompositions.
    /// </summary>
    public static class DecompositionsExtensions
    {
        /// <summary>
        /// Cholesky decomposition is performed in this function and its result, matrix L, will be returned to caller.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Matrix which will be decomposed, does not have to be symmetric.</param>
        /// <returns>Bottom triangular Matrix L, where <paramref name="matrix"/> = L * L^(T)</returns>
        public static Matrix<T> CholeskyDecomposition<T>(this Matrix<T> matrix) where T : IMatrixNumber, new()
        {
            /*
             * 
             * Cholesky decomposition: A = L * L^(T)
             *  - matrix A has to be positively-definite and of type n*n
             * 
             */

            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result;
            Matrix<T> symmetric = AlteringOperationsExtensions.Symmetric(matrix);
            if (PropertiesExtensions.Definity(symmetric) == DefinityClassification.PositiveDefinite)
            {
                int dim = matrix.Rows;
                result = new Matrix<T>(dim, dim);

                for (int i = 0; i < dim; i++)
                {
                    T multiply = new T();
                    for (int j = 0; j < i; j++)
                    {
                        multiply = (T)(multiply.__Addition(result.GetNumber(i, j).__Exponentiate(2)));
                    }
                    T write = (T)(symmetric.GetNumber(i, i).__Subtraction(multiply));
                    write = (T)write.__SquareRoot();
                    result.WriteNumber(i, i, write);

                    for (int j = i + 1; j < dim; j++)
                    {
                        multiply = new T();
                        for (int k = 0; k < i; k++)
                        {
                            multiply = (T)(multiply.__Addition(result.GetNumber(i, k).__Multiplication(result.GetNumber(j, k))));
                        }
                        write = (T)(symmetric.GetNumber(j, i).__Subtraction(multiply));
                        write = (T)(write.__Division(result.GetNumber(i, i)));
                        result.WriteNumber(j, i, write);
                    }
                }
            }
            else
            {
                throw new MatrixLibraryException("Given matrix is not positive-definite");
            }
            return result;
        }

        /// <summary>
        /// On given <paramref name="matrix"/> object QR decomposition is calculated, Gram-Schmidt algorithm is used.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Source Matrix for computations.</param>
        /// <param name="Q">Output parameter in which Q Matrix will be stored. Q matrix has orthonormal columns.</param>
        /// <param name="R">Output parameter in which R Matrix will be stored. R Matrix is upper triangular.</param>
        /// <returns>Product of R and Q multiplication is returned.</returns>
        public static Matrix<T> QRDecomposition<T>(this Matrix<T> matrix, out Matrix<T> Q, out Matrix<T> R) where T : IMatrixNumber, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result;
            int rows = matrix.Rows;
            int cols = matrix.Cols;
            Matrix<T> tmpMatrix = new Matrix<T>(matrix);
            Q = Matrix<T>.GetUninitializedMatrix(rows, rows);
            R = new Matrix<T>(rows, cols);

            for (int k = 0; k < cols && k < rows; ++k)
            {
                T norm = new T();
                for (int i = 0; i < rows; ++i)
                {
                    norm = (T)(norm.__Addition(tmpMatrix.GetNumber(i, k).__Exponentiate(2)));
                }
                norm = (T)norm.__SquareRoot();
                R.WriteNumber(k, k, norm);

                for (int i = 0; i < rows; ++i)
                {
                    Q.WriteNumber(i, k, (T)(tmpMatrix.GetNumber(i, k).__Division(norm)));
                }

                for (int j = k + 1; j < cols; ++j)
                {
                    T dotProduct = new T();
                    for (int i = 0; i < rows; ++i)
                    {
                        dotProduct = (T)(dotProduct.__Addition(tmpMatrix.GetNumber(i, j).__Multiplication(Q.GetNumber(i, k))));
                    }
                    R.WriteNumber(k, j, (T)dotProduct);

                    for (int i = 0; i < rows; ++i)
                    {
                        tmpMatrix.WriteNumber(i, j, 
                            (T)(tmpMatrix.GetNumber(i, j).__Subtraction(R.GetNumber(k, j).__Multiplication(Q.GetNumber(i, k)))));
                    }
                }
            }

            result = ClassicOperations.Multiplication(R, Q);

            return result;
        }
    }
}
