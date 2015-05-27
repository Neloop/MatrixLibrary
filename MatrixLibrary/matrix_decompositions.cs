using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLibrary
{
    public static class ConcurrentDecompositions
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

            Matrix<T> result;
            Matrix<T> symmetric = ConcurrentAlteringOperations.Symmetric(matrix);
            if (ConcurrentProperties.Definity(symmetric) == DefinityClassification.PositiveDefinite)
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
            Matrix<T> result;
            int rows = matrix.Rows;
            int cols = matrix.Cols;
            Matrix<T> tmpQ = new Matrix<T>(rows, cols);
            Matrix<T> tmpR = new Matrix<T>(rows, cols);

            for (int i = 0; i < rows; i++) // řádky
            {
                for (int j = 0; j < cols; j++) // sloupce
                {
                    T sum = new T();
                    object sumLock = new object();
                    Parallel.ForEach(tmpR.GetRowsChunks(0, i), (pair) =>
                    {
                        for (int k = pair.Item1; k < pair.Item2; k++) // suma...
                        {
                            T dotProduct = new T();
                            for (int l = 0; l < cols; l++) // skal. součin
                            {
                                dotProduct = (T)((matrix.GetNumber(i, l) * tmpQ.GetNumber(k, l)) + dotProduct);
                            }

                            tmpR.WriteNumber(k, i, dotProduct);

                            T times = (T)(dotProduct * tmpQ.GetNumber(k, j));
                            lock (sumLock) { sum = (T)(sum + times); }
                        }
                    });

                    T write = (T)(matrix.GetNumber(i, j) - sum);
                    tmpQ.WriteNumber(i, j, write);
                }

                T norm = new T();
                for (int j = 0; j < cols; j++) // vypočítá normu
                {
                    norm = (T)(norm + tmpQ.GetNumber(i, j).__Exponentiate(2));
                }
                norm = (T)norm.__SquareRoot();
                tmpR.WriteNumber(i, i, norm);

                Parallel.ForEach(tmpQ.GetColsChunks(), (pair) =>
                {
                    for (int j = pair.Item1; j < pair.Item2; j++) // vydělí všechny složky vektoru
                    {
                        tmpQ.WriteNumber(i, j, (T)(tmpQ.GetNumber(i, j) / norm));
                    }
                });
            }

            result = ConcurrentClassicOperations.Multiplication(tmpR, tmpQ);
            Q = tmpQ;
            R = tmpR;

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
            Matrix<T> result;
            int rows = matrix.Rows;
            int cols = matrix.Cols;
            Q = new Matrix<T>(rows, cols);
            R = new Matrix<T>(rows, cols);

            for (int i = 0; i < rows; i++) // řádky
            {
                for (int j = 0; j < cols; j++) // sloupce
                {
                    T sum = new T();
                    for (int k = 0; k < i; k++) // suma...
                    {
                        T dotProduct = new T();
                        for (int l = 0; l < cols; l++) // skal. součin
                        {
                            dotProduct = (T)((matrix.GetNumber(i, l) * Q.GetNumber(k, l)) + dotProduct);
                        }

                        R.WriteNumber(k, i, dotProduct);

                        T times = (T)(dotProduct * Q.GetNumber(k, j));
                        sum = (T)(sum + times);
                    }

                    T write = (T)(matrix.GetNumber(i, j) - sum);
                    Q.WriteNumber(i, j, write);
                }

                T norm = new T();
                for (int j = 0; j < cols; j++) // vypočítá normu
                {
                    norm = (T)(norm + Q.GetNumber(i, j).__Exponentiate(2));
                }
                norm = (T)norm.__SquareRoot();
                R.WriteNumber(i, i, norm);
                for (int j = 0; j < cols; j++) // vydělí všechny složky vektoru
                {
                    Q.WriteNumber(i, j, (T)(Q.GetNumber(i, j) / norm));
                }
            }

            result = ClassicOperations.Multiplication(R, Q);

            return result;
        }
    }
}
