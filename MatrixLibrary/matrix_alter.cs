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

            Parallel.ForEach(result.GetRowChunks(), (pair) =>
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
                Parallel.ForEach(result.GetRowChunks(), (pair) =>
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

        public static Matrix<T> Gauss<T>(Matrix<T> matice) where T : MatrixNumberBase, new() // Pouze Gaussova eliminace, postupně se prochází řádky matice a "hledají" se pivoty
        {
            Matrix<T> vysledek;
            int radky, sloupce;
            radky = matice.Rows;
            sloupce = matice.Cols;
            vysledek = new Matrix<T>(matice);

            for (int i = 0; i < radky; i++)
            {
                for (int j = i; j < sloupce; j++)
                {
                    if (vysledek.GetNumber(i, j).IsZero())
                    {
                        // pokud je prvek nula, tak se koukne pod něj a případně prohodí řádek a vydělí řádky pod ním (potom se breakne), 
                        // pokud i pod ním jsou nuly, pak se breakne (nemusí prostě se nechá doběhnout) cyklus a jde na další sloupec
                        for (int k = i + 1; k < radky; k++)
                        {
                            if (!vysledek.GetNumber(k, j).IsZero())
                            {
                                for (int l = j; l < sloupce; l++)
                                {
                                    vysledek.WriteNumber(k, l, vysledek.GetNumber(i, l));
                                    vysledek.WriteNumber(i, l, vysledek.GetNumber(k, l));
                                }

                                T vydelit = vysledek.GetNumber(i, j);
                                for (int l = j; l < sloupce; l++)
                                {
                                    vysledek.WriteNumber(i, l, (T)(vysledek.GetNumber(i, l) / vydelit));
                                }

                                for (int a = i + 1; a < radky; a++)
                                {
                                    vydelit = vysledek.GetNumber(k, j);
                                    if (!vydelit.IsZero())
                                    {
                                        for (int b = j; b < sloupce; b++)
                                        {
                                            vysledek.WriteNumber(a, b, (T)(vysledek.GetNumber(a, b) / vydelit));
                                            vysledek.WriteNumber(a, b, (T)(vysledek.GetNumber(a, b) - vysledek.GetNumber(i, b)));
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                    else // Pokud se narazilo na nenulový prvek (=pivot)
                    {
                        T vydelit = vysledek.GetNumber(i, j);
                        for (int k = j; k < sloupce; k++)
                        {
                            vysledek.WriteNumber(i, k, (T)(vysledek.GetNumber(i, k) / vydelit));
                        }

                        for (int k = i + 1; k < radky; k++)
                        {
                            vydelit = vysledek.GetNumber(k, j);
                            if (!vydelit.IsZero())
                            {
                                for (int l = j; l < sloupce; l++)
                                {
                                    vysledek.WriteNumber(k, l, (T)(vysledek.GetNumber(k, l) / vydelit));
                                    vysledek.WriteNumber(k, l, (T)(vysledek.GetNumber(k, l) - vysledek.GetNumber(i, l)));
                                }
                            }
                        }

                        break;
                    }
                }
            }

            return vysledek;
        }

        public static Matrix<T> Gauss_MultiThreaded<T>(Matrix<T> matice) where T : MatrixNumberBase, new() // Pouze Gaussova eliminace, postupně se prochází řádky matice a "hledají" se pivoty
        {
            Matrix<T> vysledek;
            int radky, sloupce;
            radky = matice.Rows;
            sloupce = matice.Cols;
            vysledek = new Matrix<T>(matice);

            for (int i = 0; i < radky; i++)
            {
                for (int j = i; j < sloupce; j++)
                {
                    if (vysledek.GetNumber(i, j).IsZero())
                    {
                        // pokud je prvek nula, tak se koukne pod něj a případně prohodí řádek a vydělí řádky pod ním (potom se breakne), 
                        // pokud i pod ním jsou nuly, pak se breakne (nemusí prostě se nechá doběhnout) cyklus a jde na další sloupec
                        for (int k = i + 1; k < radky; k++)
                        {
                            if (!vysledek.GetNumber(k, j).IsZero())
                            {
                                for (int l = j; l < sloupce; l++)
                                {
                                    vysledek.WriteNumber(k, l, vysledek.GetNumber(i, l));
                                    vysledek.WriteNumber(i, l, vysledek.GetNumber(k, l));
                                }

                                T vydelit = vysledek.GetNumber(i, j);
                                for (int l = j; l < sloupce; l++)
                                {
                                    vysledek.WriteNumber(i, l, (T)(vysledek.GetNumber(i, l) / vydelit));
                                }

                                for (int a = i + 1; a < radky; a++)
                                {
                                    vydelit = vysledek.GetNumber(k, j);
                                    if (!vydelit.IsZero())
                                    {
                                        for (int b = j; b < sloupce; b++)
                                        {
                                            vysledek.WriteNumber(a, b, (T)(vysledek.GetNumber(a, b) / vydelit));
                                            vysledek.WriteNumber(a, b, (T)(vysledek.GetNumber(a, b) - vysledek.GetNumber(i, b)));
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                    else // Pokud se narazilo na nenulový prvek (=pivot)
                    {
                        T vydelit = vysledek.GetNumber(i, j);
                        for (int k = j; k < sloupce; k++)
                        {
                            vysledek.WriteNumber(i, k, (T)(vysledek.GetNumber(i, k) / vydelit));
                        }

                        for (int k = i + 1; k < radky; k++)
                        {
                            vydelit = vysledek.GetNumber(k, j);
                            if (!vydelit.IsZero())
                            {
                                for (int l = j; l < sloupce; l++)
                                {
                                    vysledek.WriteNumber(k, l, (T)(vysledek.GetNumber(k, l) / vydelit));
                                    vysledek.WriteNumber(k, l, (T)(vysledek.GetNumber(k, l) - vysledek.GetNumber(i, l)));
                                }
                            }
                        }

                        break;
                    }
                }
            }

            return vysledek;
        }

        public static Matrix<T> GaussJordan<T>(Matrix<T> matice) where T : MatrixNumberBase, new() // K počítání se používá pouze normální Gaussova eliminace, nejdříve na původní matici a pak na "obrácenou"
        {
            Matrix<T> vysledek;
            int radky, sloupce;
            radky = matice.Rows;
            sloupce = matice.Cols;
            vysledek = new Matrix<T>(radky, sloupce);
            int pulka_radky;
            int pulka_sloupce;
            if ((radky % 2) == 0) { pulka_radky = radky / 2; }
            else { pulka_radky = (radky / 2) + 1; }
            if ((sloupce % 2) == 0) { pulka_sloupce = sloupce / 2; }
            else { pulka_sloupce = (sloupce / 2) + 1; }

            vysledek = AlteringOperations.Gauss(matice); // První Gaussovka

            int nul_radek_1, nul_radek_2;
            bool nul_radek = false;
            for (int i = 0; i < pulka_radky; i++) // Vymění se prvky v matici a následně se provede "obrácená" Gaussovka
            {
                nul_radek_1 = 0;
                nul_radek_2 = 0;
                for (int j = 0; j < sloupce; j++)
                {
                    if ((radky % 2) == 1 && pulka_sloupce == j && (pulka_radky - 1) == i) { break; }

                    if (vysledek.GetNumber(radky - i - 1, sloupce - j - 1).IsZero()) { nul_radek_1++; }
                    if (vysledek.GetNumber(i, j).IsZero()) { nul_radek_2++; }

                    vysledek.SwapElements(i, j, radky - i - 1, sloupce - j - 1);
                }
                if (nul_radek_1 == sloupce || nul_radek_2 == sloupce) { nul_radek = true; }
            }
            if (nul_radek == false) { vysledek = AlteringOperations.Gauss(vysledek); }

            for (int i = 0; i < pulka_radky; i++) // Vymění se prvky v matici a vrátí výsledek
            {
                for (int j = 0; j < sloupce; j++)
                {
                    if ((radky % 2) == 1 && pulka_sloupce == j && (pulka_radky - 1) == i) { break; }

                    vysledek.SwapElements(i, j, radky - i - 1, sloupce - j - 1);
                }
            }

            return vysledek;
        }
        public static Matrix<T> Inverzni<T>(Matrix<T> matice) where T : MatrixNumberBase, new() // Pokud matice není regulární, tak vyhazuje vyjimku
        {
            Matrix<T> vysledek;
            if (Properties.IsInvertible(matice) == true)
            {
                int radky, sloupce;
                radky = matice.Rows;
                sloupce = matice.Cols;
                vysledek = new Matrix<T>(radky, sloupce);
                int pulka_radky;
                int pulka_sloupce;
                if ((radky % 2) == 0) { pulka_radky = radky / 2; }
                else { pulka_radky = (radky / 2) + 1; }
                if ((sloupce % 2) == 0) { pulka_sloupce = sloupce / 2; }
                else { pulka_sloupce = (sloupce / 2) + 1; }

                Matrix<T> upravit; // Matice upravit má stejný počet řádků jako matice a 2x větší počet sloupců
                upravit = new Matrix<T>(radky, sloupce * 2);

                for (int i = 0; i < radky; i++) // Zapíše do matice upravit původní matici
                {
                    for (int j = 0; j < sloupce; j++)
                    {
                        upravit.WriteNumber(i, j, matice.GetNumber(i, j));
                    }
                }
                T jedna = new T();
                jedna.AddInt(1);
                T nula = new T();
                for (int i = 0; i < radky; i++) // Zapíše do matice upravit jednotkovou matici
                {
                    for (int j = sloupce; j < (sloupce * 2); j++)
                    {
                        if (i == (j - sloupce)) { upravit.WriteNumber(i, j, jedna); }
                        else { upravit.WriteNumber(i, j, nula); }
                    }
                }

                upravit = AlteringOperations.Gauss(upravit); // První Gaussovka

                // Převrácení a druhá Gaussovka:

                for (int i = 0; i < pulka_radky; i++) // Převrácení
                {
                    for (int j = 0; j < sloupce; j++)
                    {
                        if ((radky % 2) == 1 && pulka_sloupce == j && (pulka_radky - 1) == i) { break; }
                        upravit.SwapElements(i, j, radky - i - 1, sloupce - j - 1);

                        // Převrácení původně jednotkové matice:
                        upravit.SwapElements(i, sloupce + j, radky - i - 1, (sloupce * 2) - j - 1);
                    }
                }

                upravit = AlteringOperations.Gauss(upravit); // Druhá Gaussovka

                for (int i = 0; i < radky; i++) // Převrácení a složení výsledné matice
                {
                    for (int j = 0; j < sloupce; j++)
                    {
                        vysledek.WriteNumber(radky - i - 1, sloupce - j - 1, upravit.GetNumber(i, j + sloupce));
                    }
                }
            }
            else
            {
                throw new MatrixLibraryException("Given matrix is not regular");
            }
            return vysledek;
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
                    T tmp = Vypocty.Determinant(modified);
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

            Parallel.ForEach(result.GetRowChunks(), (pair) =>
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

                        result.WriteNumber(i, j, (T)(multiply * Vypocty.Determinant(modified)));
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
