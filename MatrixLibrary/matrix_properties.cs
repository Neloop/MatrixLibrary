﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLibrary
{
    public enum DefinityClassification { Indefinite, PositiveDefinite, NegativeDefinite };

    public static class ParallelProperties
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static bool IsInvertible<T>(Matrix<T> matrix) where T : MatrixNumberBase, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            bool result = true;

            if (matrix.Rows == matrix.Cols)
            {
                int rows = matrix.Rows;
                int cols = matrix.Cols;

                Matrix<T> temporaryM = ParallelAlteringOperations.Gauss(matrix);
                Parallel.ForEach(temporaryM.GetRowsChunks(), (pair, loopState) =>
                {
                    for (int i = pair.Item1; i < pair.Item2; i++)
                    {
                        int zeroes = 0;

                        for (int j = 0; j < cols; j++)
                        {
                            if (!temporaryM.GetNumber(i, j).IsZero()) { break; }
                            else { zeroes++; }
                        }
                        if (zeroes == cols) { result = false; loopState.Stop(); }
                    }
                });
            }
            else
            {
                return false;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static int Rank<T>(Matrix<T> matrix) where T : MatrixNumberBase, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            object resultLock = new object();
            int result = matrix.Rows;

            Matrix<T> gauss = ParallelAlteringOperations.Gauss(matrix);

            Parallel.ForEach(gauss.GetRowsChunks(), (pair) =>
            {
                for (int i = pair.Item1; i < pair.Item2; i++)
                {
                    int zeroes = 0;
                    for (int j = 0; j < gauss.Cols; j++)
                    {
                        if (gauss.GetNumber(i, j).IsZero()) { zeroes++; }
                    }
                    if (zeroes == gauss.Cols) { lock (resultLock) { result--; } }
                }
            });

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static bool IsOrthogonal<T>(Matrix<T> matrix) where T : MatrixNumberBase, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            bool result = true;

            if (matrix.Rows == matrix.Cols)
            {
                Matrix<T> transposition = ParallelAlteringOperations.Transposition(matrix);
                Matrix<T> multiplied = ParallelClassicOperations.Multiplication(transposition, matrix);

                Parallel.ForEach(multiplied.GetRowsChunks(), (pair, loopState) =>
                {
                    for (int i = pair.Item1; i < pair.Item2; i++)
                    {
                        for (int j = 0; j < multiplied.Cols; j++)
                        {
                            if (i == j)
                            {
                                if (!multiplied.GetNumber(i, j).IsOne()) { result = false; loopState.Stop(); }
                            }
                            else
                            {
                                if (!multiplied.GetNumber(i, j).IsZero()) { result = false; loopState.Stop(); }
                            }
                        }
                    }
                });
            }
            else { return false; }

            return result;
        }

        /// <summary>
        /// Rozlišuje se definitnost (pozitivní/negativní), indefinitnost; podle vráceného čísla
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static DefinityClassification Definity<T>(Matrix<T> matrix) where T : MatrixNumberBase, new()
        {
            /*
             * Neurčuje semi-definitnost (pozitivní/negativní)
             * Využívá se Sylvestrovo kriterium
             * Pokud je vrácena 
             * 0: Indefinitní
             * 1: Pozitivně definitní
             * 2: Negatině definitní
             * 
             * */

            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            DefinityClassification result = DefinityClassification.Indefinite;

            if (matrix.Rows == matrix.Cols)
            {
                int rows = matrix.Rows;
                T[] determinant = new T[rows];

                Parallel.ForEach(matrix.GetRowsChunks(), (pair) =>
                {
                    for (int i = pair.Item1; i < pair.Item2; i++)
                    {
                        Matrix<T> det = Matrix<T>.GetUninitializedMatrix(i + 1, i + 1);
                        for (int k = 0; k < (i + 1); k++)
                        {
                            for (int l = 0; l < (i + 1); l++)
                            {
                                det.WriteNumber(k, l, matrix.GetNumber(k, l));
                            }
                        }
                        determinant[i] = Computations.Determinant(det);
                    }
                });

                object positiveLock = new object();
                int positive = 0;
                bool negative = true;
                T zero = new T();
                Parallel.ForEach(matrix.GetRowsChunks(), (pair) =>
                {
                    for (int i = pair.Item1; i < pair.Item2; i++)
                    {
                        if (determinant[i] > zero) { lock (positiveLock) { positive++; } }
                        if ((i % 2) == 0 && determinant[i] >= zero) { negative = false; }
                        if ((i % 2) == 1 && determinant[i] <= zero) { negative = false; }
                    }
                });

                if (positive == rows) { result = DefinityClassification.PositiveDefinite; }
                else if (negative == true) { result = DefinityClassification.NegativeDefinite; }
            }

            return result;
        }
    }

    public static class Properties
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static bool IsInvertible<T>(Matrix<T> matrix) where T : MatrixNumberBase, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            if (matrix.Rows == matrix.Cols)
            {
                int rows = matrix.Rows;
                int cols = matrix.Cols;

                Matrix<T> temporaryM = AlteringOperations.Gauss(matrix);
                for (int i = 0; i < rows; i++)
                {
                    int zeroes = 0;

                    for (int j = 0; j < cols; j++)
                    {
                        if (!temporaryM.GetNumber(i, j).IsZero()) { break; }
                        else { zeroes++; }
                    }
                    if (zeroes == cols) { return false; }
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static int Rank<T>(Matrix<T> matrix) where T : MatrixNumberBase, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            int result = matrix.Rows;

            Matrix<T> gauss = AlteringOperations.Gauss(matrix);

            for (int i = 0; i < gauss.Rows; i++)
            {
                int zeroes = 0;
                for (int j = 0; j < gauss.Cols; j++)
                {
                    if (gauss.GetNumber(i, j).IsZero()) { zeroes++; }
                }
                if (zeroes == gauss.Cols) { result--; }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static bool IsOrthogonal<T>(Matrix<T> matrix) where T : MatrixNumberBase, new()
        {
            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            if (matrix.Rows == matrix.Cols)
            {
                Matrix<T> transposition = AlteringOperations.Transposition(matrix);
                Matrix<T> multiplied = ClassicOperations.Multiplication(transposition, matrix);

                for (int i = 0; i < multiplied.Rows; i++)
                {
                    for (int j = 0; j < multiplied.Cols; j++)
                    {
                        if (i == j)
                        {
                            if (!multiplied.GetNumber(i, j).IsOne()) { return false; }
                        }
                        else
                        {
                            if (!multiplied.GetNumber(i, j).IsZero()) { return false; }
                        }
                    }
                }
            }
            else { return false; }

            return true;
        }
        
        /// <summary>
        /// Rozlišuje se definitnost (pozitivní/negativní), indefinitnost; podle vráceného čísla
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static DefinityClassification Definity<T>(Matrix<T> matrix) where T : MatrixNumberBase, new()
        {
            /*
             * Neurčuje semi-definitnost (pozitivní/negativní)
             * Využívá se Sylvestrovo kriterium
             * Pokud je vrácena 
             * 0: Indefinitní
             * 1: Pozitivně definitní
             * 2: Negatině definitní
             * 
             * */

            if (matrix == null) { throw new MatrixLibraryException("In given matrix reference was null value!"); }

            DefinityClassification result = DefinityClassification.Indefinite;

            if (matrix.Rows == matrix.Cols)
            {
                int rows = matrix.Rows;
                T[] determinant = new T[rows];

                for (int i = 0; i < rows; i++)
                {
                    Matrix<T> det = Matrix<T>.GetUninitializedMatrix(i + 1, i + 1);
                    for (int k = 0; k < (i + 1); k++)
                    {
                        for (int l = 0; l < (i + 1); l++)
                        {
                            det.WriteNumber(k, l, matrix.GetNumber(k, l));
                        }
                    }
                    determinant[i] = Computations.Determinant(det);
                }

                int positive = 0;
                bool negative = true;
                T zero = new T();
                for (int i = 0; i < rows; i++)
                {
                    if (determinant[i] > zero) { positive++; }
                    if ((i % 2) == 0 && determinant[i] >= zero) { negative = false; }
                    if ((i % 2) == 1 && determinant[i] <= zero) { negative = false; }
                }

                if (positive == rows) { result = DefinityClassification.PositiveDefinite; }
                else if (negative == true) { result = DefinityClassification.NegativeDefinite; }
            }

            return result;
        }
    }
}
