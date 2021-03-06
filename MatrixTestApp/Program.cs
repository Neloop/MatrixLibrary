﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using MatrixLibrary;

namespace MatrixTestApp
{
    class Program
    {
        static Random RandomGenerator = new Random();

        static void WriteTwoMatrixes<T>(Matrix<T> matrix1, Matrix<T> matrix2, Matrix<T> result, string operationName) where T : IMatrixNumber, new()
        {
            int maxRows = Math.Max(Math.Max(matrix1.Rows, matrix2.Rows), result.Rows);
            int half = maxRows / 2;

            for (int i = 0; i < maxRows; i++)
            {
                if (matrix1.Rows > i)
                {
                    for (int j = 0; j < matrix1.Cols; j++) // first matrix
                    {
                        T tmp = matrix1.GetNumber(i, j);
                        Console.Write("{0,4} ", tmp.ToDouble());
                    }
                }
                else { for (int j = 0; j < matrix1.Cols; j++) { Console.Write("     "); } }

                if (i == half) { Console.Write(" {0} ", operationName); } // operation
                else { Console.Write("   "); }

                if (matrix2.Rows > i)
                {
                    for (int j = 0; j < matrix2.Cols; j++) // second matrix
                    {
                        T tmp = matrix2.GetNumber(i, j);
                        Console.Write("{0,4} ", tmp.ToDouble());
                    }
                }
                else { for (int j = 0; j < matrix2.Cols; j++) { Console.Write("     "); } }

                if (i == half) { Console.Write(" = "); }
                else { Console.Write("   "); }

                if (result.Rows > i)
                {
                    for (int j = 0; j < result.Cols; j++) // result of matrix
                    {
                        T tmp = result.GetNumber(i, j);
                        Console.Write("{0,4} ", tmp.ToDouble());
                    }
                }
                else { for (int j = 0; j < result.Cols; j++) { Console.Write("      "); } }
                Console.WriteLine();
            }
        }
        static void WriteMatrix<T>(Matrix<T> matrix, Matrix<T> result, string operationName) where T : IMatrixNumber, new()
        {
            int maxRows = Math.Max(matrix.Rows, result.Rows);
            int half = maxRows / 2;

            for (int i = 0; i < maxRows; i++)
            {
                if (matrix.Rows > i)
                {
                    for (int j = 0; j < matrix.Cols; j++)
                    {
                        T tmp = matrix.GetNumber(i, j);
                        Console.Write("{0,4} ", tmp.ToDouble());
                    }
                }
                else { for (int j = 0; j < matrix.Cols; j++) { Console.Write("      "); } }

                if (i == half) { Console.Write(" {0} ", operationName); }
                else { for (int j = 0; j < operationName.Length; j++) { Console.Write(" "); } Console.Write("  "); }

                if (result.Rows > i)
                {
                    for (int j = 0; j < result.Cols; j++)
                    {
                        T tmp = result.GetNumber(i, j);
                        Console.Write("{0,4} ", tmp.ToDouble());
                    }
                }
                else { for (int j = 0; j < result.Cols; j++) { Console.Write("      "); } }

                Console.WriteLine();
            }
        }

        static void WriteSeparator(string message = "")
        {
            Console.WriteLine();
            for (int i = 0; i < (50 - (message.Length / 2)); i++)
            {
                Console.Write("-");
            }
            Console.Write(message);
            for (int i = 0; i < (50 - (message.Length / 2)); i++)
            {
                Console.Write("-");
            }
            Console.WriteLine();
        }


        static void ExitOrContinue()
        {
            ConsoleKeyInfo cki;
            do
            {
                Console.WriteLine();
                Console.WriteLine("Press ESC to exit or ENTER to continue calculations...");
                cki = Console.ReadKey();

                if (cki.Key == ConsoleKey.Escape) { Environment.Exit(0); }
            }
            while (cki.Key != ConsoleKey.Enter);
        }

        static void ColoredWriteLine(string s)
        {
            ConsoleColor oldForegroundColor = Console.ForegroundColor;
            ConsoleColor oldBackgroundColor = Console.BackgroundColor;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(s);
            Console.ForegroundColor = oldForegroundColor;
            Console.BackgroundColor = oldBackgroundColor;
        }


        static void GenerateMatrixes(int rowsAndCols, out Matrix<MatrixNumber> A, out Matrix<MatrixNumber> B, out Matrix<MatrixNumber> b) 
        {
            Console.WriteLine("Matrixes will have {0} rows and cols", rowsAndCols);
            Console.WriteLine("Generating matrixes...");
            A = Matrix<MatrixNumber>.GetUninitializedMatrix(rowsAndCols, rowsAndCols);
            B = Matrix<MatrixNumber>.GetUninitializedMatrix(rowsAndCols, rowsAndCols);
            b = Matrix<MatrixNumber>.GetUninitializedMatrix(rowsAndCols, 1);

            for (int i = 0; i < rowsAndCols; i++)
            {
                b.WriteNumber(i, 0, new MatrixNumber(RandomGenerator.Next(-10, 10)));
                for (int j = 0; j < rowsAndCols; j++)
                {
                    A.WriteNumber(i, j, new MatrixNumber(RandomGenerator.Next(-10, 10)));
                    B.WriteNumber(i, j, new MatrixNumber(RandomGenerator.Next(-10, 10)));
                }
            }

            WriteSeparator();
        }

        delegate Matrix<MatrixNumber> MatrixBinaryOp(Matrix<MatrixNumber> A, Matrix<MatrixNumber> B);
        static void DoBinaryOp(Matrix<MatrixNumber> A, Matrix<MatrixNumber> B, MatrixBinaryOp singleOp, MatrixBinaryOp multiOp, string opName)
        {
            Stopwatch stopwatchSingle = new Stopwatch();
            Stopwatch stopwatchMulti = new Stopwatch();
            bool exceptCaughtSingle = false;
            bool exceptCaughtMulti = false;

            Console.WriteLine(opName + "...");
            stopwatchSingle.Start();
            Matrix<MatrixNumber> resultSingle = null;
            try { resultSingle = singleOp(A, B); }
            catch (MatrixLibraryException e) { Console.WriteLine(e.Message); exceptCaughtSingle = true; }
            stopwatchSingle.Stop();

            Console.WriteLine("Multi-threaded " + opName + "...");
            stopwatchMulti.Start();
            Matrix<MatrixNumber> resultMulti = null;
            try { resultMulti = multiOp(A, B); }
            catch (MatrixLibraryException e) { Console.WriteLine(e.Message); exceptCaughtMulti = true; }
            stopwatchMulti.Stop();

            if ((exceptCaughtSingle == false && exceptCaughtMulti == false && resultSingle == resultMulti)
                || (exceptCaughtSingle == true && exceptCaughtMulti == true)) { Console.WriteLine("Results are the same."); }
            else { ColoredWriteLine("Results are different!"); }
            Console.WriteLine("Single-threaded: {0}; Multi-threaded: {1}", stopwatchSingle.Elapsed, stopwatchMulti.Elapsed);

            WriteSeparator();
        }

        static void DoUnaryOpInternal<resultT>(Matrix<MatrixNumber> A, 
            Func<Matrix<MatrixNumber>, resultT> singleOp, out resultT resultSingle, out bool exceptCaughtSingle, out Stopwatch stopwatchSingle,
            Func<Matrix<MatrixNumber>, resultT> multiOp, out resultT resultMulti, out bool exceptCaughtMulti, out Stopwatch stopwatchMulti, string opName)
        {
            stopwatchSingle = new Stopwatch();
            stopwatchMulti = new Stopwatch();
            exceptCaughtSingle = false;
            exceptCaughtMulti = false;

            Console.WriteLine(opName + "...");
            stopwatchSingle.Start();
            resultSingle = default(resultT);
            try { resultSingle = singleOp(A); }
            catch (MatrixLibraryException e) { Console.WriteLine(e.Message); exceptCaughtSingle = true; }
            stopwatchSingle.Stop();

            Console.WriteLine("Multi-threaded " + opName + "...");
            stopwatchMulti.Start();
            resultMulti = default(resultT);
            try { resultMulti = multiOp(A); }
            catch (MatrixLibraryException e) { Console.WriteLine(e.Message); exceptCaughtMulti = true; }
            stopwatchMulti.Stop();
        }

        static void DoUnaryOpBool(Matrix<MatrixNumber> A, Func<Matrix<MatrixNumber>, bool> singleOp, Func<Matrix<MatrixNumber>, bool> multiOp, string opName)
        {
            Stopwatch stopwatchSingle;
            Stopwatch stopwatchMulti;
            bool exceptCaughtSingle;
            bool exceptCaughtMulti;
            bool resultSingle;
            bool resultMulti;
            DoUnaryOpInternal<bool>(A, singleOp, out resultSingle, out exceptCaughtSingle, out stopwatchSingle,
                multiOp, out resultMulti, out exceptCaughtMulti, out stopwatchMulti, opName);

            if ((exceptCaughtSingle == false && exceptCaughtMulti == false && resultSingle == resultMulti)
                || (exceptCaughtSingle == true && exceptCaughtMulti == true)) { Console.WriteLine("Results are the same."); }
            else { ColoredWriteLine("Results are different!"); }
            Console.WriteLine("Single-threaded: {0}; Multi-threaded: {1}", stopwatchSingle.Elapsed, stopwatchMulti.Elapsed);

            WriteSeparator();
        }

        static void DoUnaryOpMatrixNumber(Matrix<MatrixNumber> A, Func<Matrix<MatrixNumber>, MatrixNumber> singleOp, Func<Matrix<MatrixNumber>, MatrixNumber> multiOp, string opName)
        {
            Stopwatch stopwatchSingle;
            Stopwatch stopwatchMulti;
            bool exceptCaughtSingle;
            bool exceptCaughtMulti;
            MatrixNumber resultSingle;
            MatrixNumber resultMulti;

            DoUnaryOpInternal<MatrixNumber>(A, singleOp, out resultSingle, out exceptCaughtSingle, out stopwatchSingle,
                multiOp, out resultMulti, out exceptCaughtMulti, out stopwatchMulti, opName);

            if ((exceptCaughtSingle == false && exceptCaughtMulti == false && resultSingle == resultMulti)
                || (exceptCaughtSingle == true && exceptCaughtMulti == true)) { Console.WriteLine("Results are the same."); }
            else { ColoredWriteLine("Results are different!"); }
            Console.WriteLine("Single-threaded: {0}; Multi-threaded: {1}", stopwatchSingle.Elapsed, stopwatchMulti.Elapsed);

            WriteSeparator();
        }

        static void DoUnaryOpMatrix(Matrix<MatrixNumber> A, Func<Matrix<MatrixNumber>, Matrix<MatrixNumber>> singleOp, Func<Matrix<MatrixNumber>, Matrix<MatrixNumber>> multiOp, string opName)
        {
            Stopwatch stopwatchSingle;
            Stopwatch stopwatchMulti;
            bool exceptCaughtSingle;
            bool exceptCaughtMulti;
            Matrix<MatrixNumber> resultSingle;
            Matrix<MatrixNumber> resultMulti;

            DoUnaryOpInternal<Matrix<MatrixNumber>>(A, singleOp, out resultSingle, out exceptCaughtSingle, out stopwatchSingle,
                multiOp, out resultMulti, out exceptCaughtMulti, out stopwatchMulti, opName);

            if ((exceptCaughtSingle == false && exceptCaughtMulti == false && resultSingle == resultMulti)
                || (exceptCaughtSingle == true && exceptCaughtMulti == true)) { Console.WriteLine("Results are the same."); }
            else { ColoredWriteLine("Results are different!"); }
            Console.WriteLine("Single-threaded: {0}; Multi-threaded: {1}", stopwatchSingle.Elapsed, stopwatchMulti.Elapsed);

            WriteSeparator();
        }

        static void Main(string[] args)
        {
            Matrix<MatrixNumber> A = new Matrix<MatrixNumber>(1, 1);
            /******************BUILD ALL THINGS AT FIRST***********************/
            ClassicOperations.Addition(A, A); ParallelClassicOperations.AdditionParallel(A, A);
            ClassicOperations.Subtraction(A, A); ParallelClassicOperations.SubtractionParallel(A, A);
            ClassicOperations.MultiplyWithNumber(A, new MatrixNumber()); ParallelClassicOperations.MultiplyWithNumberParallel(A, new MatrixNumber());
            ClassicOperations.Multiplication(A, A); ParallelClassicOperations.MultiplicationParallel(A, A);
            AlteringOperationsExtensions.Adjugate(A); ParallelAlteringOperationsExtensions.AdjugateParallel(A);
            AlteringOperationsExtensions.Gauss(A); ParallelAlteringOperationsExtensions.GaussParallel(A);
            ComputationsExtensions.Cramer(A, A); ParallelComputationsExtensions.CramerParallel(A, A);


            int[,] inputMatrix = new int[4, 4] { {1, 0, 5, 6}, {1, 1, 6, 7}, {0, 1, 7, 8}, {10, 7, 8, 3} };

            A = new Matrix<MatrixNumber>(inputMatrix);

            inputMatrix = new int[4, 4] { {1, 1, -2, 3}, {4, 3, -1, 5}, {1, 0, 5, -4}, {2, 1, 3, -1} };

            Matrix<MatrixNumber> B = new Matrix<MatrixNumber>(inputMatrix);

            int rowsAndCols;
            Matrix<MatrixNumber> resultSingle;
            Matrix<MatrixNumber> resultMulti;
            Stopwatch stopwatchSingle = new Stopwatch();
            Stopwatch stopwatchMulti = new Stopwatch(); ;

            resultSingle = ClassicOperations.Multiplication(A, B);
            WriteTwoMatrixes(A, B, resultSingle, "*");

            Console.WriteLine();

            Console.WriteLine("Strassen-Winograd");
            resultSingle = ClassicOperations.StrassenWinograd(A, B);
            WriteTwoMatrixes(A, B, resultSingle, "*");

            Console.WriteLine();

            /********** Addition **********/
            resultSingle = ClassicOperations.Addition(A, B);
            WriteTwoMatrixes(A, B, resultSingle, "+");

            Console.WriteLine();

            /********** Subtraction **********/
            resultSingle = ClassicOperations.Subtraction(A, B);
            WriteTwoMatrixes(A, B, resultSingle, "-");

            WriteSeparator();

            inputMatrix = new int[4, 4] { {1, 1, -1, -1}, {2, -1, 1, 2}, {1, 2, -1, 1}, {-1, 1, 1, -1} };
            Matrix<MatrixNumber> cram = new Matrix<MatrixNumber>(inputMatrix);

            inputMatrix = new int[4, 1] {{0}, {1}, {5}, {4}};
            Matrix<MatrixNumber> b = new Matrix<MatrixNumber>(inputMatrix);

            Console.WriteLine("Cramer");
            resultSingle = ComputationsExtensions.Cramer(cram, b);
            WriteTwoMatrixes(cram, b, resultSingle, "=");

            Console.WriteLine();

            inputMatrix = new int[4, 1] {{2}, {5}, {-1}, {1}};
            b = new Matrix<MatrixNumber>(inputMatrix);

            Console.WriteLine("LinearEquations");
            resultSingle = ComputationsExtensions.SolveLinearEquations(B, b);
            //try { resultSingle = Computations.SolveLinearEquations(B, b); }
            //catch (MatrixLibraryException) { resultSingle = new Matrix<MatrixNumber>(1, 1); }
            WriteTwoMatrixes(B, b, resultSingle, "=");

            Console.WriteLine();

            Console.WriteLine("MultiplyWithNumber");
            MatrixNumber multiply = new MatrixNumber(2, 1);
            int[,] zeroM = new int[1, 1];
            zeroM[0, 0] = 2;
            Matrix<MatrixNumber> multiplyM = new Matrix<MatrixNumber>(zeroM);
            resultSingle = ClassicOperations.MultiplyWithNumber(A, multiply);
            WriteTwoMatrixes(multiplyM, A, resultSingle, "*");

            Console.WriteLine();

            /*
             * 
             * Operations with one matrix
             * 
             * 
             */

            inputMatrix = new int[3, 3] { { 1, 0, 5 }, { 1, 1, 6 }, { 0, 1, 7 } };
            //inputMatrix = new int[4, 4] {{1, 0, 5, 6}, {1, 1, 6, 7}, {0, 1, 7, 8}, {10, 7, 8, 3}};

            A = new Matrix<MatrixNumber>(inputMatrix);

            WriteSeparator();

            resultSingle = A.Transposition();
            WriteMatrix(A, resultSingle, "Transposition");

            Console.WriteLine();

            MatrixNumber det = ComputationsExtensions.Determinant(B);
            zeroM = new int[1, 1];
            resultSingle = new Matrix<MatrixNumber>(zeroM);
            resultSingle.WriteNumber(0, 0, det);
            WriteMatrix(B, resultSingle, "Determinant");

            Console.WriteLine();

            inputMatrix = new int[2, 2] {{0, 5}, {1, 7}};

            Matrix<MatrixNumber> C = new Matrix<MatrixNumber>(inputMatrix);

            det = ComputationsExtensions.Determinant(C);
            resultSingle = new Matrix<MatrixNumber>(zeroM);
            resultSingle.WriteNumber(0, 0, det);
            WriteMatrix(C, resultSingle, "Determinant");

            Console.WriteLine();

            resultSingle = AlteringOperationsExtensions.Gauss(B);
            WriteMatrix(B, resultSingle, "Gauss");

            Console.WriteLine();

            resultSingle = AlteringOperationsExtensions.GaussJordan(B);
            WriteMatrix(B, resultSingle, "GaussJordan");

            Console.WriteLine();

            resultSingle = AlteringOperationsExtensions.Adjugate(A);
            WriteMatrix(A, resultSingle, "Adjugate");

            Console.WriteLine();

            bool regular = PropertiesExtensions.IsInvertible(B);
            int[,] number = new int[1, 1];
            resultSingle = new Matrix<MatrixNumber>(number);
            MatrixNumber one = new MatrixNumber(1, 1);
            MatrixNumber zero = new MatrixNumber(0, 0);
            if (regular == true) { resultSingle.WriteNumber(0, 0, one); }
            else { resultSingle.WriteNumber(0, 0, zero); }
            WriteMatrix(B, resultSingle, "Regular");

            Console.WriteLine();
            inputMatrix = new int[4, 4];
            Matrix<MatrixNumber> D = new Matrix<MatrixNumber>(inputMatrix);
            regular = PropertiesExtensions.IsInvertible(D);
            number = new int[1, 1];
            resultSingle = new Matrix<MatrixNumber>(number);
            if (regular == true) { resultSingle.WriteNumber(0, 0, one); }
            else { resultSingle.WriteNumber(0, 0, zero); }
            WriteMatrix(D, resultSingle, "IsRegular");

            Console.WriteLine();

            bool orthogonal = PropertiesExtensions.IsOrthogonal(B);
            number = new int[1, 1];
            resultSingle = new Matrix<MatrixNumber>(number);
            if (orthogonal == true) { resultSingle.WriteNumber(0, 0, one); }
            else { resultSingle.WriteNumber(0, 0, zero); }
            WriteMatrix(B, resultSingle, "IsOrthogonal");

            Console.WriteLine();

            try { resultSingle = AlteringOperationsExtensions.Inverse(A); }
            catch { resultSingle = new Matrix<MatrixNumber>(1, 1); }
            WriteMatrix(A, resultSingle, "Inverse");

            Console.WriteLine();

            try { resultSingle = AlteringOperationsExtensions.Symmetric(A); }
            catch { resultSingle = new Matrix<MatrixNumber>(1, 1); }
            WriteMatrix(A, resultSingle, "Symmetric");

            Console.WriteLine();

            resultSingle = AlteringOperationsExtensions.Orthogonal(B);
            WriteMatrix(B, resultSingle, "Orthogonal");

            Console.WriteLine();

            try { resultSingle = DecompositionsExtensions.CholeskyDecomposition(A); }
            catch { resultSingle = new Matrix<MatrixNumber>(1, 1); }
            finally { WriteMatrix(A, resultSingle, "CholeskyDecomposition"); }

            Console.WriteLine();

            Matrix<MatrixNumber> Q, R;
            resultSingle = DecompositionsExtensions.QRDecomposition(A, out Q, out R);
            WriteMatrix(A, resultSingle, "QRDecomposition");

            Console.WriteLine();

            zeroM = new int[2, 2] {{2, 1}, {0, 1}};
            A = new Matrix<MatrixNumber>(zeroM);

            EigenValues<MatrixNumber> tmp;
            try { tmp = CharacteristicsExtensions.GetEigenValues(A, 0); }
            catch { tmp = new EigenValues<MatrixNumber>(new [] { new MatrixNumber(0) }); }
            resultSingle = new Matrix<MatrixNumber>(tmp.Count(), 1);
            for (int i = 0; i < resultSingle.Rows; i++)
            {
                resultSingle.WriteNumber(i, 0, tmp.GetEigenValue(i));
            }
            WriteMatrix(A, resultSingle, "EigenValues");

            Console.WriteLine();

            try { resultSingle = CharacteristicsExtensions.GetEigenVectors(A, out tmp, 0); }
            catch { resultSingle = new Matrix<MatrixNumber>(1, 1); }
            WriteMatrix(A, resultSingle, "EigenVectors");

            Console.WriteLine();

            Matrix<MatrixNumber> S;
            try { resultSingle = CharacteristicsExtensions.Diagonal(A, out S, 0); }
            catch { resultSingle = new Matrix<MatrixNumber>(1, 1); }
            WriteMatrix(A, resultSingle, "Diagonal");

            Console.WriteLine();

            resultSingle = ClassicOperations.Exponentiate(A, 4);
            WriteMatrix(A, resultSingle, "Exponentiate^4");

            Console.WriteLine();
            WriteSeparator();





            /**********************************************************************************/
            /***************************** ARRAYS PARALIZATION ********************************/
            /**********************************************************************************/
            ExitOrContinue();
            WriteSeparator(); WriteSeparator("ARRAYS PARALIZATION");
            rowsAndCols = 23000;
            Console.WriteLine("Array will have {0} rows and cols", rowsAndCols);
            Console.WriteLine("Generating array...");
            int [,] AA = new int[rowsAndCols, rowsAndCols];
            int [,] BB = new int[rowsAndCols, rowsAndCols];
            for (int i = 0; i < rowsAndCols; ++i)
            {
                for (int j = 0; j < rowsAndCols; ++j)
                {
                    AA[i, j] = RandomGenerator.Next(-10, 10);
                    BB[i, j] = RandomGenerator.Next(-10, 10); ;
                }
            }

            Console.WriteLine("Single-threaded multidimensional array addition...");
            stopwatchSingle.Restart();
            for (int i = 0; i < rowsAndCols; i++)
            {
                for (int j = 0; j < rowsAndCols; ++j)
                {
                    AA[i, j] = AA[i, j] + BB[i, j];
                }
            }
            stopwatchSingle.Stop();

            Console.WriteLine("Multi-threaded multidimensional array addition...");
            stopwatchMulti.Restart();
            Parallel.For(0, rowsAndCols, (i) =>
            {
                for (int j = 0; j < rowsAndCols; ++j)
                {
                    AA[i, j] = AA[i, j] + BB[i, j];
                }
            });
            stopwatchMulti.Stop();

            Console.WriteLine("Single-threaded: {0}; Multi-threaded: {1}", stopwatchSingle.Elapsed, stopwatchMulti.Elapsed);

            WriteSeparator();






            /**********************************************************************************/
            /**********************************************************************************/
            /********************************* Big matrixes ***********************************/
            /**********************************************************************************/
            /**********************************************************************************/
            ExitOrContinue();
            WriteSeparator(); WriteSeparator("BIG MATRIXES");
            int resultSingleInt;
            int resultMultiInt;
            bool exceptCaughtSingle = false;
            bool exceptCaughtMulti = false;
            EigenValues<MatrixNumber> resultSingleEigen = null;
            EigenValues<MatrixNumber> resultMultiEigen = null;
            GenerateMatrixes(5000, out A, out B, out b);


            /********** Addition **********/
            DoBinaryOp(A, B, ClassicOperations.Addition, ParallelClassicOperations.AdditionParallel, "Addition");


            /********** Subtraction **********/
            DoBinaryOp(A, B, ClassicOperations.Subtraction, ParallelClassicOperations.SubtractionParallel, "Subtraction");


            /********** MultiplyWithNumber **********/
            MatrixNumber multiplyNumber = new MatrixNumber(100);
            Console.WriteLine("MultiplyWithNumber...");
            stopwatchSingle.Restart();
            resultSingle = ClassicOperations.MultiplyWithNumber(A, multiplyNumber);
            stopwatchSingle.Stop();

            Console.WriteLine("Multi-threaded multiply with number...");
            stopwatchMulti.Restart();
            resultMulti = ParallelClassicOperations.MultiplyWithNumberParallel(A, multiplyNumber);
            stopwatchMulti.Stop();

            if (resultSingle == resultMulti) { Console.WriteLine("Results are the same."); }
            else { ColoredWriteLine("Results are different!"); }
            Console.WriteLine("Single-threaded: {0}; Multi-threaded: {1}", stopwatchSingle.Elapsed, stopwatchMulti.Elapsed);

            WriteSeparator();




            /**********************************************************************************/
            /***************************** New smaller matrixes *******************************/
            /**********************************************************************************/
            ExitOrContinue();
            WriteSeparator();

            GenerateMatrixes(350, out A, out B, out b);



            /********** Multiplication **********/
            DoBinaryOp(A, B, ClassicOperations.Multiplication, ParallelClassicOperations.MultiplicationParallel, "Multiplication");


            /********** Exponentiate **********/
            int exponent = 4;
            Console.WriteLine("Exponentiate...");
            stopwatchSingle.Restart();
            resultSingle = ClassicOperations.Exponentiate(A, exponent);
            stopwatchSingle.Stop();

            Console.WriteLine("Multi-threaded exponentiate...");
            stopwatchMulti.Restart();
            resultMulti = ParallelClassicOperations.ExponentiateParallel(A, exponent);
            stopwatchMulti.Stop();

            if (resultSingle == resultMulti) { Console.WriteLine("Results are the same."); }
            else { ColoredWriteLine("Results are different!"); }
            Console.WriteLine("Single-threaded: {0}; Multi-threaded: {1}", stopwatchSingle.Elapsed, stopwatchMulti.Elapsed);

            WriteSeparator();


            /********** IsInvertible **********/
            DoUnaryOpBool(A, PropertiesExtensions.IsInvertible, ParallelPropertiesExtensions.IsInvertibleParallel, "IsInvertible");


            /********** Rank **********/
            Console.WriteLine("Rank...");
            stopwatchSingle.Restart();
            resultSingleInt = PropertiesExtensions.Rank(A);
            stopwatchSingle.Stop();

            Console.WriteLine("Multi-threaded rank...");
            stopwatchMulti.Restart();
            resultMultiInt = ParallelPropertiesExtensions.RankParallel(A);
            stopwatchMulti.Stop();

            if (resultSingleInt == resultMultiInt) { Console.WriteLine("Results are the same."); }
            else { ColoredWriteLine("Results are different!"); }
            Console.WriteLine("Single-threaded: {0}; Multi-threaded: {1}", stopwatchSingle.Elapsed, stopwatchMulti.Elapsed);

            WriteSeparator();


            /********** IsOrthogonal **********/
            DoUnaryOpBool(A, PropertiesExtensions.IsOrthogonal, ParallelPropertiesExtensions.IsOrthogonalParallel, "IsOrthogonal");


            /********** Determinant **********/
            DoUnaryOpMatrixNumber(A, ComputationsExtensions.Determinant, ParallelComputationsExtensions.DeterminantParallel, "Determinant");


            /********** Gauss **********/
            DoUnaryOpMatrix(A, AlteringOperationsExtensions.Gauss, ParallelAlteringOperationsExtensions.GaussParallel, "Gauss");


            /********** GaussJordan **********/
            DoUnaryOpMatrix(A, AlteringOperationsExtensions.GaussJordan, ParallelAlteringOperationsExtensions.GaussJordanParallel, "GaussJordan");


            /********** SolveLinearEquations **********/
            DoBinaryOp(A, B, ComputationsExtensions.SolveLinearEquations, ParallelComputationsExtensions.SolveLinearEquationsParallel, "SolveLinearEquations");


            /********** QRDecomposition **********/
            Console.WriteLine("QRDecomposition...");
            stopwatchSingle.Restart();
            resultSingle = DecompositionsExtensions.QRDecomposition(A, out Q, out R);
            stopwatchSingle.Stop();

            Console.WriteLine("Multi-threaded QRDecomposition...");
            stopwatchMulti.Restart();
            resultMulti = ParallelDecompositionsExtensions.QRDecompositionParallel(A, out Q, out R);
            stopwatchMulti.Stop();

            if (resultSingle == resultMulti) { Console.WriteLine("Results are the same."); }
            else { ColoredWriteLine("Results are different!"); }
            Console.WriteLine("Single-threaded: {0}; Multi-threaded: {1}", stopwatchSingle.Elapsed, stopwatchMulti.Elapsed);

            WriteSeparator();


            /********** Orthogonal **********/
            DoUnaryOpMatrix(A, AlteringOperationsExtensions.Orthogonal, ParallelAlteringOperationsExtensions.OrthogonalParallel, "Orthogonal");




            /**********************************************************************************/
            /***************************** New smaller matrixes *******************************/
            /**********************************************************************************/
            ExitOrContinue();
            WriteSeparator();
            GenerateMatrixes(200, out A, out B, out b);



            /********** Inverse **********/
            DoUnaryOpMatrix(A, AlteringOperationsExtensions.Inverse, ParallelAlteringOperationsExtensions.InverseParallel, "Inverse");


            /********** StrassenWinograd **********/
            DoBinaryOp(A, B, ClassicOperations.StrassenWinograd, ParallelClassicOperations.StrassenWinogradParallel, "StrassenWinograd");






            /**********************************************************************************/
            /***************************** New smaller matrixes *******************************/
            /**********************************************************************************/
            ExitOrContinue();
            WriteSeparator();
            GenerateMatrixes(120, out A, out B, out b);



            /********** CholeskyDecomposition **********/
            DoUnaryOpMatrix(A, DecompositionsExtensions.CholeskyDecomposition, ParallelDecompositionsExtensions.CholeskyDecompositionParallel, "CholeskyDecomposition");

            
            /********** Cramer **********/
            DoBinaryOp(A, b, ComputationsExtensions.Cramer, ParallelComputationsExtensions.CramerParallel, "Cramer");




            /**********************************************************************************/
            /***************************** New smaller matrixes *******************************/
            /**********************************************************************************/
            ExitOrContinue();
            WriteSeparator();
            GenerateMatrixes(40, out A, out B, out b);



            /********** Adjugate **********/
            DoUnaryOpMatrix(A, AlteringOperationsExtensions.Adjugate, ParallelAlteringOperationsExtensions.AdjugateParallel, "Adjugate");


            /********** GetEigenValues **********/
            Console.WriteLine("GetEigenValues...");
            stopwatchSingle.Restart();
            try { resultSingle = CharacteristicsExtensions.GetEigenVectors(A, out resultSingleEigen, 100); }
            catch (Exception e) { Console.WriteLine(e.Message); exceptCaughtSingle = true; }
            stopwatchSingle.Stop();

            Console.WriteLine("Multi-threaded QRDecomposition...");
            stopwatchMulti.Restart();
            try { resultMulti = ParallelCharacteristicsExtensions.GetEigenVectorsParallel(A, out resultMultiEigen, 100); }
            catch (Exception e) { Console.WriteLine(e.Message); exceptCaughtMulti = true; }
            stopwatchMulti.Stop();

            if ((exceptCaughtSingle == false && exceptCaughtMulti == false
                && resultSingleEigen == resultMultiEigen && resultSingle == resultMulti)
                || (exceptCaughtSingle == true && exceptCaughtMulti == true))
            { Console.WriteLine("Results are the same."); }
            else { ColoredWriteLine("Results are different!"); }
            Console.WriteLine("Single-threaded: {0}; Multi-threaded: {1}", stopwatchSingle.Elapsed, stopwatchMulti.Elapsed);

            WriteSeparator();

            
            /**/
        }
    }
}
