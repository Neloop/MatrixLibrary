using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLibrary
{
    public static class Decompositions
    {
        public static Matrix<T> CholeskyDecomposition<T>(Matrix<T> matrix) where T : MatrixNumberBase, new() // Je vrácena dolní trojúhelníková matice L, vstupní matice nemusí být symetrická, bude zesymetriována
        {
            /*
             * 
             * Choleského rozklad: A = L * L^(T)
             *  - matice A musí být Positivně-definitní a typu n*n
             * 
             */

            Matrix<T> result;
            Matrix<T> symmetric = AlteringOperations.Symmetric(matrix);
            if (Properties.Definity(symmetric) == Properties.DefinityClassification.PositiveDefinite)
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
                        for (int k = 0; k < i; k++)
                        {
                            if (k != 0) { multiply = (T)(multiply + result.GetNumber(i, k) * result.GetNumber(j, k)); }
                            else { multiply = (T)(result.GetNumber(i, k) * result.GetNumber(j, k)); }
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

        public static Matrix<T> QRDecomposition<T>(Matrix<T> matrix, out Matrix<T> Q, out Matrix<T> R) where T : MatrixNumberBase, new() // Vrácena je matice R*Q
        {
            Matrix<T> result;
            int rows = matrix.Rows;
            int cols = matrix.Cols;
            Q = new Matrix<T>(rows, cols);
            R = Matrix<T>.GetUninitializedMatrix(rows, cols);

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
                            T x = (T)matrix.GetNumber(i, l).Copy();
                            T z = (T)Q.GetNumber(k, l).Copy();

                            dotProduct = (T)((x * z) + dotProduct);
                        }

                        R.WriteNumber(k, i, dotProduct);

                        T times = (T)(dotProduct * Q.GetNumber(k, j));

                        sum = (T)(sum + times);
                    }

                    T write = (T)matrix.GetNumber(i, j).Copy();
                    write = (T)(write - sum);

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
