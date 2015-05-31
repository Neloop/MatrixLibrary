using System;
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

        static void WriteTwoMatrixes<T>(Matrix<T> matrix1, Matrix<T> matrix2, Matrix<T> result, string operationName) where T : MatrixNumberBase, new()
        {
            int maxRows = Math.Max(Math.Max(matrix1.Rows, matrix2.Rows), result.Rows);
            int half = maxRows / 2;

            for (int i = 0; i < maxRows; i++)
            {
                if (matrix1.Rows > i)
                {
                    for (int j = 0; j < matrix1.Cols; j++) // první matice
                    {
                        T tmp = matrix1.GetNumber(i, j);
                        Console.Write("{0,4} ", tmp.ToDouble());
                    }
                }
                else { for (int j = 0; j < matrix1.Cols; j++) { Console.Write("     "); } }

                if (i == half) { Console.Write(" {0} ", operationName); } // operace
                else { Console.Write("   "); }

                if (matrix2.Rows > i)
                {
                    for (int j = 0; j < matrix2.Cols; j++) // druhá matice
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
                    for (int j = 0; j < result.Cols; j++) // výsledek matice
                    {
                        T tmp = result.GetNumber(i, j);
                        Console.Write("{0,4} ", tmp.ToDouble());
                    }
                }
                else { for (int j = 0; j < result.Cols; j++) { Console.Write("      "); } }
                Console.WriteLine();
            }
        }
        static void WriteMatrix<T>(Matrix<T> matrix, Matrix<T> result, string operationName) where T : MatrixNumberBase, new()
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
            int[,] inputMatrix = new int[4, 4] { {1, 0, 5, 6}, {1, 1, 6, 7}, {0, 1, 7, 8}, {10, 7, 8, 3} };

            Matrix<MatrixNumber> A = new Matrix<MatrixNumber>(inputMatrix);

            inputMatrix = new int[4, 4] { {1, 1, -2, 3}, {4, 3, -1, 5}, {1, 0, 5, -4}, {2, 1, 3, -1} };

            Matrix<MatrixNumber> B = new Matrix<MatrixNumber>(inputMatrix);

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
            resultSingle = Computations.Cramer(cram, b);
            WriteTwoMatrixes(cram, b, resultSingle, "=");

            Console.WriteLine();

            inputMatrix = new int[4, 1] {{2}, {5}, {-1}, {1}};
            b = new Matrix<MatrixNumber>(inputMatrix);

            Console.WriteLine("Soustava rovnic");
            resultSingle = Computations.SolveLinearEquations(B, b);
            //try { resultSingle = Computations.SolveLinearEquations(B, b); }
            //catch (MatrixLibraryException) { resultSingle = new Matrix<MatrixNumber>(1, 1); }
            WriteTwoMatrixes(B, b, resultSingle, "=");

            Console.WriteLine();

            Console.WriteLine("Násobení matice číslem");
            MatrixNumber multiply = new MatrixNumber(2, 1);
            int[,] zeroM = new int[1, 1];
            zeroM[0, 0] = 2;
            Matrix<MatrixNumber> multiplyM = new Matrix<MatrixNumber>(zeroM);
            resultSingle = ClassicOperations.MultiplyWithNumber(A, multiply);
            WriteTwoMatrixes(multiplyM, A, resultSingle, "*");

            Console.WriteLine();

            /*
             * 
             * Operace s jednou maticí...
             * 
             * 
             */

            inputMatrix = new int[3, 3] { { 1, 0, 5 }, { 1, 1, 6 }, { 0, 1, 7 } };
            //vstup = new int[4, 4] {{1, 0, 5, 6}, {1, 1, 6, 7}, {0, 1, 7, 8}, {10, 7, 8, 3}};

            A = new Matrix<MatrixNumber>(inputMatrix);

            WriteSeparator();

            resultSingle = AlteringOperations.Transposition(A);
            WriteMatrix(A, resultSingle, "Transponovaná");

            Console.WriteLine();

            MatrixNumber det = Computations.Determinant(B);
            zeroM = new int[1, 1];
            resultSingle = new Matrix<MatrixNumber>(zeroM);
            resultSingle.WriteNumber(0, 0, det);
            WriteMatrix(B, resultSingle, "Determinant");

            Console.WriteLine();

            inputMatrix = new int[2, 2] {{0, 5}, {1, 7}};

            Matrix<MatrixNumber> C = new Matrix<MatrixNumber>(inputMatrix);

            det = Computations.Determinant(C);
            resultSingle = new Matrix<MatrixNumber>(zeroM);
            resultSingle.WriteNumber(0, 0, det);
            WriteMatrix(C, resultSingle, "Determinant");

            Console.WriteLine();

            resultSingle = AlteringOperations.Gauss(B);
            WriteMatrix(B, resultSingle, "Gauss");

            Console.WriteLine();

            resultSingle = AlteringOperations.GaussJordan(B);
            WriteMatrix(B, resultSingle, "GaussJordan");

            Console.WriteLine();

            resultSingle = AlteringOperations.Adjugate(A);
            WriteMatrix(A, resultSingle, "Adjungovaná");

            Console.WriteLine();

            bool regular = Properties.IsInvertible(B);
            int[,] number = new int[1, 1];
            resultSingle = new Matrix<MatrixNumber>(number);
            MatrixNumber one = new MatrixNumber(1, 1);
            MatrixNumber zero = new MatrixNumber(0, 0);
            if (regular == true) { resultSingle.WriteNumber(0, 0, one); }
            else { resultSingle.WriteNumber(0, 0, zero); }
            WriteMatrix(B, resultSingle, "Regulární");

            Console.WriteLine();
            inputMatrix = new int[4, 4];
            Matrix<MatrixNumber> D = new Matrix<MatrixNumber>(inputMatrix);
            regular = Properties.IsInvertible(D);
            number = new int[1, 1];
            resultSingle = new Matrix<MatrixNumber>(number);
            if (regular == true) { resultSingle.WriteNumber(0, 0, one); }
            else { resultSingle.WriteNumber(0, 0, zero); }
            WriteMatrix(D, resultSingle, "Regulární");

            Console.WriteLine();

            bool orthogonal = Properties.IsOrthogonal(B);
            number = new int[1, 1];
            resultSingle = new Matrix<MatrixNumber>(number);
            if (orthogonal == true) { resultSingle.WriteNumber(0, 0, one); }
            else { resultSingle.WriteNumber(0, 0, zero); }
            WriteMatrix(B, resultSingle, "Ortogonální");

            Console.WriteLine();

            try { resultSingle = AlteringOperations.Inverse(A); }
            catch { resultSingle = new Matrix<MatrixNumber>(1, 1); }
            WriteMatrix(A, resultSingle, "Inverzní");

            Console.WriteLine();

            try { resultSingle = AlteringOperations.Symmetric(A); }
            catch { resultSingle = new Matrix<MatrixNumber>(1, 1); }
            WriteMatrix(A, resultSingle, "Zesymetrizování");

            Console.WriteLine();

            resultSingle = AlteringOperations.Orthogonal(B);
            WriteMatrix(B, resultSingle, "Ortogonalizace");

            Console.WriteLine();

            try { resultSingle = Decompositions.CholeskyDecomposition(A); }
            catch { resultSingle = new Matrix<MatrixNumber>(1, 1); }
            finally { WriteMatrix(A, resultSingle, "Choleského rozklad"); }

            Console.WriteLine();

            Matrix<MatrixNumber> Q, R;
            resultSingle = Decompositions.QRDecomposition(A, out Q, out R);
            WriteMatrix(A, resultSingle, "QR-rozklad");

            Console.WriteLine();

            zeroM = new int[2, 2] {{2, 1}, {0, 1}};
            A = new Matrix<MatrixNumber>(zeroM);

            EigenValues<MatrixNumber> tmp = Characteristics.GetEigenValues(A, 0);
            resultSingle = new Matrix<MatrixNumber>(tmp.Count(), 1);
            for (int i = 0; i < resultSingle.Rows; i++)
            {
                resultSingle.WriteNumber(i, 0, tmp.GetEigenValue(i));
            }
            WriteMatrix(A, resultSingle, "Vlastní čísla");

            Console.WriteLine();

            resultSingle = Characteristics.GetEigenVectors(A, out tmp, 0);
            WriteMatrix(A, resultSingle, "Vlastní vektory");

            Console.WriteLine();

            Matrix<MatrixNumber> S;
            resultSingle = Characteristics.Diagonal(A, out S, 0);
            WriteMatrix(A, resultSingle, "Diagonalizovat");

            Console.WriteLine();

            resultSingle = ClassicOperations.Exponentiate(A, 4);
            WriteMatrix(A, resultSingle, "Umocnit A^4");

            Console.WriteLine();
            WriteSeparator();






            ExitOrContinue();
            /**********************************************************************************/
            /***************************** MATRIX PARALIZATION ********************************/
            /**********************************************************************************/

            WriteSeparator(); WriteSeparator("MATRIX PARALIZATION");
            int rowsAndCols = 5000;
            Console.WriteLine("Matrix will have {0} rows and cols", rowsAndCols);
            Console.WriteLine("Generating matrix...");
            A = new Matrix<MatrixNumber>(rowsAndCols, rowsAndCols);

            Console.WriteLine("Single-threaded multidimensional array going through...");
            stopwatchSingle.Restart();
            for (int i = 0; i < rowsAndCols; i++)
            {
                for (int j = 0; j < rowsAndCols; ++j)
                {
                    A.WriteNumber(i, j, A.GetNumber(i, j));
                }
            }
            stopwatchSingle.Stop();

            Console.WriteLine("Multi-threaded multidimensional array going through...");
            stopwatchMulti.Restart();
            Parallel.ForEach(A.GetRowsChunks(), (pair) =>
            {
                //Console.WriteLine(System.Threading.Thread.CurrentThread.ManagedThreadId);
                for (int i = pair.Item1; i < pair.Item2; ++i)
                {
                    for (int j = 0; j < rowsAndCols; ++j)
                    {
                        A.WriteNumber(i, j, A.GetNumber(i, j));
                    }
                }
            });
            stopwatchMulti.Stop();

            Console.WriteLine("Single-threaded: {0}; Multi-threaded: {1}", stopwatchSingle.Elapsed, stopwatchMulti.Elapsed);

            WriteSeparator();




            ExitOrContinue();
            /**********************************************************************************/
            /**********************************************************************************/
            /********************************* Big matrixes ***********************************/
            /**********************************************************************************/
            /**********************************************************************************/

            WriteSeparator(); WriteSeparator("BIG MATRIXES");
            int resultSingleInt;
            int resultMultiInt;
            GenerateMatrixes(3000, out A, out B, out b);


            /********** Addition **********/
            DoBinaryOp(A, B, ClassicOperations.Addition, ParallelClassicOperations.Addition, "Addition");


            /********** Subtraction **********/
            DoBinaryOp(A, B, ClassicOperations.Subtraction, ParallelClassicOperations.Subtraction, "Subtraction");


            /********** MultiplyWithNumber **********/
            MatrixNumber multiplyNumber = new MatrixNumber(100);
            Console.WriteLine("MultiplyWithNumber...");
            stopwatchSingle.Restart();
            resultSingle = ClassicOperations.MultiplyWithNumber(A, multiplyNumber);
            stopwatchSingle.Stop();

            Console.WriteLine("Multi-threaded multiply with number...");
            stopwatchMulti.Restart();
            resultMulti = ParallelClassicOperations.MultiplyWithNumber(A, multiplyNumber);
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

            GenerateMatrixes(300, out A, out B, out b);



            /********** Multiplication **********/
            DoBinaryOp(A, B, ClassicOperations.Multiplication, ParallelClassicOperations.Multiplication, "Multiplication");


            /********** Exponentiate **********/
            int exponent = 4;
            Console.WriteLine("Exponentiate...");
            stopwatchSingle.Restart();
            resultSingle = ClassicOperations.Exponentiate(A, exponent);
            stopwatchSingle.Stop();

            Console.WriteLine("Multi-threaded exponentiate...");
            stopwatchMulti.Restart();
            resultMulti = ParallelClassicOperations.Exponentiate(A, exponent);
            stopwatchMulti.Stop();

            if (resultSingle == resultMulti) { Console.WriteLine("Results are the same."); }
            else { ColoredWriteLine("Results are different!"); }
            Console.WriteLine("Single-threaded: {0}; Multi-threaded: {1}", stopwatchSingle.Elapsed, stopwatchMulti.Elapsed);

            WriteSeparator();


            /********** IsInvertible **********/
            DoUnaryOpBool(A, Properties.IsInvertible, ParallelProperties.IsInvertible, "IsInvertible");


            /********** Rank **********/
            Console.WriteLine("Rank...");
            stopwatchSingle.Restart();
            resultSingleInt = Properties.Rank(A);
            stopwatchSingle.Stop();

            Console.WriteLine("Multi-threaded rank...");
            stopwatchMulti.Restart();
            resultMultiInt = ParallelProperties.Rank(A);
            stopwatchMulti.Stop();

            if (resultSingleInt == resultMultiInt) { Console.WriteLine("Results are the same."); }
            else { ColoredWriteLine("Results are different!"); }
            Console.WriteLine("Single-threaded: {0}; Multi-threaded: {1}", stopwatchSingle.Elapsed, stopwatchMulti.Elapsed);

            WriteSeparator();


            /********** IsOrthogonal **********/
            DoUnaryOpBool(A, Properties.IsOrthogonal, ParallelProperties.IsOrthogonal, "IsOrthogonal");


            /********** Determinant **********/
            DoUnaryOpMatrixNumber(A, Computations.Determinant, ParallelComputations.Determinant, "Determinant");


            /********** Gauss **********/
            DoUnaryOpMatrix(A, AlteringOperations.Gauss, ParallelAlteringOperations.Gauss, "Gauss");


            /********** GaussJordan **********/
            DoUnaryOpMatrix(A, AlteringOperations.GaussJordan, ParallelAlteringOperations.GaussJordan, "GaussJordan");


            /********** SolveLinearEquations **********/
            DoBinaryOp(A, B, Computations.SolveLinearEquations, ParallelComputations.SolveLinearEquations, "SolveLinearEquations");


            /********** QRDecomposition **********/
            Console.WriteLine("QRDecomposition...");
            stopwatchSingle.Restart();
            resultSingle = Decompositions.QRDecomposition(A, out Q, out R);
            stopwatchSingle.Stop();

            Console.WriteLine("Multi-threaded QRDecomposition...");
            stopwatchMulti.Restart();
            resultMulti = ParallelDecompositions.QRDecomposition(A, out Q, out R);
            stopwatchMulti.Stop();

            if (resultSingle == resultMulti) { Console.WriteLine("Results are the same."); }
            else { ColoredWriteLine("Results are different!"); }
            Console.WriteLine("Single-threaded: {0}; Multi-threaded: {1}", stopwatchSingle.Elapsed, stopwatchMulti.Elapsed);

            WriteSeparator();


            /********** Orthogonal **********/
            DoUnaryOpMatrix(A, AlteringOperations.Orthogonal, ParallelAlteringOperations.Orthogonal, "Orthogonal");




            /**********************************************************************************/
            /***************************** New smaller matrixes *******************************/
            /**********************************************************************************/
            ExitOrContinue();
            WriteSeparator();
            GenerateMatrixes(200, out A, out B, out b);



            /********** Inverse **********/
            DoUnaryOpMatrix(A, AlteringOperations.Inverse, ParallelAlteringOperations.Inverse, "Inverse");


            /********** StrassenWinograd **********/
            DoBinaryOp(A, B, ClassicOperations.StrassenWinograd, ParallelClassicOperations.StrassenWinograd, "StrassenWinograd");






            /**********************************************************************************/
            /***************************** New smaller matrixes *******************************/
            /**********************************************************************************/
            ExitOrContinue();
            WriteSeparator();
            GenerateMatrixes(120, out A, out B, out b);



            /********** CholeskyDecomposition **********/
            DoUnaryOpMatrix(A, Decompositions.CholeskyDecomposition, ParallelDecompositions.CholeskyDecomposition, "CholeskyDecomposition");

            
            /********** Cramer **********/
            DoBinaryOp(A, b, Computations.Cramer, ParallelComputations.Cramer, "Cramer");




            /**********************************************************************************/
            /***************************** New smaller matrixes *******************************/
            /**********************************************************************************/
            ExitOrContinue();
            WriteSeparator();
            GenerateMatrixes(40, out A, out B, out b);



            /********** Adjugate **********/
            DoUnaryOpMatrix(A, AlteringOperations.Adjugate, ParallelAlteringOperations.Adjugate, "Adjugate");
            
            
            /**/
        }
    }
}
