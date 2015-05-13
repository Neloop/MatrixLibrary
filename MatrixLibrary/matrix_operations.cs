using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLibrary
{
    public static class ClassicOperations
    {
        public static Matrix<T> Addition<T>(Matrix<T> matrix1, Matrix<T> matrix2) where T : MatrixNumberBase, new() // Pokud se řádky a sloupce obou matic nerovnají, pak je vyhozena vyjimka
        {
            Matrix<T> result;
            if (matrix1.Rows == matrix2.Rows && matrix1.Cols == matrix2.Cols)
            {
                int rows = matrix1.Rows;
                int cols = matrix1.Cols;
                result = Matrix<T>.GetUninitializedMatrix(rows, cols);

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        result.WriteNumber(i, j, (T)(matrix1.GetNumber(i, j) + matrix2.GetNumber(i, j)));
                    }
                }
            }
            else
            {
                throw new MatrixLibraryException("Rows and cols of matrixes are not equal");
            }
            return result;
        }

        public static Matrix<T> Addition_MultiThreaded<T>(Matrix<T> matrix1, Matrix<T> matrix2) where T : MatrixNumberBase, new()
        {
            Matrix<T> result;
            if (matrix1.Rows == matrix2.Rows && matrix1.Cols == matrix2.Cols)
            {
                int rows = matrix1.Rows;
                int cols = matrix1.Cols;

                result = Matrix<T>.GetUninitializedMatrix(rows, cols);

                Parallel.ForEach(result.GetRowChunks(), (pair) => 
                {
                    for (int i = pair.Item1; i < pair.Item2; ++i)
                    {
                        for (int j = 0; j < cols; j++)
                        {
                            result.WriteNumber(i, j, (T)(matrix1.GetNumber(i, j) + matrix2.GetNumber(i, j)));
                        }
                    }
                });
            }
            else
            {
                throw new MatrixLibraryException("Rows and cols of matrixes are not equal");
            }
            return result;
        }

        internal static Matrix<T> AdditionOfParts<T>(int rowsResult, int colsResult, Matrix<T> matrix1, int row1, int col1, Matrix<T> matrix2, int row2, int col2) where T : MatrixNumberBase, new() // Sčítá části dvou matic od daných indexů, pokud řádky či sloupce "chybějí", jsou doplněny nulami
        {
            Matrix<T> result = Matrix<T>.GetUninitializedMatrix(rowsResult, colsResult);

            for (int i = 0; i < rowsResult; i++)
            {
                for (int j = 0; j < colsResult; j++)
                {
                    T first = new T();
                    T second = new T();
                    if (i + row1 < matrix1.Rows && j + col1 < matrix1.Cols) { first = (T)matrix1.GetNumber(i + row1, j + col1); }
                    if (i + row2 < matrix2.Rows && j + col2 < matrix2.Cols) { second = (T)matrix2.GetNumber(i + row2, j + col2); }

                    result.WriteNumber(i, j, (T)(first + second));
                }
            }

            return result;
        }

        internal static Matrix<T> AdditionOfParts_MultiThreaded<T>(int rowsResult, int colsResult, Matrix<T> matrix1, int row1, int col1, Matrix<T> matrix2, int row2, int col2) where T : MatrixNumberBase, new() // Sčítá části dvou matic od daných indexů, pokud řádky či sloupce "chybějí", jsou doplněny nulami
        {
            Matrix<T> result = Matrix<T>.GetUninitializedMatrix(rowsResult, colsResult);

            Parallel.ForEach(result.GetRowChunks(), (pair) =>
            {
                for (int i = pair.Item1; i < pair.Item2; ++i)
                {
                    for (int j = 0; j < colsResult; j++)
                    {
                        T first = new T();
                        T second = new T();
                        if (i + row1 < matrix1.Rows && j + col1 < matrix1.Cols) { first = (T)matrix1.GetNumber(i + row1, j + col1); }
                        if (i + row2 < matrix2.Rows && j + col2 < matrix2.Cols) { second = (T)matrix2.GetNumber(i + row2, j + col2); }

                        result.WriteNumber(i, j, (T)(first + second));
                    }
                }
            });

            return result;
        }

        internal static Matrix<T> SubtractionOfParts<T>(int rowsResult, int colsResult, Matrix<T> matrix1, int row1, int col1, Matrix<T> matrix2, int row2, int col2) where T : MatrixNumberBase, new() // Odečítá části dvou matic od daných indexů, pokud řádky či sloupce "chybějí", jsou doplněny nulami
        {
            Matrix<T> result = Matrix<T>.GetUninitializedMatrix(rowsResult, colsResult);

            for (int i = 0; i < rowsResult; i++)
            {
                for (int j = 0; j < colsResult; j++)
                {
                    T first = new T();
                    T second = new T();
                    if (i + row1 < matrix1.Rows && j + col1 < matrix1.Cols) { first = (T)matrix1.GetNumber(i + row1, j + col1); }
                    if (i + row2 < matrix2.Rows && j + col2 < matrix2.Cols) { second = (T)matrix2.GetNumber(i + row2, j + col2); }

                    result.WriteNumber(i, j, (T)(first - second));
                }
            }

            return result;
        }

        internal static Matrix<T> SubtractionOfParts_MultiThreaded<T>(int rowsResult, int colsResult, Matrix<T> matrix1, int row1, int col1, Matrix<T> matrix2, int row2, int col2) where T : MatrixNumberBase, new() // Odečítá části dvou matic od daných indexů, pokud řádky či sloupce "chybějí", jsou doplněny nulami
        {
            Matrix<T> result = Matrix<T>.GetUninitializedMatrix(rowsResult, colsResult);

            Parallel.ForEach(result.GetRowChunks(), (pair) =>
            {
                for (int i = pair.Item1; i < pair.Item2; ++i)
                {
                    for (int j = 0; j < colsResult; j++)
                    {
                        T first = new T();
                        T second = new T();
                        if (i + row1 < matrix1.Rows && j + col1 < matrix1.Cols) { first = (T)matrix1.GetNumber(i + row1, j + col1); }
                        if (i + row2 < matrix2.Rows && j + col2 < matrix2.Cols) { second = (T)matrix2.GetNumber(i + row2, j + col2); }

                        result.WriteNumber(i, j, (T)(first - second));
                    }
                }
            });

            return result;
        }

        public static Matrix<T> Subtraction<T>(Matrix<T> matrix1, Matrix<T> matrix2) where T : MatrixNumberBase, new() // Pokud se řádky a sloupce obou matic nerovnají, pak je vyhozena vyjimka
        {
            Matrix<T> result;
            if (matrix1.Rows == matrix2.Rows && matrix1.Cols == matrix2.Cols)
            {
                int rows = matrix1.Rows;
                int cols = matrix1.Cols;
                result = Matrix<T>.GetUninitializedMatrix(rows, cols);

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        result.WriteNumber(i, j, (T)(matrix1.GetNumber(i, j) - matrix2.GetNumber(i, j)));
                    }
                }
            }
            else
            {
                throw new MatrixLibraryException("Rows and cols of matrixes are not equal");
            }
            return result;
        }

        public static Matrix<T> Subtraction_MultiThreaded<T>(Matrix<T> matrix1, Matrix<T> matrix2) where T : MatrixNumberBase, new() // Pokud se řádky a sloupce obou matic nerovnají, pak je vyhozena vyjimka
        {
            Matrix<T> result;
            if (matrix1.Rows == matrix2.Rows && matrix1.Cols == matrix2.Cols)
            {
                int rows = matrix1.Rows;
                int cols = matrix1.Cols;
                result = Matrix<T>.GetUninitializedMatrix(rows, cols);

                Parallel.ForEach(result.GetRowChunks(), (pair) =>
                {
                    for (int i = pair.Item1; i < pair.Item2; ++i)
                    {
                        for (int j = 0; j < cols; j++)
                        {
                            result.WriteNumber(i, j, (T)(matrix1.GetNumber(i, j) - matrix2.GetNumber(i, j)));
                        }
                    }
                });
            }
            else
            {
                throw new MatrixLibraryException("Rows and cols of matrixes are not equal");
            }
            return result;
        }

        public static Matrix<T> Multiplication<T>(Matrix<T> matrix1, Matrix<T> matrix2) where T : MatrixNumberBase, new() // Pokud se počet sloupců první a počet řádků druhé matice nerovnají, pak je vyhozena vyjimka
        {
            Matrix<T> result;
            int rows1, cols1, cols2;
            rows1 = matrix1.Rows;
            cols1 = matrix1.Cols;
            cols2 = matrix2.Cols;
            if (matrix2.Rows == cols1)
            {
                result = Matrix<T>.GetUninitializedMatrix(rows1, cols2);

                for (int i = 0; i < rows1; i++)
                {
                    for (int j = 0; j < cols2; j++)
                    {
                        T tmp = new T();
                        for (int k = 0; k < cols1; k++)
                        {
                            tmp = (T)((matrix1.GetNumber(i, k) * matrix2.GetNumber(k, j)) + tmp);
                        }
                        result.WriteNumber(i, j, tmp);
                    }
                }
            }
            else
            {
                throw new MatrixLibraryException("Number of rows of second matrix is not equal to number of cols of first matrix");
            }
            return result;
        }

        public static Matrix<T> Multiplication_MultiThreaded<T>(Matrix<T> matrix1, Matrix<T> matrix2) where T : MatrixNumberBase, new() // Pokud se počet sloupců první a počet řádků druhé matice nerovnají, pak je vyhozena vyjimka
        {
            Matrix<T> result;
            int rows1, cols1, cols2;
            rows1 = matrix1.Rows;
            cols1 = matrix1.Cols;
            cols2 = matrix2.Cols;
            if (matrix2.Rows == cols1)
            {
                result = Matrix<T>.GetUninitializedMatrix(rows1, cols2);

                Parallel.ForEach(result.GetRowChunks(), (pair) =>
                {
                    for (int i = pair.Item1; i < pair.Item2; ++i)
                    {
                        for (int j = 0; j < cols2; j++)
                        {
                            T tmp = new T();
                            for (int k = 0; k < cols1; k++)
                            {
                                tmp = (T)(tmp + (matrix1.GetNumber(i, k) * matrix2.GetNumber(k, j)));
                            }
                            result.WriteNumber(i, j, tmp);
                        }
                    }
                });
            }
            else
            {
                throw new MatrixLibraryException("Number of rows of second matrix is not equal to number of cols of first matrix");
            }
            return result;
        }

        public static Matrix<T> MultiplyWithNumber<T>(Matrix<T> matrix, T number) where T : MatrixNumberBase, new()
        {
            Matrix<T> result = Matrix<T>.GetUninitializedMatrix(matrix.Rows, matrix.Cols);

            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Cols; j++)
                {
                    result.WriteNumber(i, j, (T)(matrix.GetNumber(i, j) * number));
                }
            }

            return result;
        }

        public static Matrix<T> MultiplyWithNumber_MultiThreaded<T>(Matrix<T> matrix, T number) where T : MatrixNumberBase, new()
        {
            Matrix<T> result = Matrix<T>.GetUninitializedMatrix(matrix.Rows, matrix.Cols);

            Parallel.ForEach(result.GetRowChunks(), (pair) =>
            {
                for (int i = pair.Item1; i < pair.Item2; ++i)
                {
                    for (int j = 0; j < matrix.Cols; j++)
                    {
                        result.WriteNumber(i, j, (T)(matrix.GetNumber(i, j) * number));
                    }
                }
            });

            return result;
        }

        public static Matrix<T> StrassenWinograd<T>(Matrix<T> matrix1, Matrix<T> matrix2) where T : MatrixNumberBase, new() // Strassen-Winograd
        {
            Matrix<T> result;
            int rows1, rows2, cols1, cols2;
            rows1 = matrix1.Rows;
            rows2 = matrix2.Rows;
            cols1 = matrix1.Cols;
            cols2 = matrix2.Cols;
            if (rows2 == cols1)
            {
                result = Matrix<T>.GetUninitializedMatrix(rows1, cols2);

                int maxSize = Math.Max(Math.Max(rows1, rows2), Math.Max(cols1, cols2));
                if (maxSize <= 2)
                {
                    return ClassicOperations.Multiplication(matrix1, matrix2);
                }

                int size = 1;
                while (maxSize > size) { size *= 2; }
                int del = size / 2;

                // S_1 = A_21 + A_22
                Matrix<T> S_1 = ClassicOperations.AdditionOfParts(del, del, matrix1, del, 0, matrix1, del, del);

                // S_2 = S_1 - A_11
                Matrix<T> S_2 = ClassicOperations.SubtractionOfParts(del, del, S_1, 0, 0, matrix1, 0, 0);

                // S_3 = A_11 - A_21
                Matrix<T> S_3 = ClassicOperations.SubtractionOfParts(del, del, matrix1, 0, 0, matrix1, del, 0);

                // S_4 = A_12 - S_2
                Matrix<T> S_4 = ClassicOperations.SubtractionOfParts(del, del, matrix1, 0, del, S_2, 0, 0);

                // S_5 = B_12 - B_11
                Matrix<T> S_5 = ClassicOperations.SubtractionOfParts(del, del, matrix2, 0, del, matrix2, 0, 0);

                // S_6 = B_22 - S_5
                Matrix<T> S_6 = ClassicOperations.SubtractionOfParts(del, del, matrix2, del, del, S_5, 0, 0);

                // S_7 = B_22 - B_12
                Matrix<T> S_7 = ClassicOperations.SubtractionOfParts(del, del, matrix2, del, del, matrix2, 0, del);

                // S_8 = B_21 - S_6
                Matrix<T> S_8 = ClassicOperations.SubtractionOfParts(del, del, matrix2, del, 0, S_6, 0, 0);

                // N_1 = A_11 * B_11
                Matrix<T> A_11 = Matrix<T>.GetUninitializedMatrix(del, del);
                Matrix<T> B_11 = Matrix<T>.GetUninitializedMatrix(del, del);
                for (int i = 0; i < del; i++)
                {
                    for (int j = 0; j < del; j++)
                    {
                        T tmp = new T();
                        tmp = matrix1.GetNumber(i, j);
                        A_11.WriteNumber(i, j, tmp);

                        tmp = matrix2.GetNumber(i, j);
                        B_11.WriteNumber(i, j, tmp);
                    }
                }
                Matrix<T> N_1 = ClassicOperations.StrassenWinograd(A_11, B_11);

                // N_2 = A_12 * B_21
                Matrix<T> A_12 = Matrix<T>.GetUninitializedMatrix(del, del);
                Matrix<T> B_21 = Matrix<T>.GetUninitializedMatrix(del, del);
                for (int i = 0; i < del; i++)
                {
                    for (int j = 0; j < del; j++)
                    {
                        T tmp = new T();
                        if (del + j < matrix1.Cols) { tmp = matrix1.GetNumber(i, del + j); }
                        A_12.WriteNumber(i, j, tmp);

                        if (del + i < matrix2.Rows) { tmp = matrix2.GetNumber(del + i, j); }
                        B_21.WriteNumber(i, j, tmp);
                    }
                }
                Matrix<T> N_2 = ClassicOperations.StrassenWinograd(A_12, B_21);

                // N_3 = S_1 * S_5
                Matrix<T> N_3 = ClassicOperations.StrassenWinograd(S_1, S_5);

                // N_4 = S_2 * S_6
                Matrix<T> N_4 = ClassicOperations.StrassenWinograd(S_2, S_6);

                // N_5 = S_3 * S_7
                Matrix<T> N_5 = ClassicOperations.StrassenWinograd(S_3, S_7);

                // N_6 = S_4 * B_22
                Matrix<T> B_22 = Matrix<T>.GetUninitializedMatrix(del, del);
                for (int i = 0; i < del; i++)
                {
                    for (int j = 0; j < del; j++)
                    {
                        T tmp = new T();
                        if (del + i < matrix2.Rows && del + j < matrix2.Cols) { tmp = matrix2.GetNumber(del + i, del + j); }
                        B_22.WriteNumber(i, j, tmp);
                    }
                }
                Matrix<T> N_6 = ClassicOperations.StrassenWinograd(S_4, B_22);

                // N_7 = A_22 * S_8
                Matrix<T> A_22 = Matrix<T>.GetUninitializedMatrix(del, del);
                for (int i = 0; i < del; i++)
                {
                    for (int j = 0; j < del; j++)
                    {
                        T tmp = new T();
                        if (del + j < matrix1.Cols && del + i < matrix1.Rows) { tmp = matrix1.GetNumber(del + i, del + j); }
                        A_22.WriteNumber(i, j, tmp);
                    }
                }
                Matrix<T> N_7 = ClassicOperations.StrassenWinograd(A_22, S_8);

                // S_9 = N_1 + N_4
                Matrix<T> S_9 = ClassicOperations.Addition(N_1, N_4);

                // S_10 = S_9 + N_3
                Matrix<T> S_10 = ClassicOperations.Addition(S_9, N_3);

                // S_11 = S_9 + N_5
                Matrix<T> S_11 = ClassicOperations.Addition(S_9, N_5);

                // S_12 = N_1 + N_2 = C_11
                Matrix<T> S_12 = ClassicOperations.Addition(N_1, N_2);

                // S_13 = S_10 + N_6 = C_12
                Matrix<T> S_13 = ClassicOperations.Addition(S_10, N_6);

                // S_14 = S_11 + N_7 = C_21
                Matrix<T> S_14 = ClassicOperations.Addition(S_11, N_7);

                // S_15 = S_10 + N_5 = C_22
                Matrix<T> S_15 = ClassicOperations.Addition(S_10, N_5);

                for (int i = 0; i < del; i++)
                {
                    for (int j = 0; j < del; j++)
                    {
                        result.WriteNumber(i, j, S_12.GetNumber(i, j)); // C_11

                        if (result.Cols > (del + j)) // C_12
                        {
                            result.WriteNumber(i, del + j, S_13.GetNumber(i, j));
                        }

                        if (result.Rows > (del + i)) // C_21
                        {
                            result.WriteNumber(i + del, j, S_14.GetNumber(i, j));
                        }

                        if (result.Cols > (del + j) && result.Rows > (del + i)) // C_22
                        {
                            result.WriteNumber(del + i, del + j, S_15.GetNumber(i, j));
                        }
                    }
                }
            }
            else
            {
                throw new MatrixLibraryException("Number of rows of second matrix is not equal to number of cols of first matrix");
            }
            return result;
        }

        public static Matrix<T> StrassenWinograd_MultiThreaded<T>(Matrix<T> matrix1, Matrix<T> matrix2) where T : MatrixNumberBase, new() // Strassen-Winograd
        {
            Matrix<T> result;
            int rows1, rows2, cols1, cols2;
            rows1 = matrix1.Rows;
            rows2 = matrix2.Rows;
            cols1 = matrix1.Cols;
            cols2 = matrix2.Cols;
            if (rows2 == cols1)
            {
                result = Matrix<T>.GetUninitializedMatrix(rows1, cols2);

                int maxSize = Math.Max(Math.Max(rows1, rows2), Math.Max(cols1, cols2));
                if (maxSize <= 2)
                {
                    return ClassicOperations.Multiplication(matrix1, matrix2);
                }

                int size = 1;
                while (maxSize > size) { size *= 2; }
                int del = size / 2;

                // S_1 = A_21 + A_22
                Task<Matrix<T>> S_1 = Task<Matrix<T>>.Run(() => ClassicOperations.AdditionOfParts(del, del, matrix1, del, 0, matrix1, del, del));

                // S_2 = S_1 - A_11
                Task<Matrix<T>> S_2 = S_1.ContinueWith<Matrix<T>>((s_1) => ClassicOperations.SubtractionOfParts(del, del, S_1.Result, 0, 0, matrix1, 0, 0));

                // S_3 = A_11 - A_21
                Task<Matrix<T>> S_3 = Task<Matrix<T>>.Run(() => ClassicOperations.SubtractionOfParts(del, del, matrix1, 0, 0, matrix1, del, 0));

                // S_4 = A_12 - S_2
                Task<Matrix<T>> S_4 = S_2.ContinueWith<Matrix<T>>((s_2) => ClassicOperations.SubtractionOfParts(del, del, matrix1, 0, del, S_2.Result, 0, 0));

                // S_5 = B_12 - B_11
                Task<Matrix<T>> S_5 = Task<Matrix<T>>.Run(() => ClassicOperations.SubtractionOfParts(del, del, matrix2, 0, del, matrix2, 0, 0));

                // S_6 = B_22 - S_5
                Task<Matrix<T>> S_6 = S_5.ContinueWith<Matrix<T>>((s_5) => ClassicOperations.SubtractionOfParts(del, del, matrix2, del, del, S_5.Result, 0, 0));

                // S_7 = B_22 - B_12
                Task<Matrix<T>> S_7 = Task<Matrix<T>>.Run(() => ClassicOperations.SubtractionOfParts(del, del, matrix2, del, del, matrix2, 0, del));

                // S_8 = B_21 - S_6
                Task<Matrix<T>> S_8 = S_6.ContinueWith<Matrix<T>>((s_6) => ClassicOperations.SubtractionOfParts(del, del, matrix2, del, 0, S_6.Result, 0, 0));

                // N_1 = A_11 * B_11
                Task<Matrix<T>> N_1 = Task<Matrix<T>>.Run(() =>
                {
                    Matrix<T> A_11 = Matrix<T>.GetUninitializedMatrix(del, del);
                    Matrix<T> B_11 = Matrix<T>.GetUninitializedMatrix(del, del);
                    Parallel.ForEach(A_11.GetRowChunks(), (pair) =>
                    {
                        for (int i = pair.Item1; i < pair.Item2; i++)
                        {
                            for (int j = 0; j < del; j++)
                            {
                                T tmp = new T();
                                tmp = matrix1.GetNumber(i, j);
                                A_11.WriteNumber(i, j, tmp);

                                tmp = matrix2.GetNumber(i, j);
                                B_11.WriteNumber(i, j, tmp);
                            }
                        }
                    });

                    return ClassicOperations.StrassenWinograd(A_11, B_11);
                });

                // N_2 = A_12 * B_21
                Task<Matrix<T>> N_2 = Task<Matrix<T>>.Run(() =>
                {
                    Matrix<T> A_12 = Matrix<T>.GetUninitializedMatrix(del, del);
                    Matrix<T> B_21 = Matrix<T>.GetUninitializedMatrix(del, del);
                    Parallel.ForEach(A_12.GetRowChunks(), (pair) =>
                    {
                        for (int i = pair.Item1; i < pair.Item2; i++)
                        {
                            for (int j = 0; j < del; j++)
                            {
                                T tmp = new T();
                                if (del + j < matrix1.Cols) { tmp = matrix1.GetNumber(i, del + j); }
                                A_12.WriteNumber(i, j, tmp);

                                if (del + i < matrix2.Rows) { tmp = matrix2.GetNumber(del + i, j); }
                                B_21.WriteNumber(i, j, tmp);
                            }
                        }
                    });

                    return ClassicOperations.StrassenWinograd(A_12, B_21);
                });

                // N_3 = S_1 * S_5
                Task<Matrix<T>> N_3 = Task.Factory.ContinueWhenAll(new Task<Matrix<T>>[] { S_1, S_5 },
                    (_) => ClassicOperations.StrassenWinograd(S_1.Result, S_5.Result));

                // N_4 = S_2 * S_6
                Task<Matrix<T>> N_4 = Task.Factory.ContinueWhenAll(new Task<Matrix<T>>[] { S_2, S_6 },
                    (_) => ClassicOperations.StrassenWinograd(S_2.Result, S_6.Result));

                // N_5 = S_3 * S_7
                Task<Matrix<T>> N_5 = Task.Factory.ContinueWhenAll(new Task<Matrix<T>>[] { S_3, S_7 },
                    (_) => ClassicOperations.StrassenWinograd(S_3.Result, S_7.Result));

                // N_6 = S_4 * B_22
                Task<Matrix<T>> N_6 = Task<Matrix<T>>.Run(() =>
                {
                    Matrix<T> B_22 = Matrix<T>.GetUninitializedMatrix(del, del);
                    Parallel.ForEach(B_22.GetRowChunks(), (pair) =>
                    {
                        for (int i = pair.Item1; i < pair.Item2; i++)
                        {
                            for (int j = 0; j < del; j++)
                            {
                                T tmp = new T();
                                if (del + i < matrix2.Rows && del + j < matrix2.Cols) { tmp = matrix2.GetNumber(del + i, del + j); }
                                B_22.WriteNumber(i, j, tmp);
                            }
                        }
                    });

                    return ClassicOperations.StrassenWinograd(S_4.Result, B_22);
                });

                // N_7 = A_22 * S_8
                Task<Matrix<T>> N_7 = Task<Matrix<T>>.Run(() =>
                {
                    Matrix<T> A_22 = Matrix<T>.GetUninitializedMatrix(del, del);
                    Parallel.ForEach(A_22.GetRowChunks(), (pair) =>
                    {
                        for (int i = pair.Item1; i < pair.Item2; i++)
                        {
                            for (int j = 0; j < del; j++)
                            {
                                T tmp = new T();
                                if (del + j < matrix1.Cols && del + i < matrix1.Rows) { tmp = matrix1.GetNumber(del + i, del + j); }
                                A_22.WriteNumber(i, j, tmp);
                            }
                        }
                    });

                    return ClassicOperations.StrassenWinograd(A_22, S_8.Result);
                });

                // S_9 = N_1 + N_4
                Task<Matrix<T>> S_9 = Task.Factory.ContinueWhenAll(new Task<Matrix<T>>[] { N_1, N_4 },
                    (_) => ClassicOperations.Addition(N_1.Result, N_4.Result));

                // S_10 = S_9 + N_3
                Task<Matrix<T>> S_10 = Task.Factory.ContinueWhenAll(new Task<Matrix<T>>[] { S_9, N_3 },
                    (_) => ClassicOperations.Addition(S_9.Result, N_3.Result));

                // S_11 = S_9 + N_5
                Task<Matrix<T>> S_11 = Task.Factory.ContinueWhenAll(new Task<Matrix<T>>[] { S_9, N_5 },
                    (_) => ClassicOperations.Addition(S_9.Result, N_5.Result));

                // S_12 = N_1 + N_2 = C_11
                Task<Matrix<T>> S_12 = Task.Factory.ContinueWhenAll(new Task<Matrix<T>>[] { N_1, N_2 },
                    (_) => ClassicOperations.Addition(N_1.Result, N_2.Result));

                // S_13 = S_10 + N_6 = C_12
                Task<Matrix<T>> S_13 = Task.Factory.ContinueWhenAll(new Task<Matrix<T>>[] { S_10, N_6 },
                    (_) => ClassicOperations.Addition(S_10.Result, N_6.Result));

                // S_14 = S_11 + N_7 = C_21
                Task<Matrix<T>> S_14 = Task.Factory.ContinueWhenAll(new Task<Matrix<T>>[] { S_11, N_7 },
                    (_) => ClassicOperations.Addition(S_11.Result, N_7.Result));

                // S_15 = S_10 + N_5 = C_22
                Task<Matrix<T>> S_15 = Task.Factory.ContinueWhenAll(new Task<Matrix<T>>[] { S_10, N_5 },
                    (_) => ClassicOperations.Addition(S_10.Result, N_5.Result));

                Task WriteC_11 = S_12.ContinueWith((s_12) =>
                {
                    for(int i = 0; i < del; i++)
                    {
                        for (int j = 0; j < del; j++)
                        {
                            result.WriteNumber(i, j, S_12.Result.GetNumber(i, j)); // C_11
                        }
                    }
                });

                Task WriteC_12 = S_13.ContinueWith((s_13) =>
                {
                    for (int i = 0; i < del; i++)
                    {
                        for (int j = 0; j < del; j++)
                        {
                            if ((del + j) < result.Cols) { result.WriteNumber(i, del + j, S_13.Result.GetNumber(i, j)); } // C_12
                            else { break; }
                        }
                    }
                });

                Task WriteC_21 = S_14.ContinueWith((s_14) =>
                {
                    for (int i = 0; i < del; i++)
                    {
                        for (int j = 0; j < del; j++)
                        {
                            if ((del + i) < result.Rows) { result.WriteNumber(i + del, j, S_14.Result.GetNumber(i, j)); } // C_21
                            else { break; }
                        }
                    }
                });

                Task WriteC_22 = S_15.ContinueWith((s_15) =>
                {
                    for (int i = 0; i < del; i++)
                    {
                        for (int j = 0; j < del; j++)
                        {
                            if ((del + i) < result.Rows && (del + j) < result.Cols) { result.WriteNumber(del + i, del + j, S_15.Result.GetNumber(i, j)); } // C_22
                            else { break; }
                        }
                    }
                });

                Task.WaitAll(new Task[]{ WriteC_11, WriteC_12, WriteC_21, WriteC_22 });
            }
            else
            {
                throw new MatrixLibraryException("Number of rows of second matrix is not equal to number of cols of first matrix");
            }
            return result;
        }
        
        public static Matrix<T> Exponentiate<T>(Matrix<T> matrix, int exponent, bool tryEigenvalues = false) where T : MatrixNumberBase, new() // Zkusí využít vlastních čísel, pokud se v daném čase nevypočítají, přejde se k Strassenovi
        {
            Matrix<T> result;
            if (matrix.Rows == matrix.Cols)
            {
                Matrix<T> exponentiate = null;
                Matrix<T> S = null;
                if (tryEigenvalues == true)
                {
                    try { exponentiate = Charakteristika.Diagonalizovat(matrix, out S, 1000); }
                    catch (EigenValuesNotFoundException) { exponentiate = null; }
                }

                if(exponentiate == null)
                {
                    result = new Matrix<T>(matrix);
                    for (int i = 1; i < exponent; i++)
                    {
                        result = result * matrix;
                    }
                }
                else
                {
                    for (int i = 0; i < exponentiate.Rows; i++)
                    {
                        exponentiate.WriteNumber(i, i, (T)exponentiate.GetNumber(i, i).__Exponentiate(exponent));
                    }

                    result = S * exponentiate * AlteringOperations.Inverzni(S);
                }
            }
            else
            {
                throw new MatrixLibraryException("Number rows and cols is not equal");
            }
            return result;
        }

        public static Matrix<T> Exponentiate_MultiThreaded<T>(Matrix<T> matrix, int exponent, bool tryEigenvalues = false) where T : MatrixNumberBase, new() // Zkusí využít vlastních čísel, pokud se v daném čase nevypočítají, přejde se k Strassenovi
        {
            Matrix<T> result;
            if (matrix.Rows == matrix.Cols)
            {
                Matrix<T> exponentiate = null;
                Matrix<T> S = null;
                if (tryEigenvalues == true)
                {
                    try { exponentiate = Charakteristika.Diagonalizovat(matrix, out S, 1000); }
                    catch (EigenValuesNotFoundException) { exponentiate = null; }
                }

                if (exponentiate == null)
                {
                    result = new Matrix<T>(matrix);
                    for (int i = 1; i < exponent; i++)
                    {
                        result = Multiplication_MultiThreaded(result, matrix);
                    }
                }
                else
                {
                    Parallel.ForEach(exponentiate.GetRowChunks(), (pair) =>
                    {
                        for (int i = pair.Item1; i < pair.Item2; i++)
                        {
                            exponentiate.WriteNumber(i, i, (T)exponentiate.GetNumber(i, i).__Exponentiate(exponent));
                        }
                    });

                    result = Multiplication_MultiThreaded(S, exponentiate);
                    result = Multiplication_MultiThreaded(result, AlteringOperations.Inverzni(S));
                }
            }
            else
            {
                throw new MatrixLibraryException("Number rows and cols is not equal");
            }
            return result;
        }
    }
}
