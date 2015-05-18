using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLibrary
{
    public static class AlteringOperations
    {
        public static Matrix<T> Transposition<T>(Matrix<T> matrix) where T : MatrixNumberBase, new()
        {
            Matrix<T> result;
            int rows = matrix.Rows;
            int cols = matrix.Cols;
            result = Matrix<T>.GetUninitializedMatrix(rows, cols);

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    result.WriteNumber(i, j, matrix.GetNumber(j, i));
                }
            }
            
            return result;
        }

        public static Matrix<T> Transposition_MultiThreaded<T>(Matrix<T> matrix) where T : MatrixNumberBase, new()
        {
            Matrix<T> result;
            int rows = matrix.Rows;
            int cols = matrix.Cols;
            result = Matrix<T>.GetUninitializedMatrix(rows, cols);

            Parallel.ForEach(result.GetRowsChunks(), (pair) =>
            {
                for (int i = pair.Item1; i < pair.Item2; i++)
                {
                    for (int j = 0; j < rows; j++)
                    {
                        result.WriteNumber(i, j, matrix.GetNumber(j, i));
                    }
                }
            });

            return result;
        }

        public static Matrix<T> Symmetric<T>(Matrix<T> matrix) where T : MatrixNumberBase, new() // Zesymetrizuje zadanou matici, v případě nerovnosti řádků a sloupců vyhazuje vyjimku
        {
            Matrix<T> result;
            if (matrix.Rows == matrix.Cols)
            {
                int dim = matrix.Rows;

                Matrix<T> transpose = AlteringOperations.Transposition(matrix);
                result = ClassicOperations.Addition(matrix, transpose);

                T two = new T();
                two.AddInt(2);
                for (int i = 0; i < dim; i++) // Vydělí všechna čísla dvěma
                {
                    for (int j = 0; j < dim; j++)
                    {
                        result.WriteNumber(i, j, (T)(result.GetNumber(i, j) / two));
                    }
                }
            }
            else
            {
                throw new MatrixLibraryException("Given matrix does not have same number of rows and columns");
            }
            return result;
        }

        public static Matrix<T> Symmetric_MultiThreaded<T>(Matrix<T> matrix) where T : MatrixNumberBase, new() // Zesymetrizuje zadanou matici, v případě nerovnosti řádků a sloupců vyhazuje vyjimku
        {
            Matrix<T> result;
            if (matrix.Rows == matrix.Cols)
            {
                int dim = matrix.Rows;

                Matrix<T> transpose = AlteringOperations.Transposition_MultiThreaded(matrix);
                result = ClassicOperations.Addition_MultiThreaded(matrix, transpose);

                T two = new T();
                two.AddInt(2);
                Parallel.ForEach(result.GetRowsChunks(), (pair) =>
                {
                    for (int i = pair.Item1; i < pair.Item2; i++) // Vydělí všechna čísla dvěma
                    {
                        for (int j = 0; j < dim; j++)
                        {
                            result.WriteNumber(i, j, (T)(result.GetNumber(i, j) / two));
                        }
                    }
                });
            }
            else
            {
                throw new MatrixLibraryException("Given matrix does not have same number of rows and columns");
            }
            return result;
        }

        public static Matrix<T> Gauss<T>(Matrix<T> matrix) where T : MatrixNumberBase, new() // Pouze Gaussova eliminace, postupně se prochází řádky matice a "hledají" se pivoty
        {
            Matrix<T> result;
            int rows = matrix.Rows;
            int cols = matrix.Cols;
            result = new Matrix<T>(matrix);

            for (int i = 0; i < rows; i++)
            {
                for (int j = i; j < cols; j++)
                {
                    if (result.GetNumber(i, j).IsZero())
                    {
                        // pokud je prvek nula, tak se koukne pod něj a případně prohodí řádek a vydělí řádky pod ním (potom se breakne), 
                        // pokud i pod ním jsou nuly, pak se breakne (nemusí prostě se nechá doběhnout) cyklus a jde na další sloupec
                        for (int k = i + 1; k < rows; k++)
                        {
                            if (!result.GetNumber(k, j).IsZero())
                            {
                                for (int l = j; l < cols; l++)
                                {
                                    result.SwapElements(k, l, i, l);
                                }

                                T divide = result.GetNumber(i, j);
                                for (int l = j; l < cols; l++)
                                {
                                    result.WriteNumber(i, l, (T)(result.GetNumber(i, l) / divide));
                                }

                                for (int a = i + 1; a < rows; a++)
                                {
                                    divide = result.GetNumber(a, j);
                                    if (!divide.IsZero())
                                    {
                                        for (int b = j; b < cols; b++)
                                        {
                                            T tmp = (T)(result.GetNumber(a, b) / divide);
                                            tmp = (T)(tmp - result.GetNumber(i, b));
                                            result.WriteNumber(a, b, tmp);
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                    else // Pokud se narazilo na nenulový prvek (=pivot)
                    {
                        T divide = result.GetNumber(i, j);
                        for (int k = j; k < cols; k++)
                        {
                            result.WriteNumber(i, k, (T)(result.GetNumber(i, k) / divide));
                        }

                        for (int k = i + 1; k < rows; k++)
                        {
                            divide = result.GetNumber(k, j);
                            if (!divide.IsZero())
                            {
                                for (int l = j; l < cols; l++)
                                {
                                    T tmp = (T)(result.GetNumber(k, l) / divide);
                                    tmp = (T)(tmp - result.GetNumber(i, l));
                                    result.WriteNumber(k, l, tmp);
                                }
                            }
                        }

                        break;
                    }
                }
            }

            return result;
        }

        public static Matrix<T> Gauss_MultiThreaded<T>(Matrix<T> matrix) where T : MatrixNumberBase, new() // Pouze Gaussova eliminace, postupně se prochází řádky matice a "hledají" se pivoty
        {
            Matrix<T> result;
            int rows = matrix.Rows;
            int cols = matrix.Cols;
            result = new Matrix<T>(matrix);

            for (int i = 0; i < rows; i++)
            {
                for (int j = i; j < cols; j++)
                {
                    if (result.GetNumber(i, j).IsZero())
                    {
                        // pokud je prvek nula, tak se koukne pod něj a případně prohodí řádek a vydělí řádky pod ním (potom se breakne), 
                        // pokud i pod ním jsou nuly, pak se breakne (nemusí prostě se nechá doběhnout) cyklus a jde na další sloupec
                        for (int k = i + 1; k < rows; k++)
                        {
                            if (!result.GetNumber(k, j).IsZero())
                            {
                                for (int l = j; l < cols; l++)
                                {
                                    result.SwapElements(k, l, i, l);
                                }

                                T divide = result.GetNumber(i, j);
                                Parallel.ForEach(result.GetColsChunks(j), (pair) =>
                                {
                                    for (int l = pair.Item1; l < pair.Item2; l++)
                                    {
                                        result.WriteNumber(i, l, (T)(result.GetNumber(i, l) / divide));
                                    }
                                });

                                Parallel.ForEach(result.GetRowsChunks(i + 1), (pair) =>
                                {
                                    for (int a = pair.Item1; a < pair.Item2; a++)
                                    {
                                        T tmpDivide = result.GetNumber(a, j);
                                        if (!tmpDivide.IsZero())
                                        {
                                            for (int b = j; b < cols; b++)
                                            {
                                                T tmp = (T)(result.GetNumber(a, b) / tmpDivide);
                                                tmp = (T)(tmp - result.GetNumber(i, b));
                                                result.WriteNumber(a, b, tmp);
                                            }
                                        }
                                    }
                                });

                                break;
                            }
                        }
                    }
                    else // Pokud se narazilo na nenulový prvek (=pivot)
                    {
                        T divide = result.GetNumber(i, j);
                        Parallel.ForEach(result.GetColsChunks(j), (pair) =>
                        {
                            for (int k = pair.Item1; k < pair.Item2; k++)
                            {
                                result.WriteNumber(i, k, (T)(result.GetNumber(i, k) / divide));
                            }
                        });

                        Parallel.ForEach(result.GetRowsChunks(i + 1), (pair) =>
                        {
                            for (int k = pair.Item1; k < pair.Item2; k++)
                            {
                                T tmpDivide = result.GetNumber(k, j);
                                if (!tmpDivide.IsZero())
                                {
                                    for (int l = j; l < cols; l++)
                                    {
                                        T tmp = (T)(result.GetNumber(k, l) / tmpDivide);
                                        tmp = (T)(tmp - result.GetNumber(i, l));
                                        result.WriteNumber(k, l, tmp);
                                    }
                                }
                            }
                        });

                        break;
                    }
                }
            }

            return result;
        }

        public static Matrix<T> GaussJordan<T>(Matrix<T> matrix) where T : MatrixNumberBase, new() // K počítání se používá pouze normální Gaussova eliminace, nejdříve na původní matici a pak na "obrácenou"
        {
            Matrix<T> result;
            int rows = matrix.Rows;
            int cols = matrix.Cols;

            int halfOfRow;
            int halfOfCol;
            if ((rows % 2) == 0) { halfOfRow = rows / 2; }
            else { halfOfRow = (rows / 2) + 1; }
            if ((cols % 2) == 0) { halfOfCol = cols / 2; }
            else { halfOfCol = (cols / 2) + 1; }

            result = AlteringOperations.Gauss(matrix); // První Gaussovka

            int zeroRow1, zeroRow2;
            bool isZeroRow = false;
            for (int i = 0; i < halfOfRow; i++) // Vymění se prvky v matici a následně se provede "obrácená" Gaussovka
            {
                zeroRow1 = 0;
                zeroRow2 = 0;
                for (int j = 0; j < cols; j++)
                {
                    if ((rows % 2) == 1 && halfOfCol == j && (halfOfRow - 1) == i) { break; }

                    if (result.GetNumber(rows - i - 1, cols - j - 1).IsZero()) { zeroRow1++; }
                    if (result.GetNumber(i, j).IsZero()) { zeroRow2++; }

                    result.SwapElements(i, j, rows - i - 1, cols - j - 1);
                }
                if (zeroRow1 == cols || zeroRow2 == cols) { isZeroRow = true; }
            }
            if (isZeroRow == false) { result = AlteringOperations.Gauss(result); }

            for (int i = 0; i < halfOfRow; i++) // Vymění se prvky v matici a vrátí výsledek
            {
                for (int j = 0; j < cols; j++)
                {
                    if ((rows % 2) == 1 && halfOfCol == j && (halfOfRow - 1) == i) { break; }

                    result.SwapElements(i, j, rows - i - 1, cols - j - 1);
                }
            }

            return result;
        }

        public static Matrix<T> GaussJordan_MultiThreaded<T>(Matrix<T> matrix) where T : MatrixNumberBase, new() // K počítání se používá pouze normální Gaussova eliminace, nejdříve na původní matici a pak na "obrácenou"
        {
            Matrix<T> result;
            int rows = matrix.Rows;
            int cols = matrix.Cols;

            int halfOfRow;
            int halfOfCol;
            if ((rows % 2) == 0) { halfOfRow = rows / 2; }
            else { halfOfRow = (rows / 2) + 1; }
            if ((cols % 2) == 0) { halfOfCol = cols / 2; }
            else { halfOfCol = (cols / 2) + 1; }

            result = AlteringOperations.Gauss_MultiThreaded(matrix); // První Gaussovka

            bool isZeroRow = false;
            Parallel.ForEach(result.GetHalfRowsChunks(), (pair) =>
            {
                for (int i = pair.Item1; i < pair.Item2; i++) // Vymění se prvky v matici a následně se provede "obrácená" Gaussovka
                {
                    int zeroRow1 = 0;
                    int zeroRow2 = 0;
                    for (int j = 0; j < cols; j++)
                    {
                        if ((rows % 2) == 1 && halfOfCol == j && (halfOfRow - 1) == i) { break; }

                        if (result.GetNumber(rows - i - 1, cols - j - 1).IsZero()) { zeroRow1++; }
                        if (result.GetNumber(i, j).IsZero()) { zeroRow2++; }

                        result.SwapElements(i, j, rows - i - 1, cols - j - 1);
                    }
                    if (zeroRow1 == cols || zeroRow2 == cols) { isZeroRow = true; }
                }
            });
            if (isZeroRow == false) { result = AlteringOperations.Gauss_MultiThreaded(result); }

            Parallel.ForEach(result.GetHalfRowsChunks(), (pair) =>
            {
                for (int i = pair.Item1; i < pair.Item2; i++) // Vymění se prvky v matici a vrátí výsledek
                {
                    for (int j = 0; j < cols; j++)
                    {
                        if ((rows % 2) == 1 && halfOfCol == j && (halfOfRow - 1) == i) { break; }

                        result.SwapElements(i, j, rows - i - 1, cols - j - 1);
                    }
                }
            });

            return result;
        }

        public static Matrix<T> Inverse<T>(Matrix<T> matrix) where T : MatrixNumberBase, new() // Pokud matice není regulární, tak vyhazuje vyjimku
        {
            Matrix<T> result;
            if (Properties.IsInvertible(matrix) == true)
            {
                int rows = matrix.Rows;
                int cols = matrix.Cols;
                result = Matrix<T>.GetUninitializedMatrix(rows, cols);

                int halfOfRow;
                int halfOfCol;
                if ((rows % 2) == 0) { halfOfRow = rows / 2; }
                else { halfOfRow = (rows / 2) + 1; }
                if ((cols % 2) == 0) { halfOfCol = cols / 2; }
                else { halfOfCol = (cols / 2) + 1; }

                Matrix<T> modify = Matrix<T>.GetUninitializedMatrix(rows, cols * 2); // Matice upravit má stejný počet řádků jako matice a 2x větší počet sloupců

                for (int i = 0; i < rows; i++) // Zapíše do matice upravit původní matici
                {
                    for (int j = 0; j < cols; j++)
                    {
                        modify.WriteNumber(i, j, matrix.GetNumber(i, j));
                    }
                }
                T one = new T();
                one.AddInt(1);
                T zero = new T();
                for (int i = 0; i < rows; i++) // Zapíše do matice upravit jednotkovou matici
                {
                    for (int j = cols; j < (cols * 2); j++)
                    {
                        if (i == (j - cols)) { modify.WriteNumber(i, j, one); }
                        else { modify.WriteNumber(i, j, zero); }
                    }
                }

                modify = AlteringOperations.Gauss(modify); // První Gaussovka

                // Převrácení a druhá Gaussovka:

                for (int i = 0; i < halfOfRow; i++) // Převrácení
                {
                    for (int j = 0; j < cols; j++)
                    {
                        if ((rows % 2) == 1 && halfOfCol == j && (halfOfRow - 1) == i) { break; }
                        modify.SwapElements(i, j, rows - i - 1, cols - j - 1);

                        // Převrácení původně jednotkové matice:
                        modify.SwapElements(i, cols + j, rows - i - 1, (cols * 2) - j - 1);
                    }
                }

                modify = AlteringOperations.Gauss(modify); // Druhá Gaussovka

                for (int i = 0; i < rows; i++) // Převrácení a složení výsledné matice
                {
                    for (int j = 0; j < cols; j++)
                    {
                        result.WriteNumber(rows - i - 1, cols - j - 1, modify.GetNumber(i, j + cols));
                    }
                }
            }
            else
            {
                throw new MatrixLibraryException("Given matrix is not regular");
            }
            return result;
        }

        public static Matrix<T> Inverse_MultiThreaded<T>(Matrix<T> matrix) where T : MatrixNumberBase, new() // Pokud matice není regulární, tak vyhazuje vyjimku
        {
            Matrix<T> result;
            if (Properties.IsInvertible_MultiThreaded(matrix) == true)
            {
                int rows = matrix.Rows;
                int cols = matrix.Cols;
                result = Matrix<T>.GetUninitializedMatrix(rows, cols);

                int halfOfRow;
                int halfOfCol;
                if ((rows % 2) == 0) { halfOfRow = rows / 2; }
                else { halfOfRow = (rows / 2) + 1; }
                if ((cols % 2) == 0) { halfOfCol = cols / 2; }
                else { halfOfCol = (cols / 2) + 1; }

                Matrix<T> modify = Matrix<T>.GetUninitializedMatrix(rows, cols * 2); // Matice upravit má stejný počet řádků jako matice a 2x větší počet sloupců

                Parallel.ForEach(modify.GetRowsChunks(), (pair) => 
                {
                    for (int i = pair.Item1; i < pair.Item2; i++) // Zapíše do matice upravit původní matici
                    {
                        for (int j = 0; j < cols; j++)
                        {
                            modify.WriteNumber(i, j, matrix.GetNumber(i, j));
                        }
                    }
                });
                T one = new T();
                one.AddInt(1);
                T zero = new T();
                Parallel.ForEach(modify.GetRowsChunks(), (pair) =>
                {
                    for (int i = pair.Item1; i < pair.Item2; i++) // Zapíše do matice upravit jednotkovou matici
                    {
                        for (int j = cols; j < (cols * 2); j++)
                        {
                            if (i == (j - cols)) { modify.WriteNumber(i, j, one); }
                            else { modify.WriteNumber(i, j, zero); }
                        }
                    }
                });

                modify = AlteringOperations.Gauss_MultiThreaded(modify); // První Gaussovka

                // Převrácení a druhá Gaussovka:

                Parallel.ForEach(modify.GetHalfRowsChunks(), (pair) =>
                {
                    for (int i = pair.Item1; i < pair.Item2; i++) // Převrácení
                    {
                        for (int j = 0; j < cols; j++)
                        {
                            if ((rows % 2) == 1 && halfOfCol == j && (halfOfRow - 1) == i) { break; }
                            modify.SwapElements(i, j, rows - i - 1, cols - j - 1);

                            // Převrácení původně jednotkové matice:
                            modify.SwapElements(i, cols + j, rows - i - 1, (cols * 2) - j - 1);
                        }
                    }
                });

                modify = AlteringOperations.Gauss_MultiThreaded(modify); // Druhá Gaussovka

                Parallel.ForEach(result.GetRowsChunks(), (pair) =>
                {
                    for (int i = pair.Item1; i < pair.Item2; i++) // Převrácení a složení výsledné matice
                    {
                        for (int j = 0; j < cols; j++)
                        {
                            result.WriteNumber(rows - i - 1, cols - j - 1, modify.GetNumber(i, j + cols));
                        }
                    }
                });
            }
            else
            {
                throw new MatrixLibraryException("Given matrix is not regular");
            }
            return result;
        }

        public static Matrix<T> Adjugate<T>(Matrix<T> matrix) where T : MatrixNumberBase, new()
        {
            int rows = matrix.Rows;
            int cols = matrix.Cols;
            Matrix<T> result = Matrix<T>.GetUninitializedMatrix(rows, cols);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    T multiply;
                    if ((i + j) % 2 == 0) { multiply = new T(); multiply.AddInt(1); }
                    else { multiply = new T(); multiply.AddInt(-1); }

                    Matrix<T> modified = Matrix<T>.GetUninitializedMatrix(rows - 1, cols - 1);
                    int subtractK = 0;

                    for (int k = 0; k < rows; k++)
                    {
                        if (k == j) { subtractK = 1; continue; }
                        int subtractL = 0;

                        for (int l = 0; l < cols; l++)
                        {
                            if (l == i) { subtractL = 1; continue; }
                            else
                            {
                                modified.WriteNumber(k - subtractK, l - subtractL, matrix.GetNumber(k, l));
                            }
                        }
                    }
                    T tmp = Computations.Determinant(modified);
                    result.WriteNumber(i, j, (T)(multiply * tmp));
                }
            }

            return result;
        }

        public static Matrix<T> Adjugate_MultiThreaded<T>(Matrix<T> matrix) where T : MatrixNumberBase, new()
        {
            int rows = matrix.Rows;
            int cols = matrix.Cols;
            Matrix<T> result = Matrix<T>.GetUninitializedMatrix(rows, cols);

            Parallel.ForEach(result.GetRowsChunks(), (pair) =>
            {
                for (int i = pair.Item1; i < pair.Item2; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        T multiply;
                        if ((i + j) % 2 == 0) { multiply = new T(); multiply.AddInt(1); }
                        else { multiply = new T(); multiply.AddInt(-1); }

                        Matrix<T> modified = Matrix<T>.GetUninitializedMatrix(rows - 1, cols - 1);
                        int subtractK = 0;

                        for (int k = 0; k < rows; k++)
                        {
                            if (k == j) { subtractK = 1; continue; }
                            int subtractL = 0;

                            for (int l = 0; l < cols; l++)
                            {
                                if (l == i) { subtractL = 1; continue; }
                                else
                                {
                                    modified.WriteNumber(k - subtractK, l - subtractL, matrix.GetNumber(k, l));
                                }
                            }
                        }

                        result.WriteNumber(i, j, (T)(multiply * Computations.Determinant(modified)));
                    }
                }
            });

            return result;
        }

        public static Matrix<T> Orthogonal<T>(Matrix<T> matrix) where T : MatrixNumberBase, new() // Využívá Gram-Schmidtovu ortogonalizaci, vstupem by měli být lineárně nezávislé vektory
        {
            Matrix<T> result;
            int rows = matrix.Rows;
            int cols = matrix.Cols;
            result = new Matrix<T>(rows, cols);

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
                            dotProduct = (T)((matrix.GetNumber(i, l) * result.GetNumber(k, l)) + dotProduct);
                        }

                        T times = (T)(dotProduct * result.GetNumber(k, j));
                        sum = (T)(sum + times);
                    }

                    result.WriteNumber(i, j, (T)(matrix.GetNumber(i, j) - sum));
                }

                T norm = new T();
                for (int j = 0; j < cols; j++) // vypočítá normu
                {
                    norm = (T)(norm + result.GetNumber(i, j).__Exponentiate(2));
                }
                norm = (T)norm.__SquareRoot();
                for (int j = 0; j < cols; j++) // vydělí všechny složky vektoru
                {
                    result.WriteNumber(i, j, (T)(result.GetNumber(i, j) / norm));
                }
            }

            return result;
        }
    }
}
