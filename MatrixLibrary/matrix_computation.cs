using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLibrary
{
    public static class Computations
    {
        public static T Determinant<T>(Matrix<T> matrix) where T : MatrixNumberBase, new() // Pokud matice není regulární je vracena 0
        {
            T multiplyResult = new T();
            multiplyResult.AddInt(1);

            int rows = matrix.Rows;
            int cols = matrix.Cols;
            Matrix<T> modified = new Matrix<T>(matrix);

            int add = 0;

            for (int i = 0; i < rows; i++) // První cyklus projde všechny řádky a od každého spustí další cyklus na vynulování sloupce pod pivotem
            {
                bool pivot = true;
                for (int j = i + add; j < rows; j++)
                {
                    if (pivot == true) // Pivot na každém řádku se změní na jedničku
                    {
                        pivot = false;
                        T divide = (T)modified.GetNumber(i, j).Copy();

                        if (modified.GetNumber(i, j).IsZero())
                        {
                            bool endIt = false;
                            for (int l = j; l < cols; l++)
                            {
                                for (int k = i + 1; k < rows; k++)
                                {
                                    if (!modified.GetNumber(k, l).IsZero() && modified.GetNumber(i, l).IsZero()) // Vymění dva řádky, aby ten s méně nenulovými sloupci byl na vrchu
                                    {
                                        for (int s = l; s < cols; s++)
                                        {
                                            modified.SwapElements(k, s, i, s);
                                        }

                                        multiplyResult = (T)(-multiplyResult);

                                        add = add + (l - j);
                                        j = j + add;
                                        endIt = true;
                                        break;
                                    }
                                }
                                if (endIt == true) // Pokud se povedlo vyměnit dva řádky
                                {
                                    divide = (T)modified.GetNumber(i, j).Copy();
                                    multiplyResult = (T)(divide * multiplyResult);
                                    break;
                                }
                                if (modified.GetNumber(i, l).IsZero()) // Pouze v případě, že byl celý sloupec nulový, tak se přičte 1 ke zpracovávaným indexům sloupců
                                {
                                    add++;
                                    j = j + add;
                                    if (add >= modified.Cols) { return new T(); }
                                }
                            }
                        }

                        multiplyResult = (T)(divide * multiplyResult); // Číslo, kterým se bude determinant ve výsledku násobit

                        for (int k = i + add; k < cols; k++)
                        {
                            modified.WriteNumber(i, k, (T)(modified.GetNumber(i, k) / divide));
                        }
                    }
                    else
                    {
                        T divide = (T)modified.GetNumber(j, i + add).Copy();

                        if (divide.IsZero()) // pokud je už prvek vynulován, pokračuje se na dalším řádku
                        {
                            continue;
                        }

                        multiplyResult = (T)(divide * multiplyResult);

                        for (int k = i + add; k < cols; k++)
                        {
                            modified.WriteNumber(j, k, (T)(modified.GetNumber(j, k) / divide));
                        }
                        for (int l = i + add; l < cols; l++)
                        {
                            T tmp = (T)(modified.GetNumber(j, l) - modified.GetNumber(i, l));
                            modified.WriteNumber(j, l, tmp);
                        }
                    }
                }
            }

            T result = new T();
            result.AddInt(1);
            for (int i = 0; i < rows; i++) // Vynásobí prvky na diagonále
            {
                result = (T)(modified.GetNumber(i, i) * result);
            }

            result = (T)(multiplyResult * result);

            return result;
        }

        public static Matrix<T> Cramer<T>(Matrix<T> matrix, Matrix<T> b) where T : MatrixNumberBase, new() // Vrací vlastně vektor n*1; vstupem musí být regulární matice
        {
            int rows = matrix.Rows;
            int cols = matrix.Cols;

            if (b.Cols != 1) { throw new MatrixLibraryException("b is not vector which does not have one column!"); }
            if (b.Rows != matrix.Rows) { throw new MatrixLibraryException("Given matrix and vector b do not have same number of rows!"); }

            Matrix<T> result = Matrix<T>.GetUninitializedMatrix(cols, 1);

            Matrix<T> det = Matrix<T>.GetUninitializedMatrix(rows, cols);
            T determinant = Computations.Determinant(matrix);

            for (int i = 0; i < cols; i++)
            {
                for (int k = 0; k < rows; k++) // sestavení matice, kde je i-tý sloupec nahrazen sloupcem b
                {
                    for (int l = 0; l < cols; l++)
                    {
                        if (i == l)
                        {
                            det.WriteNumber(k, l, b.GetNumber(k, 0));
                        }
                        else
                        {
                            det.WriteNumber(k, l, matrix.GetNumber(k, l));
                        }
                    }
                }

                T x = Computations.Determinant(det);
                x = (T)(x / determinant);

                result.WriteNumber(i, 0, x);
            }

            return result;
        }

        public static Matrix<T> Cramer_MultiThreaded<T>(Matrix<T> matrix, Matrix<T> b) where T : MatrixNumberBase, new() // Vrací vlastně vektor n*1; vstupem musí být regulární matice
        {
            /* sice pocita determinanty paralelne, ale kvuli tomu potrebuje mit pro kazdou iteraci docasne vytvorenou stejne velkou matici jako je vstupni,
             * diky tomu je to velmi pametove narocne 
             * (dokud nebude napsan specialni Determinant(), ktery bude umet nahrazovat sloupec v matici bez vytvoreni pomocne) */

            int rows = matrix.Rows;
            int cols = matrix.Cols;

            if (b.Cols != 1) { throw new MatrixLibraryException("b is not vector which does not have one column!"); }
            if (b.Rows != matrix.Rows) { throw new MatrixLibraryException("Given matrix and vector b do not have same number of rows!"); }

            Matrix<T> result = Matrix<T>.GetUninitializedMatrix(cols, 1);
            T determinant = Computations.Determinant(matrix);

            Parallel.ForEach(result.GetRowChunks(), (pair) =>
            {
                for (int i = pair.Item1; i < pair.Item2; i++)
                {
                    Matrix<T> det = Matrix<T>.GetUninitializedMatrix(rows, cols);

                    for (int k = 0; k < rows; k++) // sestavení matice, kde je i-tý sloupec nahrazen sloupcem b
                    {
                        for (int l = 0; l < cols; l++)
                        {
                            if (i == l)
                            {
                                det.WriteNumber(k, l, b.GetNumber(k, 0));
                            }
                            else
                            {
                                det.WriteNumber(k, l, matrix.GetNumber(k, l));
                            }
                        }
                    }

                    T x = Computations.Determinant(det);
                    x = (T)(x / determinant);

                    result.WriteNumber(i, 0, x);
                }
            });

            return result;
        }

        public static Matrix<T> SolveLinearEquations<T>(Matrix<T> matrix, Matrix<T> b) where T : MatrixNumberBase, new() // Vrací sloupcové vektory: první je partikulární část, další jsou obecné části (jeden sloupec = jeden parametr)
        {
            /*
             * 
             * Výsledek je ve tvaru: x = (x1,x2,x3,...) + [t*(t1,t2,t3,...) + s*(s1,s2,s3,...) + ...]
             *  - vektory jsou ve výsledné matici sloupce
             * 
             */

            Matrix<T> result;
            int rows = matrix.Rows;
            int cols = matrix.Cols;
            int halfOfRows;
            int halfOfCols;
            if ((rows % 2) == 0) { halfOfRows = rows / 2; }
            else { halfOfRows = (rows / 2) + 1; }
            if ((cols % 2) == 0) { halfOfCols = cols / 2; }
            else { halfOfCols = (cols / 2) + 1; }

            Matrix<T> tmpMatrix = Matrix<T>.GetUninitializedMatrix(rows, cols + 1);

            for (int i = 0; i < rows; i++) // Naplnění matice 'uprav' maticí 'matice' a sloupcem 'b'
            {
                tmpMatrix.WriteNumber(i, cols, b.GetNumber(i, 0));
                for (int j = 0; j < cols; j++)
                {
                    tmpMatrix.WriteNumber(i, j, matrix.GetNumber(i, j));
                }
            }

            tmpMatrix = AlteringOperations.Gauss(tmpMatrix); // První gaussovka

            /*
             * 
             * Následuje projití matice 'uprav' jestli v ní nejsou nulové řádky a vyměnění prvků pro druhou gaussovku
             *  a pak podmínka která se rozděluje:
             *   - Pokud matice není regulární nebo má nulové řádky
             *      > potom se neprovádí druhá gaussovka a rovnou se sestavuje řešení
             *   - Matice má stejný počet řádků a sloupců
             *      > matice 'uprav' se projede druhou gaussovkou a tím se získá řešení
             * 
             */

            int zeroRows = 0;
            for (int i = 0; i < halfOfRows; i++) // Vymění se prvky v matici 'uprav'
            {
                int zeroesInRow1 = 0;
                int zeroesInRow2 = 0;

                tmpMatrix.SwapElements(i, cols, rows - i - 1, cols);

                for (int j = 0; j < cols; j++)
                {
                    if (tmpMatrix.GetNumber(i, j).IsZero()) { zeroesInRow1++; }
                    if (tmpMatrix.GetNumber(rows - i - 1, cols - j - 1).IsZero()) { zeroesInRow2++; }
                    if ((rows % 2) == 1 && halfOfCols == j && (halfOfRows - 1) == i) { break; }

                    tmpMatrix.SwapElements(i, j, rows - i - 1, cols - j - 1);
                }
                if (zeroesInRow1 == cols)
                {
                    zeroRows++;

                    if (!tmpMatrix.GetNumber(i, cols).IsZero())
                    {
                        throw new MatrixLibraryException("Found row that contain all zeroes but vector b contains on the same row non-zero!");
                    }
                }
                if (zeroesInRow2 == cols)
                {
                    zeroRows++;

                    if (!tmpMatrix.GetNumber(rows - i - 1, cols).IsZero())
                    {
                        throw new MatrixLibraryException("Found row that contain all zeroes but vector b contains on the same row non-zero!");
                    }
                }
            }

            if ((rows - zeroRows) == cols) // Matice má jedno možné řešení
            {
                tmpMatrix = AlteringOperations.Gauss(tmpMatrix);
                result = new Matrix<T>(rows, 1);
                for (int i = 0; i < rows; i++)
                {
                    result.WriteNumber(i, 0, tmpMatrix.GetNumber(rows - i - 1, cols));
                }
            }
            else
            {
                result = new Matrix<T>(rows, zeroRows + 1);
                List<int> parameters = new List<int>();
                Matrix<T> parametrise = Matrix<T>.GetUninitializedMatrix(rows - zeroRows, cols + 1);
                int add = 0;
                for (int i = 0; i < parametrise.Rows; i++) // Zapsání a zpřeházení z matice 'uprav' do matice 'vyparametrizuj'
                {
                    parametrise.WriteNumber(i, cols, tmpMatrix.GetNumber(rows - i - 1, cols));

                    for (int j = 0; j < cols; j++)
                    {
                        parametrise.WriteNumber(i, j, tmpMatrix.GetNumber(rows - i - 1, cols - j - 1));
                    }
                    for (int j = i + add; j < cols; j++) // Určení, co budou parametry
                    {
                        if (!parametrise.GetNumber(i, j).IsOne())
                        {
                            parameters.Add(j);
                            add++;
                        }
                        else { break; }
                    }
                }
                for (int i = parametrise.Rows + parameters.Count; i < (parametrise.Cols - 1); i++) // Dopsání zbývající parametrů
                {
                    parameters.Add(i);
                }

                for (int i = 0; i < parameters.Count; i++) // Zapsání parametrů do výsledku
                {
                    T one = new T();
                    one.AddInt(1);
                    result.WriteNumber(parameters[i], i + 1, one);
                }

                for (int i = (parametrise.Rows - 1); i >= 0; i--)
                {
                    for (int j = 0; j < parametrise.Cols; j++)
                    {
                        if (parametrise.GetNumber(i, j).IsOne())
                        {
                            result.WriteNumber(j, 0, parametrise.GetNumber(i, parametrise.Cols - 1));
                            for (int k = (j + 1); k < (parametrise.Cols - 1); k++) // Jde po prvcích v matici vyparametrizuj
                            {
                                for (int l = 0; l < result.Cols; l++) // Dosazuje z už vypočítaných výsledků
                                {
                                    T temp = (T)(result.GetNumber(k, l) * (-parametrise.GetNumber(j, k)));
                                    temp = (T)(temp + result.GetNumber(j, l));
                                    result.WriteNumber(j, l, temp);
                                }
                            }
                            break;
                        }
                    }
                }
            }

            return result;
        }
    }
}
