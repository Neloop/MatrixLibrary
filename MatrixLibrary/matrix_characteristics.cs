using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MatrixLibrary
{
    public static class ParallelCharacteristics
    {
        /// <summary>
        /// Pokud je limit nula, algoritmus počítá dokud vlastní čísla nenajde, určitý integer pak vyjadřuje počet opakování cyklu na zjištění podobné matice
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static EigenValues<T> GetEigenValues<T>(Matrix<T> matrix, int limit = 0) where T : MatrixNumberBase, new()
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

                    ParallelDecompositions.QRDecomposition(RQ, out Q, out R);
                    RQ = ParallelClassicOperations.Multiplication(ParallelAlteringOperations.Transposition(Q), RQ);
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
                        if (tmpRes[j] == RQ.GetNumber(i, i)) { write = false; }
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
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <param name="eigenValues"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static Matrix<T> GetEigenVectors<T>(Matrix<T> matrix, out EigenValues<T> eigenValues, int limit = 0) where T : MatrixNumberBase, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result;
            EigenValues<T> tmpEigenValues;
            if (matrix.Rows == matrix.Cols)
            {
                tmpEigenValues = GetEigenValues(matrix, limit);
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
                                    modified.WriteNumber(k, l, (T)(matrix.GetNumber(k, l) - tmpEigenValue));
                                }
                                else
                                {
                                    modified.WriteNumber(k, l, matrix.GetNumber(k, l));
                                }
                            }
                        }
                    });

                    Matrix<T> system = ParallelComputations.SolveLinearEquations(modified, zeroVector);

                    Parallel.ForEach(system.GetRowsChunks(), (pair) =>
                    {
                        for (int k = pair.Item1; k < pair.Item2; k++)
                        {
                            T sum = new T();
                            for (int l = 0; l < system.Cols; l++)
                            {
                                sum = (T)(sum + system.GetNumber(k, l));
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
        /// Pomocí vlastních čísel určí diagonální matici a vrací jí
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <param name="S"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static Matrix<T> Diagonal<T>(Matrix<T> matrix, out Matrix<T> S, int limit = 0) where T : MatrixNumberBase, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result;

            if (matrix.Rows == matrix.Cols)
            {
                EigenValues<T> eigenValues;
                S = GetEigenVectors(matrix, out eigenValues, limit);

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

    public static class Characteristics
    {
        /// <summary>
        /// Pokud je limit nula, algoritmus počítá dokud vlastní čísla nenajde, určitý integer pak vyjadřuje počet opakování cyklu na zjištění podobné matice
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static EigenValues<T> GetEigenValues<T>(Matrix<T> matrix, int limit = 0) where T : MatrixNumberBase, new()
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

                    Decompositions.QRDecomposition(RQ, out Q, out R);
                    RQ = AlteringOperations.Transposition(Q) * RQ * Q;

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
                        if (tmpRes[j] == RQ.GetNumber(i, i)) { write = false; }
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
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <param name="eigenValues"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static Matrix<T> GetEigenVectors<T>(Matrix<T> matrix, out EigenValues<T> eigenValues, int limit = 0) where T : MatrixNumberBase, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result;
            if (matrix.Rows == matrix.Cols)
            {
                eigenValues = Characteristics.GetEigenValues(matrix, limit);
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
                                modified.WriteNumber(k, l, (T)(matrix.GetNumber(k, l) - eigenValues.GetEigenValue(i)));
                            }
                            else
                            {
                                modified.WriteNumber(k, l, matrix.GetNumber(k, l));
                            }
                        }
                    }

                    system = Computations.SolveLinearEquations(modified, zeroVector);

                    for (int k = 0; k < system.Rows; k++)
                    {
                        T sum = new T();
                        for (int l = 0; l < system.Cols; l++)
                        {
                            sum = (T)(sum + system.GetNumber(k, l));
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
        /// Pomocí vlastních čísel určí diagonální matici a vrací jí
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <param name="S"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static Matrix<T> Diagonal<T>(Matrix<T> matrix, out Matrix<T> S, int limit = 0) where T : MatrixNumberBase, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            Matrix<T> result;

            if (matrix.Rows == matrix.Cols)
            {
                EigenValues<T> eigenValues;
                S = Characteristics.GetEigenVectors(matrix, out eigenValues, limit);

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
