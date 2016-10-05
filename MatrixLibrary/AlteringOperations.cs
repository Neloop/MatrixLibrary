using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLibrary
{
    /// <summary>
    /// Technically namespace of extension methods. All of them are parallel and unify operations which returns somehow altered given matrixes.
    /// </summary>
    public static class ParallelAlteringOperationsExtensions
    {
        /// <summary>
        /// Execute parallel transposition on given <paramref name="matrix"/> and returns result.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Matrix which elements will be transposed.</param>
        /// <returns>Newly created transposed Matrix.</returns>
        public static Matrix<T> TranspositionParallel<T>(this Matrix<T> matrix) where T : IMatrixNumber, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result;
            int rows = matrix.Rows;
            result = Matrix<T>.GetUninitializedMatrix(matrix.Cols, rows);

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

        /// <summary>
        /// Simultaneously symetrize given <paramref name="matrix"/> and returns the result.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Matrix to be symetrized.</param>
        /// <returns>New instance of Matrix class.</returns>
        public static Matrix<T> SymmetricParallel<T>(this Matrix<T> matrix) where T : IMatrixNumber, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result;
            if (matrix.Rows == matrix.Cols)
            {
                int dim = matrix.Rows;

                Matrix<T> transpose = ParallelAlteringOperationsExtensions.TranspositionParallel(matrix);
                result = ParallelClassicOperations.AdditionParallel(matrix, transpose);

                T two = (T)new T().AddInt(2);
                Parallel.ForEach(result.GetRowsChunks(), (pair) =>
                {
                    for (int i = pair.Item1; i < pair.Item2; i++) // Divide all numbers with number two
                    {
                        for (int j = 0; j < dim; j++)
                        {
                            result.WriteNumber(i, j, (T)(result.GetNumber(i, j).__Division(two)));
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

        /// <summary>
        /// Performs Gauss elimination, with some parallel parts, on given <paramref name="matrix"/>.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Matrix which will be source for Gauss elimination.</param>
        /// <returns>Returns result of Gauss elimination.</returns>
        public static Matrix<T> GaussParallel<T>(this Matrix<T> matrix) where T : IMatrixNumber, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

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
                        bool end = false;
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
                                        result.WriteNumber(i, l, (T)(result.GetNumber(i, l).__Division(divide)));
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
                                                T tmp = (T)(result.GetNumber(a, b).__Division(tmpDivide));
                                                tmp = (T)(tmp.__Subtraction(result.GetNumber(i, b)));
                                                result.WriteNumber(a, b, tmp);
                                            }
                                        }
                                    }
                                });
                                end = true;
                                break;
                            }
                        }

                        if (end == true) { break; }
                    }
                    else // in case pivot was encountered
                    {
                        T divide = result.GetNumber(i, j);
                        Parallel.ForEach(result.GetColsChunks(j), (pair) =>
                        {
                            for (int k = pair.Item1; k < pair.Item2; k++)
                            {
                                result.WriteNumber(i, k, (T)(result.GetNumber(i, k).__Division(divide)));
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
                                        T tmp = (T)(result.GetNumber(k, l).__Division(tmpDivide));
                                        tmp = (T)(tmp.__Subtraction(result.GetNumber(i, l)));
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

        /// <summary>
        /// Gauss-Jordan elimination performed on parameter <paramref name="matrix"/>.
        /// </summary>
        /// <remarks>
        /// To compute Gauss-Jordan elimination are used two single Gauss eliminations, one on original matrix and second on "reverse".
        /// </remarks>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Source for Gauss-Jordan elimination.</param>
        /// <returns>Newly created instance of Matrix class.</returns>
        public static Matrix<T> GaussJordanParallel<T>(this Matrix<T> matrix) where T : IMatrixNumber, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result;
            int rows = matrix.Rows;
            int cols = matrix.Cols;

            int halfOfRow;
            int halfOfCol;
            if ((rows % 2) == 0) { halfOfRow = rows / 2; }
            else { halfOfRow = (rows / 2) + 1; }
            if ((cols % 2) == 0) { halfOfCol = cols / 2; }
            else { halfOfCol = (cols / 2) + 1; }

            result = ParallelAlteringOperationsExtensions.GaussParallel(matrix); // first gauss elimination

            bool isZeroRow = false;
            Parallel.ForEach(result.GetHalfRowsChunks(), (pair) =>
            {
                for (int i = pair.Item1; i < pair.Item2; i++) // reallocate elements in matrix and execute "reversed" gauss
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
            if (isZeroRow == false) { result = ParallelAlteringOperationsExtensions.GaussParallel(result); }

            Parallel.ForEach(result.GetHalfRowsChunks(), (pair) =>
            {
                for (int i = pair.Item1; i < pair.Item2; i++) // reallocate matrix elements and return result
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

        /// <summary>
        /// On specified <paramref name="matrix"/> performs inversion and returns result.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Matrix to be inversed.</param>
        /// <returns>Inverse matrix.</returns>
        public static Matrix<T> InverseParallel<T>(this Matrix<T> matrix) where T : IMatrixNumber, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result;
            if (ParallelPropertiesExtensions.IsInvertibleParallel(matrix) == true)
            {
                int rows = matrix.Rows;
                int cols = matrix.Cols;
                result = Matrix<T>.GetUninitializedMatrix(rows, cols);

                int halfOfRow = matrix.GetHalfOfRows();
                int halfOfCol = matrix.GetHalfOfCols();

                Matrix<T> modify = Matrix<T>.GetUninitializedMatrix(rows, cols * 2);

                Parallel.ForEach(modify.GetRowsChunks(), (pair) =>
                {
                    for (int i = pair.Item1; i < pair.Item2; i++) // write original (given) matrix to the temporary one
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
                    for (int i = pair.Item1; i < pair.Item2; i++) // write identity matrix
                    {
                        for (int j = cols; j < (cols * 2); j++)
                        {
                            if (i == (j - cols)) { modify.WriteNumber(i, j, one); }
                            else { modify.WriteNumber(i, j, zero); }
                        }
                    }
                });

                modify = ParallelAlteringOperationsExtensions.GaussParallel(modify); // first gauss

                Parallel.ForEach(modify.GetHalfRowsChunks(), (pair) =>
                {
                    for (int i = pair.Item1; i < pair.Item2; i++) // reallocate elements
                    {
                        for (int j = 0; j < cols; j++)
                        {
                            if ((rows % 2) == 1 && halfOfCol == j && (halfOfRow - 1) == i) { break; }
                            modify.SwapElements(i, j, rows - i - 1, cols - j - 1);
                            
                            // reallocate elements of identity matrix part
                            modify.SwapElements(i, cols + j, rows - i - 1, (cols * 2) - j - 1);
                        }
                    }
                });

                modify = ParallelAlteringOperationsExtensions.GaussParallel(modify); // second gauss

                Parallel.ForEach(result.GetRowsChunks(), (pair) =>
                {
                    for (int i = pair.Item1; i < pair.Item2; i++) // reallocate elements and write result
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

        /// <summary>
        /// Calculate adjugate matrix from given <paramref name="matrix"/> and returns it.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Source matrix to this operation.</param>
        /// <returns>Result of adjugation of <paramref name="matrix"/>.</returns>
        public static Matrix<T> AdjugateParallel<T>(this Matrix<T> matrix) where T : IMatrixNumber, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            int rows = matrix.Rows;
            int cols = matrix.Cols;
            Matrix<T> result = Matrix<T>.GetUninitializedMatrix(cols, rows);

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
                            if (k == i) { subtractK = 1; continue; }
                            int subtractL = 0;

                            for (int l = 0; l < cols; l++)
                            {
                                if (l == j) { subtractL = 1; continue; }
                                else
                                {
                                    modified.WriteNumber(k - subtractK, l - subtractL, matrix.GetNumber(k, l));
                                }
                            }
                        }

                        result.WriteNumber(j, i, (T)(multiply.__Multiplication(ComputationsExtensions.Determinant(modified))));
                    }
                }
            });

            return result;
        }

        /// <summary>
        /// Computes orthogonal form of <paramref name="matrix"/> through use of Gram-Schmidt orthogonalization.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Matrix object which will serve as a source for orthogonalization.</param>
        /// <returns>New instance of orthogonal Matrix.</returns>
        public static Matrix<T> OrthogonalParallel<T>(this Matrix<T> matrix) where T : IMatrixNumber, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            int rows = matrix.Rows;
            int cols = matrix.Cols;
            Matrix<T> tmpMatrix = new Matrix<T>(matrix);
            Matrix<T> result = Matrix<T>.GetUninitializedMatrix(rows, rows);

            for (int k = 0; k < cols && k < rows; ++k)
            {
                T norm = new T();
                for (int i = 0; i < rows; ++i)
                {
                    norm = (T)(norm.__Addition(tmpMatrix.GetNumber(i, k).__Exponentiate(2)));
                }
                norm = (T)norm.__SquareRoot();

                Parallel.ForEach(matrix.GetRowsChunks(), (pair) =>
                {
                    for (int i = pair.Item1; i < pair.Item2; ++i)
                    {
                        result.WriteNumber(i, k, (T)(tmpMatrix.GetNumber(i, k).__Division(norm)));
                    }
                });

                Parallel.ForEach(matrix.GetColsChunks(k + 1), (pair) =>
                {
                    for (int j = pair.Item1; j < pair.Item2; ++j)
                    {
                        T dotProduct = new T();
                        for (int i = 0; i < rows; ++i)
                        {
                            dotProduct = (T)(dotProduct.__Addition(tmpMatrix.GetNumber(i, j).__Multiplication(result.GetNumber(i, k))));
                        }

                        for (int i = 0; i < rows; ++i)
                        {
                            tmpMatrix.WriteNumber(i, j, 
                                (T)(tmpMatrix.GetNumber(i, j).__Subtraction(dotProduct.__Multiplication(result.GetNumber(i, k)))));
                        }
                    }
                });
            }

            return result;
        }
    }

    /// <summary>
    /// Namespace of extension methods which are unifying operations returning somehow altered given matrixes
    /// </summary>
    public static class AlteringOperationsExtensions
    {
        /// <summary>
        /// Execute transposition on given <paramref name="matrix"/> and returns result.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Matrix which elements will be transposed.</param>
        /// <returns>Newly created transposed Matrix.</returns>
        public static Matrix<T> Transposition<T>(this Matrix<T> matrix) where T : IMatrixNumber, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result;
            int rows = matrix.Rows;
            int cols = matrix.Cols;
            result = Matrix<T>.GetUninitializedMatrix(cols, rows);

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    result.WriteNumber(i, j, matrix.GetNumber(j, i));
                }
            }
            
            return result;
        }

        /// <summary>
        /// Symetrize given <paramref name="matrix"/> and returns the result.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Matrix to be symetrized.</param>
        /// <returns>New instance of Matrix class.</returns>
        public static Matrix<T> Symmetric<T>(this Matrix<T> matrix) where T : IMatrixNumber, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result;
            if (matrix.Rows == matrix.Cols)
            {
                int dim = matrix.Rows;

                Matrix<T> transpose = AlteringOperationsExtensions.Transposition(matrix);
                result = ClassicOperations.Addition(matrix, transpose);

                T two = (T)new T().AddInt(2);
                for (int i = 0; i < dim; i++) // divide all values with number two
                {
                    for (int j = 0; j < dim; j++)
                    {
                        result.WriteNumber(i, j, (T)(result.GetNumber(i, j).__Division(two)));
                    }
                }
            }
            else
            {
                throw new MatrixLibraryException("Given matrix does not have same number of rows and columns");
            }
            return result;
        }

        /// <summary>
        /// Performs Gauss elimination on given <paramref name="matrix"/>.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Matrix which will be source for Gauss elimination.</param>
        /// <returns>Returns result of Gauss elimination.</returns>
        public static Matrix<T> Gauss<T>(this Matrix<T> matrix) where T : IMatrixNumber, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

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
                        bool end = false;
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
                                    result.WriteNumber(i, l, (T)(result.GetNumber(i, l).__Division(divide)));
                                }

                                for (int a = i + 1; a < rows; a++)
                                {
                                    divide = result.GetNumber(a, j);
                                    if (!divide.IsZero())
                                    {
                                        for (int b = j; b < cols; b++)
                                        {
                                            T tmp = (T)(result.GetNumber(a, b).__Division(divide));
                                            tmp = (T)(tmp.__Subtraction(result.GetNumber(i, b)));
                                            result.WriteNumber(a, b, tmp);
                                        }
                                    }
                                }
                                end = true;
                                break;
                            }
                        }

                        if (end == true) { break; }
                    }
                    else // in case of pivot
                    {
                        T divide = result.GetNumber(i, j);
                        for (int k = j; k < cols; k++)
                        {
                            result.WriteNumber(i, k, (T)(result.GetNumber(i, k).__Division(divide)));
                        }

                        for (int k = i + 1; k < rows; k++)
                        {
                            divide = result.GetNumber(k, j);
                            if (!divide.IsZero())
                            {
                                for (int l = j; l < cols; l++)
                                {
                                    T tmp = (T)(result.GetNumber(k, l).__Division(divide));
                                    tmp = (T)(tmp.__Subtraction(result.GetNumber(i, l)));
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

        /// <summary>
        /// Gauss-Jordan elimination performed on <paramref name="matrix"/> parameter.
        /// </summary>
        /// <remarks>
        /// To compute Gauss-Jordan elimination are used two single Gauss eliminations, one on original matrix and second on "reverse".
        /// </remarks>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Source for Gauss-Jordan elimination.</param>
        /// <returns>Newly created instance of Matrix class.</returns>
        public static Matrix<T> GaussJordan<T>(this Matrix<T> matrix) where T : IMatrixNumber, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result;
            int rows = matrix.Rows;
            int cols = matrix.Cols;

            int halfOfRow;
            int halfOfCol;
            if ((rows % 2) == 0) { halfOfRow = rows / 2; }
            else { halfOfRow = (rows / 2) + 1; }
            if ((cols % 2) == 0) { halfOfCol = cols / 2; }
            else { halfOfCol = (cols / 2) + 1; }

            result = AlteringOperationsExtensions.Gauss(matrix); // first gauss

            int zeroRow1, zeroRow2;
            bool isZeroRow = false;
            for (int i = 0; i < halfOfRow; i++) // reallocate elements in matrix and execute first gauss
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
            if (isZeroRow == false) { result = AlteringOperationsExtensions.Gauss(result); }

            for (int i = 0; i < halfOfRow; i++) // reallocate elements and write result
            {
                for (int j = 0; j < cols; j++)
                {
                    if ((rows % 2) == 1 && halfOfCol == j && (halfOfRow - 1) == i) { break; }

                    result.SwapElements(i, j, rows - i - 1, cols - j - 1);
                }
            }

            return result;
        }

        /// <summary>
        /// On specified <paramref name="matrix"/> performs inversion and returns result.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Matrix to be inversed.</param>
        /// <returns>Inverse matrix.</returns>
        public static Matrix<T> Inverse<T>(this Matrix<T> matrix) where T : IMatrixNumber, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result;
            if (PropertiesExtensions.IsInvertible(matrix) == true)
            {
                int rows = matrix.Rows;
                int cols = matrix.Cols;
                result = Matrix<T>.GetUninitializedMatrix(rows, cols);

                int halfOfRow = matrix.GetHalfOfRows();
                int halfOfCol = matrix.GetHalfOfCols();

                Matrix<T> modify = Matrix<T>.GetUninitializedMatrix(rows, cols * 2);

                for (int i = 0; i < rows; i++) // write original matrix into temporary one
                {
                    for (int j = 0; j < cols; j++)
                    {
                        modify.WriteNumber(i, j, matrix.GetNumber(i, j));
                    }
                }
                T one = new T();
                one.AddInt(1);
                T zero = new T();
                for (int i = 0; i < rows; i++) // write identity matrix
                {
                    for (int j = cols; j < (cols * 2); j++)
                    {
                        if (i == (j - cols)) { modify.WriteNumber(i, j, one); }
                        else { modify.WriteNumber(i, j, zero); }
                    }
                }

                modify = AlteringOperationsExtensions.Gauss(modify); // first gauss

                for (int i = 0; i < halfOfRow; i++) // reallocate elements
                {
                    for (int j = 0; j < cols; j++)
                    {
                        if ((rows % 2) == 1 && halfOfCol == j && (halfOfRow - 1) == i) { break; }
                        modify.SwapElements(i, j, rows - i - 1, cols - j - 1);
                        
                        // reallocate identity matrix part
                        modify.SwapElements(i, cols + j, rows - i - 1, (cols * 2) - j - 1);
                    }
                }

                modify = AlteringOperationsExtensions.Gauss(modify); // second gauss

                for (int i = 0; i < rows; i++) // reallocate and write result
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

        /// <summary>
        /// Calculate adjugate matrix from given <paramref name="matrix"/> and returns it.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Source matrix to this operation.</param>
        /// <returns>Result of adjugation of <paramref name="matrix"/>.</returns>
        public static Matrix<T> Adjugate<T>(this Matrix<T> matrix) where T : IMatrixNumber, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            int rows = matrix.Rows;
            int cols = matrix.Cols;
            Matrix<T> result = Matrix<T>.GetUninitializedMatrix(cols, rows);

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
                        if (k == i) { subtractK = 1; continue; }
                        int subtractL = 0;

                        for (int l = 0; l < cols; l++)
                        {
                            if (l == j) { subtractL = 1; continue; }
                            else
                            {
                                modified.WriteNumber(k - subtractK, l - subtractL, matrix.GetNumber(k, l));
                            }
                        }
                    }
                    T tmp = ComputationsExtensions.Determinant(modified);
                    result.WriteNumber(j, i, (T)(multiply.__Multiplication(tmp)));
                }
            }

            return result;
        }

        /// <summary>
        /// Computes orthogonal form of <paramref name="matrix"/> through use of Gram-Schmidt orthogonalization.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Matrix object which will serve as a source for orthogonalization.</param>
        /// <returns>New instance of orthogonal Matrix.</returns>
        public static Matrix<T> Orthogonal<T>(this Matrix<T> matrix) where T : IMatrixNumber, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            int rows = matrix.Rows;
            int cols = matrix.Cols;
            Matrix<T> tmpMatrix = new Matrix<T>(matrix);
            Matrix<T> result = Matrix<T>.GetUninitializedMatrix(rows, rows);

            for (int k = 0; k < cols && k < rows; ++k)
            {
                T norm = new T();
                for (int i = 0; i < rows; ++i)
                {
                    norm = (T)(norm.__Addition(tmpMatrix.GetNumber(i, k).__Exponentiate(2)));
                }
                norm = (T)norm.__SquareRoot();

                for (int i = 0; i < rows; ++i)
                {
                    result.WriteNumber(i, k, (T)(tmpMatrix.GetNumber(i, k).__Division(norm)));
                }

                for (int j = k + 1; j < cols; ++j)
                {
                    T dotProduct = new T();
                    for (int i = 0; i < rows; ++i)
                    {
                        dotProduct = (T)(dotProduct.__Addition(tmpMatrix.GetNumber(i, j).__Multiplication(result.GetNumber(i, k))));
                    }

                    for (int i = 0; i < rows; ++i)
                    {
                        tmpMatrix.WriteNumber(i, j, 
                            (T)(tmpMatrix.GetNumber(i, j).__Subtraction(dotProduct.__Multiplication(result.GetNumber(i, k)))));
                    }
                }
            }

            return result;
        }
    }
}
