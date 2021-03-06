﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLibrary
{
    /// <summary>
    /// Namespace of parallel methods unifying some classic arithmetic operations on matrixes.
    /// </summary>
    public static class ParallelClassicOperations
    {
        /// <summary>
        /// Summarize <paramref name="matrix1"/> and <paramref name="matrix2"/> and return result.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix1">First parameter of addition.</param>
        /// <param name="matrix2">Second parameter of addition.</param>
        /// <returns>Newly created Matrix class instance storing result of addition.</returns>
        public static Matrix<T> AdditionParallel<T>(Matrix<T> matrix1, Matrix<T> matrix2) where T : IMatrixNumber, new()
        {
            if (matrix1 == null || matrix2 == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result;
            if (matrix1.Rows == matrix2.Rows && matrix1.Cols == matrix2.Cols)
            {
                int cols = matrix1.Cols;
                result = Matrix<T>.GetUninitializedMatrix(matrix1.Rows, cols);

                Parallel.ForEach(result.GetRowsChunks(), (pair) =>
                {
                    for (int i = pair.Item1; i < pair.Item2; ++i)
                    {
                        for (int j = 0; j < cols; j++)
                        {
                            result.WriteNumber(i, j, (T)(matrix1.GetNumber(i, j).__Addition(matrix2.GetNumber(i, j))));
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

        /// <summary>
        /// Summarize parts of two matrixes <paramref name="matrix1"/> and <paramref name="matrix2"/> and returns result. Parts are specified by indexes of starting rows and cols.
        /// </summary>
        /// <remarks>
        /// If one of the matrixes is too small, then missing elements are replaces by zeroes.
        /// </remarks>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="rowsResult">Tells how many rows result Matrix will have.</param>
        /// <param name="colsResult">Tells how many columns will result Matrix have.</param>
        /// <param name="matrix1">First summand.</param>
        /// <param name="row1">Starting row in <paramref name="matrix1"/>.</param>
        /// <param name="col1">Starting column in <paramref name="matrix1"/>.</param>
        /// <param name="matrix2">Second summand.</param>
        /// <param name="row2">Starting row in <paramref name="matrix2"/>.</param>
        /// <param name="col2">Starting column in <paramref name="matrix2"/>.</param>
        /// <returns>Created instance of Matrix class.</returns>
        internal static Matrix<T> AdditionOfPartsParallel<T>(int rowsResult, int colsResult, Matrix<T> matrix1, int row1, int col1, Matrix<T> matrix2, int row2, int col2) where T : IMatrixNumber, new()
        {
            if (matrix1 == null || matrix2 == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result = Matrix<T>.GetUninitializedMatrix(rowsResult, colsResult);

            Parallel.ForEach(result.GetRowsChunks(), (pair) =>
            {
                for (int i = pair.Item1; i < pair.Item2; ++i)
                {
                    for (int j = 0; j < colsResult; j++)
                    {
                        T first = new T();
                        T second = new T();
                        if (i + row1 < matrix1.Rows && j + col1 < matrix1.Cols) { first = (T)matrix1.GetNumber(i + row1, j + col1); }
                        if (i + row2 < matrix2.Rows && j + col2 < matrix2.Cols) { second = (T)matrix2.GetNumber(i + row2, j + col2); }

                        result.WriteNumber(i, j, (T)(first.__Addition(second)));
                    }
                }
            });

            return result;
        }

        /// <summary>
        /// Subtract parts of two matrixes <paramref name="matrix1"/> and <paramref name="matrix2"/> and returns result. Parts are specified by indexes of starting rows and cols.
        /// </summary>
        /// <remarks>
        /// If one of the matrixes is too small, then missing elements are replaces by zeroes.
        /// </remarks>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="rowsResult">Tells how many rows result Matrix will have.</param>
        /// <param name="colsResult">Tells how many columns will result Matrix have.</param>
        /// <param name="matrix1">Minuend.</param>
        /// <param name="row1">Starting row in <paramref name="matrix1"/>.</param>
        /// <param name="col1">Starting column in <paramref name="matrix1"/>.</param>
        /// <param name="matrix2">Subtrahend.</param>
        /// <param name="row2">Starting row in <paramref name="matrix2"/>.</param>
        /// <param name="col2">Starting column in <paramref name="matrix2"/>.</param>
        /// <returns>Created instance of Matrix class.</returns>
        internal static Matrix<T> SubtractionOfPartsParallel<T>(int rowsResult, int colsResult, Matrix<T> matrix1, int row1, int col1, Matrix<T> matrix2, int row2, int col2) where T : IMatrixNumber, new()
        {
            if (matrix1 == null || matrix2 == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result = Matrix<T>.GetUninitializedMatrix(rowsResult, colsResult);

            Parallel.ForEach(result.GetRowsChunks(), (pair) =>
            {
                for (int i = pair.Item1; i < pair.Item2; ++i)
                {
                    for (int j = 0; j < colsResult; j++)
                    {
                        T first = new T();
                        T second = new T();
                        if (i + row1 < matrix1.Rows && j + col1 < matrix1.Cols) { first = (T)matrix1.GetNumber(i + row1, j + col1); }
                        if (i + row2 < matrix2.Rows && j + col2 < matrix2.Cols) { second = (T)matrix2.GetNumber(i + row2, j + col2); }

                        result.WriteNumber(i, j, (T)(first.__Subtraction(second)));
                    }
                }
            });

            return result;
        }

        /// <summary>
        /// Subtract <paramref name="matrix1"/> and <paramref name="matrix2"/> and return result.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix1">Minuend.</param>
        /// <param name="matrix2">Subtrahend.</param>
        /// <returns>Newly created Matrix with result of subtraction.</returns>
        public static Matrix<T> SubtractionParallel<T>(Matrix<T> matrix1, Matrix<T> matrix2) where T : IMatrixNumber, new()
        {
            if (matrix1 == null || matrix2 == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result;
            if (matrix1.Rows == matrix2.Rows && matrix1.Cols == matrix2.Cols)
            {
                int cols = matrix1.Cols;
                result = Matrix<T>.GetUninitializedMatrix(matrix1.Rows, cols);

                Parallel.ForEach(result.GetRowsChunks(), (pair) =>
                {
                    for (int i = pair.Item1; i < pair.Item2; ++i)
                    {
                        for (int j = 0; j < cols; j++)
                        {
                            result.WriteNumber(i, j, (T)(matrix1.GetNumber(i, j).__Subtraction(matrix2.GetNumber(i, j))));
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

        /// <summary>
        /// Classic multiplication of two matrixes with slow n^3 algorithm.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix1">Multiplicant.</param>
        /// <param name="matrix2">Multiplier.</param>
        /// <returns>Matrix class with result of multiplication of <paramref name="matrix1"/> and <paramref name="matrix2"/>.</returns>
        public static Matrix<T> MultiplicationParallel<T>(Matrix<T> matrix1, Matrix<T> matrix2) where T : IMatrixNumber, new()
        {
            if (matrix1 == null || matrix2 == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result;
            int cols1, cols2;
            cols1 = matrix1.Cols;
            cols2 = matrix2.Cols;
            if (matrix2.Rows == cols1)
            {
                result = Matrix<T>.GetUninitializedMatrix(matrix1.Rows, cols2);

                Parallel.ForEach(result.GetRowsChunks(), (pair) =>
                {
                    for (int i = pair.Item1; i < pair.Item2; ++i)
                    {
                        for (int j = 0; j < cols2; j++)
                        {
                            T tmp = new T();
                            for (int k = 0; k < cols1; k++)
                            {
                                tmp = (T)(tmp.__Addition(matrix1.GetNumber(i, k).__Multiplication(matrix2.GetNumber(k, j))));
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

        /// <summary>
        /// Given <paramref name="matrix"/> will multiply with <paramref name="number"/>.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">First parameter of multiplication.</param>
        /// <param name="number">Number with which all <paramref name="matrix"/> elements will be multiplied.</param>
        /// <returns>Matrix class with multiplied elements.</returns>
        public static Matrix<T> MultiplyWithNumberParallel<T>(Matrix<T> matrix, T number) where T : IMatrixNumber, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }
            if (number == null) { throw new MatrixLibraryException("In given number reference was null value!"); }

            int cols = matrix.Cols;
            Matrix<T> result = Matrix<T>.GetUninitializedMatrix(matrix.Rows, cols);

            Parallel.ForEach(result.GetRowsChunks(), (pair) =>
            {
                for (int i = pair.Item1; i < pair.Item2; ++i)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        result.WriteNumber(i, j, (T)(matrix.GetNumber(i, j).__Multiplication(number)));
                    }
                }
            });

            return result;
        }

        /// <summary>
        /// Strassen-Winograd algorithm which multiplies two matrixes, <paramref name="matrix1"/> and <paramref name="matrix2"/>.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix1">Multiplicant.</param>
        /// <param name="matrix2">Multiplier.</param>
        /// <returns>Suitable newly created Matrix object.</returns>
        public static Matrix<T> StrassenWinogradParallel<T>(Matrix<T> matrix1, Matrix<T> matrix2) where T : IMatrixNumber, new()
        {
            if (matrix1 == null || matrix2 == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

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
                Task<Matrix<T>> S_2 = S_1.ContinueWith<Matrix<T>>((s1) => ClassicOperations.SubtractionOfParts(del, del, s1.Result, 0, 0, matrix1, 0, 0));

                // S_3 = A_11 - A_21
                Task<Matrix<T>> S_3 = Task<Matrix<T>>.Run(() => ClassicOperations.SubtractionOfParts(del, del, matrix1, 0, 0, matrix1, del, 0));

                // S_4 = A_12 - S_2
                Task<Matrix<T>> S_4 = S_2.ContinueWith<Matrix<T>>((s2) => ClassicOperations.SubtractionOfParts(del, del, matrix1, 0, del, s2.Result, 0, 0));

                // S_5 = B_12 - B_11
                Task<Matrix<T>> S_5 = Task<Matrix<T>>.Run(() => ClassicOperations.SubtractionOfParts(del, del, matrix2, 0, del, matrix2, 0, 0));

                // S_6 = B_22 - S_5
                Task<Matrix<T>> S_6 = S_5.ContinueWith<Matrix<T>>((s5) => ClassicOperations.SubtractionOfParts(del, del, matrix2, del, del, s5.Result, 0, 0));

                // S_7 = B_22 - B_12
                Task<Matrix<T>> S_7 = Task<Matrix<T>>.Run(() => ClassicOperations.SubtractionOfParts(del, del, matrix2, del, del, matrix2, 0, del));

                // S_8 = B_21 - S_6
                Task<Matrix<T>> S_8 = S_6.ContinueWith<Matrix<T>>((s6) => ClassicOperations.SubtractionOfParts(del, del, matrix2, del, 0, s6.Result, 0, 0));

                // N_1 = A_11 * B_11
                Task<Matrix<T>> N_1 = Task<Matrix<T>>.Run(() =>
                {
                    Matrix<T> A_11 = Matrix<T>.GetUninitializedMatrix(del, del);
                    Matrix<T> B_11 = Matrix<T>.GetUninitializedMatrix(del, del);
                    Parallel.ForEach(A_11.GetRowsChunks(), (pair) =>
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
                    Parallel.ForEach(A_12.GetRowsChunks(), (pair) =>
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
                    tasks => ClassicOperations.StrassenWinograd(tasks[0].Result, tasks[1].Result));

                // N_4 = S_2 * S_6
                Task<Matrix<T>> N_4 = Task.Factory.ContinueWhenAll(new Task<Matrix<T>>[] { S_2, S_6 },
                    tasks => ClassicOperations.StrassenWinograd(tasks[0].Result, tasks[1].Result));

                // N_5 = S_3 * S_7
                Task<Matrix<T>> N_5 = Task.Factory.ContinueWhenAll(new Task<Matrix<T>>[] { S_3, S_7 },
                    tasks => ClassicOperations.StrassenWinograd(tasks[0].Result, tasks[1].Result));

                // N_6 = S_4 * B_22
                Task<Matrix<T>> N_6 = Task<Matrix<T>>.Run(() =>
                {
                    Matrix<T> B_22 = Matrix<T>.GetUninitializedMatrix(del, del);
                    Parallel.ForEach(B_22.GetRowsChunks(), (pair) =>
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
                    Parallel.ForEach(A_22.GetRowsChunks(), (pair) =>
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
                    tasks => ClassicOperations.Addition(tasks[0].Result, tasks[1].Result));

                // S_10 = S_9 + N_3
                Task<Matrix<T>> S_10 = Task.Factory.ContinueWhenAll(new Task<Matrix<T>>[] { S_9, N_3 },
                    tasks => ClassicOperations.Addition(tasks[0].Result, tasks[1].Result));

                // S_11 = S_9 + N_5
                Task<Matrix<T>> S_11 = Task.Factory.ContinueWhenAll(new Task<Matrix<T>>[] { S_9, N_5 },
                    tasks => ClassicOperations.Addition(tasks[0].Result, tasks[1].Result));

                // S_12 = N_1 + N_2 = C_11
                Task<Matrix<T>> S_12 = Task.Factory.ContinueWhenAll(new Task<Matrix<T>>[] { N_1, N_2 },
                    tasks => ClassicOperations.Addition(tasks[0].Result, tasks[1].Result));

                // S_13 = S_10 + N_6 = C_12
                Task<Matrix<T>> S_13 = Task.Factory.ContinueWhenAll(new Task<Matrix<T>>[] { S_10, N_6 },
                    tasks => ClassicOperations.Addition(tasks[0].Result, tasks[1].Result));

                // S_14 = S_11 + N_7 = C_21
                Task<Matrix<T>> S_14 = Task.Factory.ContinueWhenAll(new Task<Matrix<T>>[] { S_11, N_7 },
                    tasks => ClassicOperations.Addition(tasks[0].Result, tasks[1].Result));

                // S_15 = S_10 + N_5 = C_22
                Task<Matrix<T>> S_15 = Task.Factory.ContinueWhenAll(new Task<Matrix<T>>[] { S_10, N_5 },
                    tasks => ClassicOperations.Addition(tasks[0].Result, tasks[1].Result));

                Task WriteC_11 = S_12.ContinueWith((s12) =>
                {
                    for (int i = 0; i < del; i++)
                    {
                        for (int j = 0; j < del; j++)
                        {
                            result.WriteNumber(i, j, s12.Result.GetNumber(i, j)); // C_11
                        }
                    }
                });

                Task WriteC_12 = S_13.ContinueWith((s13) =>
                {
                    for (int i = 0; i < del; i++)
                    {
                        for (int j = 0; j < del; j++)
                        {
                            if ((del + j) < result.Cols) { result.WriteNumber(i, del + j, s13.Result.GetNumber(i, j)); } // C_12
                            else { break; }
                        }
                    }
                });

                Task WriteC_21 = S_14.ContinueWith((s14) =>
                {
                    for (int i = 0; i < del; i++)
                    {
                        for (int j = 0; j < del; j++)
                        {
                            if ((del + i) < result.Rows) { result.WriteNumber(i + del, j, s14.Result.GetNumber(i, j)); } // C_21
                            else { break; }
                        }
                    }
                });

                Task WriteC_22 = S_15.ContinueWith((s15) =>
                {
                    for (int i = 0; i < del; i++)
                    {
                        for (int j = 0; j < del; j++)
                        {
                            if ((del + i) < result.Rows && (del + j) < result.Cols) { result.WriteNumber(del + i, del + j, s15.Result.GetNumber(i, j)); } // C_22
                            else { break; }
                        }
                    }
                });

                Task.WaitAll(new Task[] { WriteC_11, WriteC_12, WriteC_21, WriteC_22 });
            }
            else
            {
                throw new MatrixLibraryException("Number of rows of second matrix is not equal to number of cols of first matrix");
            }
            return result;
        }

        /// <summary>
        /// Exponents <paramref name="matrix"/> with <paramref name="exponent"/>.
        /// </summary>
        /// <remarks>
        /// If <paramref name="tryEigenvalues"/> is set to true, then firstly it tries to exponentiate <paramref name="matrix"/> with the help of eigenvalues. If eigenvalues are not calculated in time, then algorithm will use classic multiplication.
        /// </remarks>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Matrix object which will be exponentiated.</param>
        /// <param name="exponent">Numbers of multiplication (-1) in exponentiation.</param>
        /// <param name="tryEigenvalues">True if eigenvalues will be used to calculate exponentiation, false otherwise.</param>
        /// <returns>New instance of Matrix class with exponentiated matrix.</returns>
        public static Matrix<T> ExponentiateParallel<T>(Matrix<T> matrix, int exponent, bool tryEigenvalues = false) where T : IMatrixNumber, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result;
            if (matrix.Rows == matrix.Cols)
            {
                Matrix<T> exponentiate = null;
                Matrix<T> S = null;
                if (tryEigenvalues == true)
                {
                    try { exponentiate = CharacteristicsExtensions.Diagonal(matrix, out S, 1000); }
                    catch (EigenValuesNotFoundException) { exponentiate = null; }
                }

                if (exponentiate == null)
                {
                    result = new Matrix<T>(matrix);
                    for (int i = 1; i < exponent; i++)
                    {
                        result = ParallelClassicOperations.MultiplicationParallel(result, matrix);
                    }
                }
                else
                {
                    Parallel.ForEach(exponentiate.GetRowsChunks(), (pair) =>
                    {
                        for (int i = pair.Item1; i < pair.Item2; i++)
                        {
                            exponentiate.WriteNumber(i, i, (T)exponentiate.GetNumber(i, i).__Exponentiate(exponent));
                        }
                    });

                    result = ParallelClassicOperations.MultiplicationParallel(S, exponentiate);
                    result = ParallelClassicOperations.MultiplicationParallel(result, AlteringOperationsExtensions.Inverse(S));
                }
            }
            else
            {
                throw new MatrixLibraryException("Number rows and cols is not equal");
            }
            return result;
        }
    }

    /// <summary>
    /// Namespace of methods unifying some classic arithmetic operations on matrixes.
    /// </summary>
    public static class ClassicOperations
    {
        /// <summary>
        /// Summarize <paramref name="matrix1"/> and <paramref name="matrix2"/> and return result.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix1">First parameter of addition.</param>
        /// <param name="matrix2">Second parameter of addition.</param>
        /// <returns>Newly created Matrix class instance storing result of addition.</returns>
        public static Matrix<T> Addition<T>(Matrix<T> matrix1, Matrix<T> matrix2) where T : IMatrixNumber, new()
        {
            if (matrix1 == null || matrix2 == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

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
                        result.WriteNumber(i, j, (T)(matrix1.GetNumber(i, j).__Addition(matrix2.GetNumber(i, j))));
                    }
                }
            }
            else
            {
                throw new MatrixLibraryException("Rows and cols of matrixes are not equal");
            }
            return result;
        }

        /// <summary>
        /// Summarize parts of two matrixes <paramref name="matrix1"/> and <paramref name="matrix2"/> and returns result. Parts are specified by indexes of starting rows and cols.
        /// </summary>
        /// <remarks>
        /// If one of the matrixes is too small, then missing elements are replaces by zeroes.
        /// </remarks>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="rowsResult">Tells how many rows result Matrix will have.</param>
        /// <param name="colsResult">Tells how many columns will result Matrix have.</param>
        /// <param name="matrix1">First summand.</param>
        /// <param name="row1">Starting row in <paramref name="matrix1"/>.</param>
        /// <param name="col1">Starting column in <paramref name="matrix1"/>.</param>
        /// <param name="matrix2">Second summand.</param>
        /// <param name="row2">Starting row in <paramref name="matrix2"/>.</param>
        /// <param name="col2">Starting column in <paramref name="matrix2"/>.</param>
        /// <returns>Created instance of Matrix class.</returns>
        internal static Matrix<T> AdditionOfParts<T>(int rowsResult, int colsResult, Matrix<T> matrix1, int row1, int col1, Matrix<T> matrix2, int row2, int col2) where T : IMatrixNumber, new()
        {
            if (matrix1 == null || matrix2 == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result = Matrix<T>.GetUninitializedMatrix(rowsResult, colsResult);

            for (int i = 0; i < rowsResult; i++)
            {
                for (int j = 0; j < colsResult; j++)
                {
                    T first = new T();
                    T second = new T();
                    if (i + row1 < matrix1.Rows && j + col1 < matrix1.Cols) { first = (T)matrix1.GetNumber(i + row1, j + col1); }
                    if (i + row2 < matrix2.Rows && j + col2 < matrix2.Cols) { second = (T)matrix2.GetNumber(i + row2, j + col2); }

                    result.WriteNumber(i, j, (T)(first.__Addition(second)));
                }
            }

            return result;
        }

        /// <summary>
        /// Subtract parts of two matrixes <paramref name="matrix1"/> and <paramref name="matrix2"/> and returns result. Parts are specified by indexes of starting rows and cols.
        /// </summary>
        /// <remarks>
        /// If one of the matrixes is too small, then missing elements are replaces by zeroes.
        /// </remarks>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="rowsResult">Tells how many rows result Matrix will have.</param>
        /// <param name="colsResult">Tells how many columns will result Matrix have.</param>
        /// <param name="matrix1">Minuend.</param>
        /// <param name="row1">Starting row in <paramref name="matrix1"/>.</param>
        /// <param name="col1">Starting column in <paramref name="matrix1"/>.</param>
        /// <param name="matrix2">Subtrahend.</param>
        /// <param name="row2">Starting row in <paramref name="matrix2"/>.</param>
        /// <param name="col2">Starting column in <paramref name="matrix2"/>.</param>
        /// <returns>Created instance of Matrix class.</returns>
        internal static Matrix<T> SubtractionOfParts<T>(int rowsResult, int colsResult, Matrix<T> matrix1, int row1, int col1, Matrix<T> matrix2, int row2, int col2) where T : IMatrixNumber, new()
        {
            if (matrix1 == null || matrix2 == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result = Matrix<T>.GetUninitializedMatrix(rowsResult, colsResult);

            for (int i = 0; i < rowsResult; i++)
            {
                for (int j = 0; j < colsResult; j++)
                {
                    T first = new T();
                    T second = new T();
                    if (i + row1 < matrix1.Rows && j + col1 < matrix1.Cols) { first = (T)matrix1.GetNumber(i + row1, j + col1); }
                    if (i + row2 < matrix2.Rows && j + col2 < matrix2.Cols) { second = (T)matrix2.GetNumber(i + row2, j + col2); }

                    result.WriteNumber(i, j, (T)(first.__Subtraction(second)));
                }
            }

            return result;
        }

        /// <summary>
        /// Subtract <paramref name="matrix1"/> and <paramref name="matrix2"/> and return result.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix1">Minuend.</param>
        /// <param name="matrix2">Subtrahend.</param>
        /// <returns>Newly created Matrix with result of subtraction.</returns>
        public static Matrix<T> Subtraction<T>(Matrix<T> matrix1, Matrix<T> matrix2) where T : IMatrixNumber, new()
        {
            if (matrix1 == null || matrix2 == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

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
                        result.WriteNumber(i, j, (T)(matrix1.GetNumber(i, j).__Subtraction(matrix2.GetNumber(i, j))));
                    }
                }
            }
            else
            {
                throw new MatrixLibraryException("Rows and cols of matrixes are not equal");
            }
            return result;
        }

        /// <summary>
        /// Classic multiplication of two matrixes with slow n^3 algorithm.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix1">Multiplicant.</param>
        /// <param name="matrix2">Multiplier.</param>
        /// <returns>Matrix class with result of multiplication of <paramref name="matrix1"/> and <paramref name="matrix2"/>.</returns>
        public static Matrix<T> Multiplication<T>(Matrix<T> matrix1, Matrix<T> matrix2) where T : IMatrixNumber, new()
        {
            if (matrix1 == null || matrix2 == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

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
                            tmp = (T)((matrix1.GetNumber(i, k).__Multiplication(matrix2.GetNumber(k, j))).__Addition(tmp));
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

        /// <summary>
        /// Given <paramref name="matrix"/> will multiply with <paramref name="number"/>.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">First parameter of multiplication.</param>
        /// <param name="number">Number with which all <paramref name="matrix"/> elements will be multiplied.</param>
        /// <returns>Matrix class with multiplied elements.</returns>
        public static Matrix<T> MultiplyWithNumber<T>(Matrix<T> matrix, T number) where T : IMatrixNumber, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }
            if (number == null) { throw new MatrixLibraryException("In given number reference was null value!"); }

            int cols = matrix.Cols;
            Matrix<T> result = Matrix<T>.GetUninitializedMatrix(matrix.Rows, cols);

            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result.WriteNumber(i, j, (T)(matrix.GetNumber(i, j).__Multiplication(number)));
                }
            }

            return result;
        }

        /// <summary>
        /// Strassen-Winograd algorithm which multiplies two matrixes, <paramref name="matrix1"/> and <paramref name="matrix2"/>.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix1">Multiplicant.</param>
        /// <param name="matrix2">Multiplier.</param>
        /// <returns>Suitable newly created Matrix object.</returns>
        public static Matrix<T> StrassenWinograd<T>(Matrix<T> matrix1, Matrix<T> matrix2) where T : IMatrixNumber, new()
        {
            if (matrix1 == null || matrix2 == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

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

        /// <summary>
        /// Exponents <paramref name="matrix"/> with <paramref name="exponent"/>.
        /// </summary>
        /// <remarks>
        /// If <paramref name="tryEigenvalues"/> is set to true, then firstly it tries to exponentiate <paramref name="matrix"/> with the help of eigenvalues. If eigenvalues are not calculated in time, then algorithm will use classic multiplication.
        /// </remarks>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Matrix object which will be exponentiated.</param>
        /// <param name="exponent">Numbers of multiplication (-1) in exponentiation.</param>
        /// <param name="tryEigenvalues">True if eigenvalues will be used to calculate exponentiation, false otherwise.</param>
        /// <returns>New instance of Matrix class with exponentiated matrix.</returns>
        public static Matrix<T> Exponentiate<T>(Matrix<T> matrix, int exponent, bool tryEigenvalues = false) where T : IMatrixNumber, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result;
            if (matrix.Rows == matrix.Cols)
            {
                Matrix<T> exponentiate = null;
                Matrix<T> S = null;
                if (tryEigenvalues == true)
                {
                    try { exponentiate = CharacteristicsExtensions.Diagonal(matrix, out S, 1000); }
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

                    result = S * exponentiate * AlteringOperationsExtensions.Inverse(S);
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
