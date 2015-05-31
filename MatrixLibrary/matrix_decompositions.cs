using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLibrary
{
    public static class ParallelDecompositions
    {
        /// <summary>
        /// Je vrácena dolní trojúhelníková matice L, vstupní matice nemusí být symetrická, bude zesymetriována
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Matrix<T> CholeskyDecomposition<T>(Matrix<T> matrix) where T : MatrixNumberBase, new()
        {
            /*
             * 
             * Choleského rozklad: A = L * L^(T)
             *  - matice A musí být Positivně-definitní a typu n*n
             * 
             */

            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result;
            Matrix<T> symmetric = ParallelAlteringOperations.Symmetric(matrix);
            if (ParallelProperties.Definity(symmetric) == DefinityClassification.PositiveDefinite)
            {
                int dim = matrix.Rows;
                result = new Matrix<T>(dim, dim);

                for (int i = 0; i < dim; i++)
                {
                    T multiply = new T();
                    for (int j = 0; j < i; j++)
                    {
                        multiply = (T)(multiply + result.GetNumber(i, j).__Exponentiate(2));
                    }
                    T write = (T)(symmetric.GetNumber(i, i) - multiply);
                    write = (T)write.__SquareRoot();
                    result.WriteNumber(i, i, write);

                    Parallel.ForEach(result.GetRowsChunks(i + 1), (pair) =>
                    {
                        for (int j = pair.Item1; j < pair.Item2; j++)
                        {
                            T tmpMultiply = new T();
                            for (int k = 0; k < i; k++)
                            {
                                tmpMultiply = (T)(tmpMultiply + result.GetNumber(i, k) * result.GetNumber(j, k));
                            }
                            T tmpWrite = (T)(symmetric.GetNumber(j, i) - tmpMultiply);
                            tmpWrite = (T)(tmpWrite / result.GetNumber(i, i));
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
        /// Vrácena je matice R*Q
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <param name="Q"></param>
        /// <param name="R"></param>
        /// <returns></returns>
        public static Matrix<T> QRDecomposition<T>(Matrix<T> matrix, out Matrix<T> Q, out Matrix<T> R) where T : MatrixNumberBase, new()
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
                    norm = (T)(norm + tmpMatrix.GetNumber(i, k).__Exponentiate(2));
                }
                norm = (T)norm.__SquareRoot();
                tmpR.WriteNumber(k, k, norm);

                Parallel.ForEach(matrix.GetRowsChunks(), (pair) =>
                {
                    for (int i = pair.Item1; i < pair.Item2; ++i)
                    {
                        tmpQ.WriteNumber(i, k, (T)(tmpMatrix.GetNumber(i, k) / norm));
                    }
                });

                Parallel.ForEach(matrix.GetColsChunks(k + 1), (pair) =>
                {
                    for (int j = pair.Item1; j < pair.Item2; ++j)
                    {
                        T dotProduct = new T();
                        for (int i = 0; i < rows; ++i)
                        {
                            dotProduct = (T)(dotProduct + (tmpMatrix.GetNumber(i, j) * tmpQ.GetNumber(i, k)));
                        }
                        tmpR.WriteNumber(k, j, (T)dotProduct);

                        for (int i = 0; i < rows; ++i)
                        {
                            tmpMatrix.WriteNumber(i, j, (T)(tmpMatrix.GetNumber(i, j) - (dotProduct * tmpQ.GetNumber(i, k))));
                        }
                    }
                });
            }

            result = ParallelClassicOperations.Multiplication(tmpR, tmpQ);
            R = tmpR;
            Q = tmpQ;

            return result;
        }
    }

    public static class Decompositions
    {
        /// <summary>
        /// Je vrácena dolní trojúhelníková matice L, vstupní matice nemusí být symetrická, bude zesymetriována
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Matrix<T> CholeskyDecomposition<T>(Matrix<T> matrix) where T : MatrixNumberBase, new()
        {
            /*
             * 
             * Choleského rozklad: A = L * L^(T)
             *  - matice A musí být Positivně-definitní a typu n*n
             * 
             */

            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result;
            Matrix<T> symmetric = AlteringOperations.Symmetric(matrix);
            if (Properties.Definity(symmetric) == DefinityClassification.PositiveDefinite)
            {
                int dim = matrix.Rows;
                result = new Matrix<T>(dim, dim);

                for (int i = 0; i < dim; i++)
                {
                    T multiply = new T();
                    for (int j = 0; j < i; j++)
                    {
                        multiply = (T)(multiply + result.GetNumber(i, j).__Exponentiate(2));
                    }
                    T write = (T)(symmetric.GetNumber(i, i) - multiply);
                    write = (T)write.__SquareRoot();
                    result.WriteNumber(i, i, write);

                    for (int j = i + 1; j < dim; j++)
                    {
                        multiply = new T();
                        for (int k = 0; k < i; k++)
                        {
                            multiply = (T)(multiply + result.GetNumber(i, k) * result.GetNumber(j, k));
                        }
                        write = (T)(symmetric.GetNumber(j, i) - multiply);
                        write = (T)(write / result.GetNumber(i, i));
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

        public static Matrix<T> QRDecomposition<T>(Matrix<T> matrix, out Matrix<T> Q, out Matrix<T> R) where T : MatrixNumberBase, new()
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
                    norm = (T)(norm + tmpMatrix.GetNumber(i, k).__Exponentiate(2));
                }
                norm = (T)norm.__SquareRoot();
                R.WriteNumber(k, k, norm);

                for (int i = 0; i < rows; ++i)
                {
                    Q.WriteNumber(i, k, (T)(tmpMatrix.GetNumber(i, k) / norm));
                }

                for (int j = k + 1; j < cols; ++j)
                {
                    T dotProduct = new T();
                    for (int i = 0; i < rows; ++i)
                    {
                        dotProduct = (T)(dotProduct + (tmpMatrix.GetNumber(i, j) * Q.GetNumber(i, k)));
                    }
                    R.WriteNumber(k, j, (T)dotProduct);

                    for (int i = 0; i < rows; ++i)
                    {
                        tmpMatrix.WriteNumber(i, j, (T)(tmpMatrix.GetNumber(i, j) - (R.GetNumber(k, j) * Q.GetNumber(i, k))));
                    }
                }
            }

            result = ClassicOperations.Multiplication(R, Q);

            return result;
        }
    }
}
