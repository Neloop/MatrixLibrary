﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLibrary
{
    /// <summary>
    /// Namespace of parallel extension methods which unite actions about determinants and solving linear equations.
    /// </summary>
    public static class ParallelComputationsExtensions
    {
        /// <summary>
        /// Calculate determinant on given <paramref name="matrix"/> object and returns result as number.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Source container of elements which will be calculated into determinant.</param>
        /// <returns>Class or strucct which fulfill IMatrixNumber interface and represents elements stored in Matrix.</returns>
        public static T DeterminantParallel<T>(this Matrix<T> matrix) where T : IMatrixNumber, new()
        {
            return DeterminantInternalParallel(matrix);
        }

        /// <summary>
        /// Internal method to compute determinant of <paramref name="matrix"/> in which some particular column can be replaced with another.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Matrix on which determinant will be calculated.</param>
        /// <param name="replace">True if column <paramref name="b"/> will be replace on index <paramref name="col"/> in <paramref name="matrix"/>.</param>
        /// <param name="col">Index of column to replace.</param>
        /// <param name="b">Column which maybe will be moved to <paramref name="matrix"/>.</param>
        /// <returns>Returns determinant from <paramref name="matrix"/> or assembled matrix from <paramref name="matrix"/> and <paramref name="b"/>.</returns>
        internal static T DeterminantInternalParallel<T>(Matrix<T> matrix, bool replace = false, int col = 0, Matrix<T> b = null) where T : IMatrixNumber, new()
        {
            if (matrix == null || (replace == true && b == null)) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            T multiplyResult = new T();
            multiplyResult.AddInt(1);
            object multiplyResultLock = new object();

            Matrix<T> modified;
            if (replace == false) { modified = new Matrix<T>(matrix); }
            else { modified = new Matrix<T>(matrix, col, b); }
            int rows = modified.Rows;
            int cols = modified.Cols;

            for (int i = 0; i < rows; ++i)
            {
                for (int j = i; j < cols; ++j)
                {
                    if (modified.GetNumber(i, j).IsZero()) // na miste kde mel byt pivot je nula, zkusi se najit nenulove cislo ve stejnem sloupci
                    {
                        // pokud je prvek nula, tak se koukne pod něj a případně prohodí řádek a vydělí řádky pod ním (potom se breakne), 
                        // pokud i pod ním jsou nuly, pak se breakne (nemusí prostě se nechá doběhnout) cyklus a jde na další sloupec
                        bool end = false;
                        for (int k = i + 1; k < rows; k++)
                        {
                            if (!modified.GetNumber(k, j).IsZero())
                            {
                                for (int l = j; l < cols; l++) // cyklus ktery vymeni prvky na dvou radcich
                                {
                                    modified.SwapElements(k, l, i, l);
                                }
                                multiplyResult = (T)(multiplyResult.__Negate());

                                T divide = modified.GetNumber(i, j); // cislo kterym se bude delit cely radek
                                multiplyResult = (T)(divide.__Multiplication(multiplyResult));
                                Parallel.ForEach(modified.GetColsChunks(j), (pair) =>
                                {
                                    for (int l = pair.Item1; l < pair.Item2; l++) // radek ktery byl posunut nahoru bude vydelen tak aby pivot byl 1
                                    {
                                        modified.WriteNumber(i, l, (T)(modified.GetNumber(i, l).__Division(divide)));
                                    }
                                });

                                Parallel.ForEach(modified.GetRowsChunks(i + 1), (pair) =>
                                {
                                    for (int a = pair.Item1; a < pair.Item2; a++) // vynuluje sloupce pod aktualnim sloupcem j
                                    {
                                        T tmpDivide = modified.GetNumber(a, j);
                                        if (!tmpDivide.IsZero())
                                        {
                                            lock (multiplyResultLock) { multiplyResult = (T)(tmpDivide.__Multiplication(multiplyResult)); }

                                            for (int ab = j; ab < cols; ab++)
                                            {
                                                T tmp = (T)(modified.GetNumber(a, ab).__Division(tmpDivide));
                                                tmp = (T)(tmp.__Subtraction(modified.GetNumber(i, ab)));
                                                modified.WriteNumber(a, ab, tmp);
                                            }
                                        }
                                    }
                                });
                                end = true;
                                break;
                            }

                            if (k == (rows - 1)) { return new T(); }
                        }

                        if (end == true) { break; }

                        // pouzije se v pripade, ze posledni sloupec je nulovy, v jinych pripadech by se sem nikdy nemelo dojit
                        if (i == (rows - 1)) { return new T(); }
                    }
                    else // na miste pivotu je nenulove cislo, tudiz se zmeni na jednicku a vynuluji se sloupce pod nim
                    {
                        T divide = modified.GetNumber(i, j);
                        multiplyResult = (T)(divide.__Multiplication(multiplyResult));
                        Parallel.ForEach(modified.GetColsChunks(j), (pair) =>
                        {
                            for (int k = pair.Item1; k < pair.Item2; k++) // vydeli aktualni radek cislem na zacatku tak, aby byl pivot 1
                            {
                                modified.WriteNumber(i, k, (T)(modified.GetNumber(i, k).__Division(divide)));
                            }
                        });

                        Parallel.ForEach(modified.GetRowsChunks(i + 1), (pair) =>
                        {
                            for (int k = pair.Item1; k < pair.Item2; k++) // tento cyklus vynuluje sloupce pod sloupcem j
                            {
                                T tmpDivide = modified.GetNumber(k, j);
                                if (!tmpDivide.IsZero())
                                {
                                    multiplyResult = (T)(tmpDivide.__Multiplication(multiplyResult));

                                    for (int l = j; l < cols; l++)
                                    {
                                        T tmp = (T)(modified.GetNumber(k, l).__Division(tmpDivide));
                                        tmp = (T)(tmp.__Subtraction(modified.GetNumber(i, l)));
                                        modified.WriteNumber(k, l, tmp);
                                    }
                                }
                            }
                        });
                        break;
                    }
                }
            }


            T result = new T();
            result.AddInt(1);

            result = (T)(multiplyResult.__Multiplication(result));

            return result;
        }

        /// <summary>
        /// Solve linear equations system through Cramer algorithm which uses determinants. Input <paramref name="matrix"/> has to be regular or zero is returned.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Left side of linear equations.</param>
        /// <param name="b">Right side of linear equations.</param>
        /// <returns>Actually its a single column vector of individual results.</returns>
        public static Matrix<T> CramerParallel<T>(this Matrix<T> matrix, Matrix<T> b) where T : IMatrixNumber, new()
        {
            if (matrix == null || b == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            if (b.Cols != 1) { throw new MatrixLibraryException("b is not vector which does not have one column!"); }
            if (b.Rows != matrix.Rows) { throw new MatrixLibraryException("Given matrix and vector b do not have same number of rows!"); }

            Matrix<T> result = Matrix<T>.GetUninitializedMatrix(matrix.Cols, 1);
            T determinant = ParallelComputationsExtensions.DeterminantParallel(matrix);

            Parallel.ForEach(result.GetRowsChunks(), (pair) =>
            {
                T tmpDeterminant = (T)determinant.Copy();

                for (int i = pair.Item1; i < pair.Item2; i++)
                {
                    T x = ComputationsExtensions.DeterminantInternal(matrix, true, i, b);
                    x = (T)(x.__Division(tmpDeterminant));

                    result.WriteNumber(i, 0, x);
                }
            });

            return result;
        }

        /// <summary>
        /// Solve linear equations system on given <paramref name="matrix"/> and right side <paramref name="b"/>. If given <paramref name="matrix"/> is not regular than result is parametrised.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Left side of linear equations system.</param>
        /// <param name="b">Right side of linear equations system.</param>
        /// <returns>Returns Matrix of column vectors, first one is particular part and another are general parts, where one column is one parameter.</returns>
        public static Matrix<T> SolveLinearEquationsParallel<T>(this Matrix<T> matrix, Matrix<T> b) where T : IMatrixNumber, new()
        {
            /*
             * 
             * Result is in form: x = (x1,x2,x3,...) + [t*(t1,t2,t3,...) + s*(s1,s2,s3,...) + ...]
             *  - vectors are in result matrix in columns
             * 
             */

            if (matrix == null || b == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result;
            int rows = matrix.Rows;
            int cols = matrix.Cols;
            int halfOfRows = matrix.GetHalfOfRows();
            int halfOfCols = matrix.GetHalfOfCols();

            Matrix<T> tmpMatrix = Matrix<T>.GetUninitializedMatrix(rows, cols + 1);

            Parallel.ForEach(tmpMatrix.GetRowsChunks(), (pair) =>
            {
                for (int i = pair.Item1; i < pair.Item2; i++) // Naplnění matice 'uprav' maticí 'matice' a sloupcem 'b'
                {
                    tmpMatrix.WriteNumber(i, cols, b.GetNumber(i, 0));
                    for (int j = 0; j < cols; j++)
                    {
                        tmpMatrix.WriteNumber(i, j, matrix.GetNumber(i, j));
                    }
                }
            });

            tmpMatrix = ParallelAlteringOperationsExtensions.GaussParallel(tmpMatrix); // První gaussovka

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
            object zeroRowsLock = new object();
            Parallel.ForEach(tmpMatrix.GetHalfRowsChunks(), (pair) =>
            {
                for (int i = pair.Item1; i < pair.Item2; i++) // Vymění se prvky v matici 'uprav'
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
                    if (zeroesInRow2 == cols)
                    {
                        lock (zeroRowsLock) { zeroRows++; }

                        if (!tmpMatrix.GetNumber(i, cols).IsZero())
                        {
                            throw new MatrixLibraryException("Found row that contain all zeroes but vector b contains on the same row non-zero!");
                        }
                    }
                    if (zeroesInRow1 == cols)
                    {
                        lock (zeroRowsLock) { zeroRows++; }

                        if (!tmpMatrix.GetNumber(rows - i - 1, cols).IsZero())
                        {
                            throw new MatrixLibraryException("Found row that contain all zeroes but vector b contains on the same row non-zero!");
                        }
                    }
                }
            });

            if ((rows - zeroRows) == cols) // Matice má jedno možné řešení
            {
                tmpMatrix = ParallelAlteringOperationsExtensions.GaussParallel(tmpMatrix);
                result = Matrix<T>.GetUninitializedMatrix(rows, 1);
                Parallel.ForEach(result.GetRowsChunks(), (pair) =>
                {
                    for (int i = pair.Item1; i < pair.Item2; i++)
                    {
                        result.WriteNumber(i, 0, tmpMatrix.GetNumber(rows - i - 1, cols));
                    }
                });
            }
            else
            {
                result = new Matrix<T>(cols, zeroRows + 1);
                List<int> parameters = new List<int>();
                object parametersLock = new object();
                Matrix<T> parametrise = Matrix<T>.GetUninitializedMatrix(rows - zeroRows, cols + 1);
                int add = 0;
                Parallel.ForEach(parametrise.GetRowsChunks(), (pair) =>
                {
                    for (int i = pair.Item1; i < pair.Item2; i++) // Zapsání a zpřeházení z matice 'uprav' do matice 'vyparametrizuj'
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
                                lock (parametersLock) { parameters.Add(j); }
                                add++;
                            }
                            else { break; }
                        }
                    }
                });
                
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
                                    T temp = (T)(result.GetNumber(k, l).__Multiplication(parametrise.GetNumber(i, k).__Negate()));
                                    temp = (T)(temp.__Addition(result.GetNumber(j, l)));
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

    /// <summary>
    /// Namespace of extension methods which unite actions about determinants and solving linear equations.
    /// </summary>
    public static class ComputationsExtensions
    {
        /// <summary>
        /// Calculate determinant on given <paramref name="matrix"/> object and returns result as number.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Source container of elements which will be calculated into determinant.</param>
        /// <returns>Class or strucct which fulfill IMatrixNumber interface and represents elements stored in Matrix.</returns>
        public static T Determinant<T>(this Matrix<T> matrix) where T : IMatrixNumber, new()
        {
            return DeterminantInternal(matrix);
        }

        /// <summary>
        /// Internal method to compute determinant of <paramref name="matrix"/> in which some particular column can be replaced with another.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Matrix on which determinant will be calculated.</param>
        /// <param name="replace">True if column <paramref name="b"/> will be replace on index <paramref name="col"/> in <paramref name="matrix"/>.</param>
        /// <param name="col">Index of column to replace.</param>
        /// <param name="b">Column which maybe will be moved to <paramref name="matrix"/>.</param>
        /// <returns>Returns determinant from <paramref name="matrix"/> or assembled matrix from <paramref name="matrix"/> and <paramref name="b"/>.</returns>
        internal static T DeterminantInternal<T>(Matrix<T> matrix, bool replace = false, int col = 0, Matrix<T> b = null) where T : IMatrixNumber, new()
        {
            if (matrix == null || (replace == true && b == null)) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            T multiplyResult = new T();
            multiplyResult.AddInt(1);

            Matrix<T> modified;
            if (replace == false) { modified = new Matrix<T>(matrix); }
            else { modified = new Matrix<T>(matrix, col, b); }
            int rows = modified.Rows;
            int cols = modified.Cols;

            for (int i = 0; i < rows; ++i)
            {
                for (int j = i; j < cols; ++j)
                {
                    if (modified.GetNumber(i, j).IsZero()) // na miste kde mel byt pivot je nula, zkusi se najit nenulove cislo ve stejnem sloupci
                    {
                        // pokud je prvek nula, tak se koukne pod něj a případně prohodí řádek a vydělí řádky pod ním (potom se breakne), 
                        // pokud i pod ním jsou nuly, pak se breakne (nemusí prostě se nechá doběhnout) cyklus a jde na další sloupec
                        bool end = false;
                        for (int k = i + 1; k < rows; k++)
                        {
                            if (!modified.GetNumber(k, j).IsZero())
                            {
                                for (int l = j; l < cols; l++) // cyklus ktery vymeni prvky na dvou radcich
                                {
                                    modified.SwapElements(k, l, i, l);
                                }
                                multiplyResult = (T)(multiplyResult.__Negate());

                                T divide = modified.GetNumber(i, j); // cislo kterym se bude delit cely radek
                                multiplyResult = (T)(divide.__Multiplication(multiplyResult));
                                for (int l = j; l < cols; l++) // radek ktery byl posunut nahoru bude vydelen tak aby pivot byl 1
                                {
                                    modified.WriteNumber(i, l, (T)(modified.GetNumber(i, l).__Division(divide)));
                                }

                                for (int a = i + 1; a < modified.Rows; a++) // vynuluje sloupce pod aktualnim sloupcem j
                                {
                                    divide = modified.GetNumber(a, j);
                                    if (!divide.IsZero())
                                    {
                                        multiplyResult = (T)(divide.__Multiplication(multiplyResult));

                                        for (int ab = j; ab < cols; ab++)
                                        {
                                            T tmp = (T)(modified.GetNumber(a, ab).__Division(divide));
                                            tmp = (T)(tmp.__Subtraction(modified.GetNumber(i, ab)));
                                            modified.WriteNumber(a, ab, tmp);
                                        }
                                    }
                                }
                                end = true;
                                break;
                            }

                            if (k == (rows - 1)) { return new T(); }
                        }

                        if (end == true) { break; }

                        // pouzije se v pripade, ze posledni sloupec je nulovy, v jinych pripadech by se sem nikdy nemelo dojit
                        if (i == (rows - 1)) { return new T(); }
                    }
                    else // na miste pivotu je nenulove cislo, tudiz se zmeni na jednicku a vynuluji se sloupce pod nim
                    {
                        T divide = modified.GetNumber(i, j);
                        multiplyResult = (T)(divide.__Multiplication(multiplyResult));
                        for (int k = j; k < cols; k++) // vydeli aktualni radek cislem na zacatku tak, aby byl pivot 1
                        {
                            modified.WriteNumber(i, k, (T)(modified.GetNumber(i, k).__Division(divide)));
                        }

                        for (int k = i + 1; k < rows; k++) // tento cyklus vynuluje sloupce pod sloupcem j
                        {
                            divide = modified.GetNumber(k, j);
                            if (!divide.IsZero())
                            {
                                multiplyResult = (T)(divide.__Multiplication(multiplyResult));

                                for (int l = j; l < cols; l++)
                                {
                                    T tmp = (T)(modified.GetNumber(k, l).__Division(divide));
                                    tmp = (T)(tmp.__Subtraction(modified.GetNumber(i, l)));
                                    modified.WriteNumber(k, l, tmp);
                                }
                            }
                        }
                        break;
                    }
                }
            }

            T result = new T();
            result.AddInt(1);
            /*for (int i = 0; i < rows; i++) // Vynásobí prvky na diagonále (meli by byt vsechno jednicky, takze je tato operace vicemene zbytecna)
            {
                result = (T)(modified.GetNumber(i, i) * result);
            }*/

            result = (T)(multiplyResult.__Multiplication(result));

            return result;
        }

        /// <summary>
        /// Solve linear equations system through Cramer algorithm which uses determinants. Input <paramref name="matrix"/> has to be regular or zero is returned.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Left side of linear equations.</param>
        /// <param name="b">Right side of linear equations.</param>
        /// <returns>Actually its a single column vector of individual results.</returns>
        public static Matrix<T> Cramer<T>(this Matrix<T> matrix, Matrix<T> b) where T : IMatrixNumber, new()
        {
            if (matrix == null || b == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            int rows = matrix.Rows;
            int cols = matrix.Cols;

            if (b.Cols != 1) { throw new MatrixLibraryException("b is not vector which does not have one column!"); }
            if (b.Rows != matrix.Rows) { throw new MatrixLibraryException("Given matrix and vector b do not have same number of rows!"); }

            Matrix<T> result = Matrix<T>.GetUninitializedMatrix(cols, 1);

            T determinant = ComputationsExtensions.Determinant(matrix);

            for (int i = 0; i < cols; i++)
            {
                T x = ComputationsExtensions.DeterminantInternal(matrix, true, i, b);
                x = (T)(x.__Division(determinant));

                result.WriteNumber(i, 0, x);
            }

            return result;
        }

        /// <summary>
        /// Solve linear equations system on given <paramref name="matrix"/> and right side <paramref name="b"/>. If given <paramref name="matrix"/> is not regular than result is parametrised.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Left side of linear equations system.</param>
        /// <param name="b">Right side of linear equations system.</param>
        /// <returns>Returns Matrix of column vectors, first one is particular part and another are general parts, where one column is one parameter.</returns>
        public static Matrix<T> SolveLinearEquations<T>(this Matrix<T> matrix, Matrix<T> b) where T : IMatrixNumber, new()
        {
            /*
             * 
             * Výsledek je ve tvaru: x = (x1,x2,x3,...) + [t*(t1,t2,t3,...) + s*(s1,s2,s3,...) + ...]
             *  - vektory jsou ve výsledné matici sloupce
             * 
             */

            if (matrix == null || b == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result;
            int rows = matrix.Rows;
            int cols = matrix.Cols;
            int halfOfRows = matrix.GetHalfOfRows();
            int halfOfCols = matrix.GetHalfOfCols();

            Matrix<T> tmpMatrix = Matrix<T>.GetUninitializedMatrix(rows, cols + 1);

            for (int i = 0; i < rows; i++) // Naplnění matice 'uprav' maticí 'matice' a sloupcem 'b'
            {
                tmpMatrix.WriteNumber(i, cols, b.GetNumber(i, 0));
                for (int j = 0; j < cols; j++)
                {
                    tmpMatrix.WriteNumber(i, j, matrix.GetNumber(i, j));
                }
            }

            tmpMatrix = AlteringOperationsExtensions.Gauss(tmpMatrix); // První gaussovka

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
                if (zeroesInRow2 == cols)
                {
                    zeroRows++;

                    if (!tmpMatrix.GetNumber(i, cols).IsZero())
                    {
                        throw new MatrixLibraryException("Found row that contain all zeroes but vector b contains on the same row non-zero!");
                    }
                }
                if (zeroesInRow1 == cols)
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
                tmpMatrix = AlteringOperationsExtensions.Gauss(tmpMatrix);
                result = Matrix<T>.GetUninitializedMatrix(rows, 1);
                for (int i = 0; i < rows; i++)
                {
                    result.WriteNumber(i, 0, tmpMatrix.GetNumber(rows - i - 1, cols));
                }
            }
            else
            {
                result = new Matrix<T>(cols, zeroRows + 1);
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
                                    T temp = (T)(result.GetNumber(k, l).__Multiplication(parametrise.GetNumber(i, k).__Negate()));
                                    temp = (T)(temp.__Addition(result.GetNumber(j, l)));
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
