using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MatrixLibrary
{
    /// <summary>
    /// Namespace of methods which relate to eigen values and eigen vectors. All of them use parallel execution.
    /// </summary>
    public static class ParallelCharacteristicsExtensions
    {
        /// <summary>
        /// On given <paramref name="matrix"/> tries to find eigen values. It can run until it finds them or can have a limit of iterations to find them.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Matrix on which eigen values will be found.</param>
        /// <param name="limit">If the limit is zero, then algorithm computes until it finds eigen values. If the limit is specific integer, then this integer represents number of iterations fo internal cycle.</param>
        /// <returns>Reference to EigenValues class in which result is stored.</returns>
        public static EigenValues<T> GetEigenValuesParallel<T>(this Matrix<T> matrix, int limit = 0) where T : IMatrixNumber, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            EigenValues<T> result;
            if (limit < 0) { throw new MatrixLibraryException("Given limit of executions was negative."); }

            if (matrix.Rows == matrix.Cols)
            {
                Matrix<T> Q, R, RQ = matrix;

                bool end1 = false;
                bool end2 = false;
                while (end1 == false && end2 == false)
                {
                    end1 = true;
                    end2 = true;

                    ParallelDecompositionsExtensions.QRDecompositionParallel(RQ, out Q, out R);
                    RQ = ParallelClassicOperations.Multiplication(ParallelAlteringOperationsExtensions.TranspositionParallel(Q), RQ);
                    RQ = ParallelClassicOperations.Multiplication(RQ, Q);

                    Parallel.ForEach(RQ.GetRowsChunks(1), (pair) =>
                    {
                        for (int i = pair.Item1; i < pair.Item2; i++)
                        {
                            for (int j = 0; j < i; j++)
                            {
                                if (!RQ.GetNumber(i, j).IsZero()) { end1 = false; break; }
                            }
                            if (end1 == false) { break; }
                        }
                    });
                    Parallel.ForEach(RQ.GetColsChunks(1), (pair) =>
                    {
                        for (int i = pair.Item1; i < pair.Item2; i++)
                        {
                            for (int j = 0; j < i; j++)
                            {
                                if (!RQ.GetNumber(j, i).IsZero()) { end2 = false; break; }
                            }
                            if (end2 == false) { break; }
                        }
                    });
                    
                    if (limit == 1)
                    {
                        if (end1 == false && end2 == false)
                        {
                            throw new EigenValuesNotFoundException("Cannot find eigenvalues in specified limit!");
                        }
                    }
                    if (limit != 0) { limit--; }
                }

                int zeroCol;
                List<T> tmpRes = new List<T>();
                for (int i = 0; i < RQ.Rows; i++)
                {
                    bool write = true;
                    for (int j = 0; j < tmpRes.Count; j++)
                    {
                        if (tmpRes[j].__IsEqual(RQ.GetNumber(i, i))) { write = false; }
                    }
                    if (write == true)
                    {
                        tmpRes.Add(RQ.GetNumber(i, i));
                    }

                    zeroCol = 0;
                    Parallel.ForEach(RQ.GetRowsChunks(), (pair) =>
                    {
                        for (int j = pair.Item1; j < pair.Item2; j++)
                        {
                            if (RQ.GetNumber(i, j).IsZero()) { Interlocked.Increment(ref zeroCol); }
                        }
                    });
                    
                    if (zeroCol == RQ.Cols)
                    {
                        throw new EigenValuesNotFoundException("Matrix was not regular!");
                    }
                }

                result = new EigenValues<T>(tmpRes.ToArray());
            }
            else
            {
                throw new MatrixLibraryException("Rows and cols are not equal");
            }


            return result;
        }

        /// <summary>
        /// Tries to find eigen vectors and eigen values on specified <paramref name="matrix"/>. It can run until it finds them or can have a limit of iterations.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Matrix on which eigen values and vectors will be found.</param>
        /// <param name="eigenValues">The result of GetEigenValuesParallel() function which is outputed to caller.</param>
        /// <param name="limit">This limit is passed to GetEigenValuesParallel() function.</param>
        /// <returns>Returns new instance of Matrix in which eigen vectors are stored.</returns>
        public static Matrix<T> GetEigenVectorsParallel<T>(this Matrix<T> matrix, out EigenValues<T> eigenValues, int limit = 0) where T : IMatrixNumber, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result;
            EigenValues<T> tmpEigenValues;
            if (matrix.Rows == matrix.Cols)
            {
                tmpEigenValues = GetEigenValuesParallel(matrix, limit);
                result = Matrix<T>.GetUninitializedMatrix(tmpEigenValues.Count(), matrix.Cols);
                Matrix<T> modified = Matrix<T>.GetUninitializedMatrix(matrix.Rows, matrix.Cols);
                Matrix<T> zeroVector = new Matrix<T>(matrix.Rows, 1);

                for (int i = 0; i < tmpEigenValues.Count(); i++)
                {
                    T tmpEigenValue = tmpEigenValues.GetEigenValue(i);
                    Parallel.ForEach(modified.GetRowsChunks(), (pair) =>
                    {
                        for (int k = pair.Item1; k < pair.Item2; k++)
                        {
                            for (int l = 0; l < modified.Cols; l++)
                            {
                                if (k == l)
                                {
                                    modified.WriteNumber(k, l, (T)(matrix.GetNumber(k, l).__Subtraction(tmpEigenValue)));
                                }
                                else
                                {
                                    modified.WriteNumber(k, l, matrix.GetNumber(k, l));
                                }
                            }
                        }
                    });

                    Matrix<T> system = ParallelComputationsExtensions.SolveLinearEquationsParallel(modified, zeroVector);

                    Parallel.ForEach(system.GetRowsChunks(), (pair) =>
                    {
                        for (int k = pair.Item1; k < pair.Item2; k++)
                        {
                            T sum = new T();
                            for (int l = 0; l < system.Cols; l++)
                            {
                                sum = (T)(sum.__Addition(system.GetNumber(k, l)));
                            }
                            result.WriteNumber(i, k, sum);
                        }
                    });
                }
            }
            else
            {
                throw new MatrixLibraryException("Rows and cols are not equal");
            }

            eigenValues = tmpEigenValues;
            return result;
        }

        /// <summary>
        /// Through eigen values and vectors computes diagonal matrix and returns it.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Source matrix to diagonalization process.</param>
        /// <param name="S">Output matrix of eigen vectors.</param>
        /// <param name="limit">This limit is passed to GetEigenVectorsParallel() function.</param>
        /// <returns>Diagonal matrix which consist of eigen values.</returns>
        public static Matrix<T> DiagonalParallel<T>(this Matrix<T> matrix, out Matrix<T> S, int limit = 0) where T : IMatrixNumber, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result;

            if (matrix.Rows == matrix.Cols)
            {
                EigenValues<T> eigenValues;
                S = GetEigenVectorsParallel(matrix, out eigenValues, limit);

                if (S.Rows == S.Cols)
                {
                    result = new Matrix<T>(S.Rows, S.Cols);
                    Parallel.ForEach(result.GetRowsChunks(), (pair) =>
                    {
                        for (int i = pair.Item1; i < pair.Item2; i++)
                        {
                            result.WriteNumber(i, i, eigenValues.GetEigenValue(i));
                        }
                    });
                }
                else
                {
                    throw new MatrixLibraryException("Rows and cols of EigenVector matrix are not equal");
                }
            }
            else
            {
                throw new MatrixLibraryException("Rows and cols are not equal");
            }

            return result;
        }
    }

    /// <summary>
    /// Namespace of methods which relate to eigen values and eigen vectors.
    /// </summary>
    public static class CharacteristicsExtensions
    {
        /// <summary>
        /// On given <paramref name="matrix"/> tries to find eigen values. It can run until it finds them or can have a limit of iterations to find them.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Matrix on which eigen values will be found.</param>
        /// <param name="limit">If the limit is zero, then algorithm computes until it finds eigen values. If the limit is specific integer, then this integer represents number of iterations fo internal cycle.</param>
        /// <returns>Reference to EigenValues class in which result is stored.</returns>
        public static EigenValues<T> GetEigenValues<T>(this Matrix<T> matrix, int limit = 0) where T : IMatrixNumber, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            EigenValues<T> result;
            if (limit < 0) { throw new MatrixLibraryException("Given limit of executions was negative."); }

            if (matrix.Rows == matrix.Cols)
            {
                Matrix<T> Q, R, RQ = matrix;

                bool end1 = false;
                bool end2 = false;
                while (end1 == false && end2 == false)
                {
                    end1 = true;
                    end2 = true;

                    DecompositionsExtensions.QRDecomposition(RQ, out Q, out R);
                    RQ = AlteringOperationsExtensions.Transposition(Q) * RQ * Q;

                    for (int i = 1; i < RQ.Rows; i++)
                    {
                        for (int j = 0; j < i; j++)
                        {
                            if (!RQ.GetNumber(i, j).IsZero()) { end1 = false; break; }
                        }
                        if (end1 == false) { break; }
                    }
                    for (int i = 1; i < RQ.Cols; i++)
                    {
                        for (int j = 0; j < i; j++)
                        {
                            if (!RQ.GetNumber(j, i).IsZero()) { end2 = false; break; }
                        }
                        if (end2 == false) { break; }
                    }
                    if (limit == 1)
                    {
                        if (end1 == false && end2 == false)
                        {
                            throw new EigenValuesNotFoundException("Cannot find eigenvalues in specified limit!");
                        }
                    }
                    if (limit != 0) { limit--; }
                }

                int zeroCol;
                List<T> tmpRes = new List<T>();
                for (int i = 0; i < RQ.Rows; i++)
                {
                    bool write = true;
                    for (int j = 0; j < tmpRes.Count; j++)
                    {
                        if (tmpRes[j].__IsEqual(RQ.GetNumber(i, i))) { write = false; }
                    }
                    if (write == true)
                    {
                        tmpRes.Add(RQ.GetNumber(i, i));
                    }

                    zeroCol = 0;
                    for (int j = 0; j < RQ.Cols; j++)
                    {
                        if (RQ.GetNumber(i, j).IsZero()) { zeroCol++; }
                    }
                    if (zeroCol == RQ.Cols)
                    {
                        throw new EigenValuesNotFoundException("Matrix was not regular!");
                    }
                }

                result = new EigenValues<T>(tmpRes.ToArray());
            }
            else
            {
                throw new MatrixLibraryException("Rows and cols are not equal");
            }


            return result;
        }

        /// <summary>
        /// Tries to find eigen vectors and eigen values on specified <paramref name="matrix"/>. It can run until it finds them or can have a limit of iterations.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Matrix on which eigen values and vectors will be found.</param>
        /// <param name="eigenValues">The result of GetEigenValuesParallel() function which is outputed to caller.</param>
        /// <param name="limit">This limit is passed to GetEigenValuesParallel() function.</param>
        /// <returns>Returns new instance of Matrix in which eigen vectors are stored.</returns>
        public static Matrix<T> GetEigenVectors<T>(this Matrix<T> matrix, out EigenValues<T> eigenValues, int limit = 0) where T : IMatrixNumber, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result;
            if (matrix.Rows == matrix.Cols)
            {
                eigenValues = CharacteristicsExtensions.GetEigenValues(matrix, limit);
                result = Matrix<T>.GetUninitializedMatrix(eigenValues.Count(), matrix.Cols);
                Matrix<T> modified = Matrix<T>.GetUninitializedMatrix(matrix.Rows, matrix.Cols);
                Matrix<T> zeroVector = new Matrix<T>(matrix.Rows, 1);
                Matrix<T> system;

                for (int i = 0; i < eigenValues.Count(); i++)
                {
                    for (int k = 0; k < modified.Rows; k++)
                    {
                        for (int l = 0; l < modified.Cols; l++)
                        {
                            if (k == l)
                            {
                                modified.WriteNumber(k, l, (T)(matrix.GetNumber(k, l).__Subtraction(eigenValues.GetEigenValue(i))));
                            }
                            else
                            {
                                modified.WriteNumber(k, l, matrix.GetNumber(k, l));
                            }
                        }
                    }

                    system = ComputationsExtensions.SolveLinearEquations(modified, zeroVector);

                    for (int k = 0; k < system.Rows; k++)
                    {
                        T sum = new T();
                        for (int l = 0; l < system.Cols; l++)
                        {
                            sum = (T)(sum.__Addition(system.GetNumber(k, l)));
                        }
                        result.WriteNumber(i, k, sum);
                    }
                }
            }
            else
            {
                throw new MatrixLibraryException("Rows and cols are not equal");
            }

            return result;
        }

        /// <summary>
        /// Through eigen values and vectors computes diagonal matrix and returns it.
        /// </summary>
        /// <typeparam name="T">Type of numbers which are be stored in Matrix. Must fulfill IMatrixNumber interface and have parametresless constructor.</typeparam>
        /// <param name="matrix">Source matrix to diagonalization process.</param>
        /// <param name="S">Output matrix of eigen vectors.</param>
        /// <param name="limit">This limit is passed to GetEigenVectorsParallel() function.</param>
        /// <returns>Diagonal matrix which consist of eigen values.</returns>
        public static Matrix<T> Diagonal<T>(this Matrix<T> matrix, out Matrix<T> S, int limit = 0) where T : IMatrixNumber, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result;

            if (matrix.Rows == matrix.Cols)
            {
                EigenValues<T> eigenValues;
                S = CharacteristicsExtensions.GetEigenVectors(matrix, out eigenValues, limit);

                if (S.Rows == S.Cols)
                {
                    result = new Matrix<T>(S.Rows, S.Cols);
                    for (int i = 0; i < eigenValues.Count(); i++)
                    {
                        result.WriteNumber(i, i, eigenValues.GetEigenValue(i));
                    }
                }
                else
                {
                    throw new MatrixLibraryException("Rows and cols of EigenVector matrix are not equal");
                }
            }
            else
            {
                throw new MatrixLibraryException("Rows and cols are not equal");
            }

            return result;
        }
    }
}
