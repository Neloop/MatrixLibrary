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
            /**********************************************************************************/
            /********************************* Big matrixes ***********************************/
            /**********************************************************************************/
            /**********************************************************************************/

            WriteSeparator(); WriteSeparator("BIG MATRIXES");
            int rowsAndCols = 3000;
            Console.WriteLine("Matrixes will have {0} rows and cols", rowsAndCols);
            Console.WriteLine("Generating matrixes...");
            Random rdm = new Random();
            A = Matrix<MatrixNumber>.GetUninitializedMatrix(rowsAndCols, rowsAndCols);
            B = Matrix<MatrixNumber>.GetUninitializedMatrix(rowsAndCols, rowsAndCols);
            bool resultSingleBool;
            bool resultMultiBool;
            int resultSingleInt;
            int resultMultiInt;
            MatrixNumber resultSingleMatrixNumber;
            MatrixNumber resultMultiMatrixNumber;

            for (int i = 0; i < rowsAndCols; i++)
            {
                for (int j = 0; j < rowsAndCols; j++)
                {
                    A.WriteNumber(i, j, new MatrixNumber(rdm.Next(-10, 10)));
                    B.WriteNumber(i, j, new MatrixNumber(rdm.Next(-10, 10)));
                }
            }
            WriteSeparator();

            /********** Addition **********/
            Console.WriteLine("Addition...");
            stopwatchSingle.Restart();
            resultSingle = ClassicOperations.Addition(A, B);
            stopwatchSingle.Stop();

            Console.WriteLine("Multi-threaded addition...");
            stopwatchMulti.Restart();
            resultMulti = ParallelClassicOperations.Addition(A, B);
            stopwatchMulti.Stop();

            if (resultSingle == resultMulti) { Console.WriteLine("Results are the same."); }
            else { ColoredWriteLine("Results are different!"); }
            Console.WriteLine("Single-threaded: {0}; Multi-threaded: {1}", stopwatchSingle.Elapsed, stopwatchMulti.Elapsed);

            WriteSeparator();


            /********** Subtraction **********/
            Console.WriteLine("Subtraction...");
            stopwatchSingle.Restart();
            resultSingle = ClassicOperations.Subtraction(A, B);
            stopwatchSingle.Stop();

            Console.WriteLine("Multi-threaded subtraction...");
            stopwatchMulti.Restart();
            resultMulti = ParallelClassicOperations.Subtraction(A, B);
            stopwatchMulti.Stop();

            if (resultSingle == resultMulti) { Console.WriteLine("Results are the same."); }
            else { ColoredWriteLine("Results are different!"); }
            Console.WriteLine("Single-threaded: {0}; Multi-threaded: {1}", stopwatchSingle.Elapsed, stopwatchMulti.Elapsed);

            WriteSeparator();


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
            rowsAndCols = 300;
            Console.WriteLine("Matrixes will have {0} rows and cols", rowsAndCols);
            Console.WriteLine("Generating matrixes...");
            A = Matrix<MatrixNumber>.GetUninitializedMatrix(rowsAndCols, rowsAndCols);
            B = Matrix<MatrixNumber>.GetUninitializedMatrix(rowsAndCols, rowsAndCols);
            b = Matrix<MatrixNumber>.GetUninitializedMatrix(rowsAndCols, 1);

            for (int i = 0; i < rowsAndCols; i++)
            {
                b.WriteNumber(i, 0, new MatrixNumber(rdm.Next(-10, 10)));
                for (int j = 0; j < rowsAndCols; j++)
                {
                    A.WriteNumber(i, j, new MatrixNumber(rdm.Next(-10, 10)));
                    B.WriteNumber(i, j, new MatrixNumber(rdm.Next(-10, 10)));
                }
            }
            WriteSeparator();





            /********** Multiplication **********/
            Console.WriteLine("Multiplication...");
            stopwatchSingle.Restart();
            resultSingle = ClassicOperations.Multiplication(A, B);
            stopwatchSingle.Stop();

            Console.WriteLine("Multi-threaded multiplication...");
            stopwatchMulti.Restart();
            resultMulti = ParallelClassicOperations.Multiplication(A, B);
            stopwatchMulti.Stop();

            if (resultSingle == resultMulti) { Console.WriteLine("Results are the same."); }
            else { ColoredWriteLine("Results are different!"); }
            Console.WriteLine("Single-threaded: {0}; Multi-threaded: {1}", stopwatchSingle.Elapsed, stopwatchMulti.Elapsed);

            WriteSeparator();


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
            Console.WriteLine("IsInvertible...");
            stopwatchSingle.Restart();
            resultSingleBool = Properties.IsInvertible(A);
            stopwatchSingle.Stop();

            Console.WriteLine("Multi-threaded IsInvertible...");
            stopwatchMulti.Restart();
            resultMultiBool = ParallelProperties.IsInvertible(A);
            stopwatchMulti.Stop();

            if (resultSingleBool == resultMultiBool) { Console.WriteLine("Results are the same."); }
            else { ColoredWriteLine("Results are different!"); }
            Console.WriteLine("Single-threaded: {0}; Multi-threaded: {1}", stopwatchSingle.Elapsed, stopwatchMulti.Elapsed);

            WriteSeparator();


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
            Console.WriteLine("IsOrthogonal...");
            stopwatchSingle.Restart();
            resultSingleBool = Properties.IsOrthogonal(A);
            stopwatchSingle.Stop();

            Console.WriteLine("Multi-threaded IsOrthogonal...");
            stopwatchMulti.Restart();
            resultMultiBool = ParallelProperties.IsOrthogonal(A);
            stopwatchMulti.Stop();

            if (resultSingleBool == resultMultiBool) { Console.WriteLine("Results are the same."); }
            else { ColoredWriteLine("Results are different!"); }
            Console.WriteLine("Single-threaded: {0}; Multi-threaded: {1}", stopwatchSingle.Elapsed, stopwatchMulti.Elapsed);

            WriteSeparator();


            /********** Determinant **********/
            Console.WriteLine("Determinant...");
            stopwatchSingle.Restart();
            resultSingleMatrixNumber = Computations.Determinant(A);
            stopwatchSingle.Stop();

            Console.WriteLine("Multi-threaded determinant...");
            stopwatchMulti.Restart();
            resultMultiMatrixNumber = ParallelComputations.Determinant(A);
            stopwatchMulti.Stop();

            if (resultSingle == resultMulti) { Console.WriteLine("Results are the same."); }
            else { ColoredWriteLine("Results are different!"); }
            Console.WriteLine("Single-threaded: {0}; Multi-threaded: {1}", stopwatchSingle.Elapsed, stopwatchMulti.Elapsed);

            WriteSeparator();


            /********** Gauss **********/
            Console.WriteLine("Gauss...");
            stopwatchSingle.Restart();
            resultSingle = AlteringOperations.Gauss(A);
            stopwatchSingle.Stop();

            Console.WriteLine("Multi-threaded gauss...");
            stopwatchMulti.Restart();
            resultMulti = ParallelAlteringOperations.Gauss(A);
            stopwatchMulti.Stop();

            if (resultSingle == resultMulti) { Console.WriteLine("Results are the same."); }
            else { ColoredWriteLine("Results are different!"); }
            Console.WriteLine("Single-threaded: {0}; Multi-threaded: {1}", stopwatchSingle.Elapsed, stopwatchMulti.Elapsed);

            WriteSeparator();


            /********** GaussJordan **********/
            Console.WriteLine("GaussJordan...");
            stopwatchSingle.Restart();
            resultSingle = AlteringOperations.GaussJordan(A);
            stopwatchSingle.Stop();

            Console.WriteLine("Multi-threaded GaussJordan...");
            stopwatchMulti.Restart();
            resultMulti = ParallelAlteringOperations.GaussJordan(A);
            stopwatchMulti.Stop();

            if (resultSingle == resultMulti) { Console.WriteLine("Results are the same."); }
            else { ColoredWriteLine("Results are different!"); }
            Console.WriteLine("Single-threaded: {0}; Multi-threaded: {1}", stopwatchSingle.Elapsed, stopwatchMulti.Elapsed);

            WriteSeparator();


            /********** SolveLinearEquations **********/
            Console.WriteLine("SolveLinearEquations...");
            stopwatchSingle.Restart();
            resultSingle = Computations.SolveLinearEquations(A, b);
            stopwatchSingle.Stop();

            Console.WriteLine("Multi-threaded SolveLinearEquations...");
            stopwatchMulti.Restart();
            resultMulti = ParallelComputations.SolveLinearEquations(A, b);
            stopwatchMulti.Stop();

            if (resultSingle == resultMulti) { Console.WriteLine("Results are the same."); }
            else { ColoredWriteLine("Results are different!"); }
            Console.WriteLine("Single-threaded: {0}; Multi-threaded: {1}", stopwatchSingle.Elapsed, stopwatchMulti.Elapsed);

            WriteSeparator();


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




            /**********************************************************************************/
            /***************************** New smaller matrixes *******************************/
            /**********************************************************************************/
            ExitOrContinue();
            WriteSeparator();
            rowsAndCols = 100;
            Console.WriteLine("Matrixes will have {0} rows and cols", rowsAndCols);
            Console.WriteLine("Generating matrixes...");
            A = Matrix<MatrixNumber>.GetUninitializedMatrix(rowsAndCols, rowsAndCols);
            B = Matrix<MatrixNumber>.GetUninitializedMatrix(rowsAndCols, rowsAndCols);

            for (int i = 0; i < rowsAndCols; i++)
            {
                for (int j = 0; j < rowsAndCols; j++)
                {
                    A.WriteNumber(i, j, new MatrixNumber(rdm.Next(-10, 10)));
                    B.WriteNumber(i, j, new MatrixNumber(rdm.Next(-10, 10)));
                }
            }
            WriteSeparator();





            /********** Inverse **********/
            Console.WriteLine("Inverse...");
            bool inverseExceptionCaught = false;
            stopwatchSingle.Restart();
            try { resultSingle = AlteringOperations.Inverse(A); }
            catch (MatrixLibraryException e) { Console.WriteLine(e.Message); inverseExceptionCaught = true; }
            stopwatchSingle.Stop();

            Console.WriteLine("Multi-threaded inverse...");
            stopwatchMulti.Restart();
            try { resultMulti = ParallelAlteringOperations.Inverse(A); }
            catch (MatrixLibraryException e) { Console.WriteLine(e.Message); inverseExceptionCaught = true; }
            stopwatchMulti.Stop();

            if (inverseExceptionCaught == false)
            {
                if (resultSingle == resultMulti) { Console.WriteLine("Results are the same."); }
                else { ColoredWriteLine("Results are different!"); }
            }
            Console.WriteLine("Single-threaded: {0}; Multi-threaded: {1}", stopwatchSingle.Elapsed, stopwatchMulti.Elapsed);

            WriteSeparator();


            /********** StrassenWinograd **********/
            Console.WriteLine("StrassenWinograd...");
            stopwatchSingle.Restart();
            resultSingle = ClassicOperations.StrassenWinograd(A, B);
            stopwatchSingle.Stop();

            Console.WriteLine("Multi-threaded StrassenWinograd...");
            stopwatchMulti.Restart();
            resultMulti = ParallelClassicOperations.StrassenWinograd(A, B);
            stopwatchMulti.Stop();

            if (resultSingle == resultMulti) { Console.WriteLine("Results are the same."); }
            else { ColoredWriteLine("Results are different!"); }
            Console.WriteLine("Single-threaded: {0}; Multi-threaded: {1}", stopwatchSingle.Elapsed, stopwatchMulti.Elapsed);

            WriteSeparator();


            /********** Orthogonal **********/
            Console.WriteLine("Orthogonal...");
            stopwatchSingle.Restart();
            resultSingle = AlteringOperations.Orthogonal(A);
            stopwatchSingle.Stop();

            Console.WriteLine("Multi-threaded Orthogonal...");
            stopwatchMulti.Restart();
            resultMulti = ParallelAlteringOperations.Orthogonal(A);
            stopwatchMulti.Stop();

            if (resultSingle == resultMulti) { Console.WriteLine("Results are the same."); }
            else { ColoredWriteLine("Results are different!"); }
            Console.WriteLine("Single-threaded: {0}; Multi-threaded: {1}", stopwatchSingle.Elapsed, stopwatchMulti.Elapsed);

            WriteSeparator();


            /********** CholeskyDecomposition **********/
            Console.WriteLine("CholeskyDecomposition...");
            bool choleskyExceptionCaught = false;
            stopwatchSingle.Restart();
            try { resultSingle = Decompositions.CholeskyDecomposition(A); }
            catch (MatrixLibraryException e) { Console.WriteLine(e.Message); choleskyExceptionCaught = true; }
            stopwatchSingle.Stop();

            Console.WriteLine("Multi-threaded CholeskyDecomposition...");
            stopwatchMulti.Restart();
            try { resultMulti = ParallelDecompositions.CholeskyDecomposition(A); }
            catch (MatrixLibraryException e) { Console.WriteLine(e.Message); choleskyExceptionCaught = true; }
            stopwatchMulti.Stop();

            if (choleskyExceptionCaught == false)
            {
                if (resultSingle == resultMulti) { Console.WriteLine("Results are the same."); }
                else { ColoredWriteLine("Results are different!"); }
            }
            Console.WriteLine("Single-threaded: {0}; Multi-threaded: {1}", stopwatchSingle.Elapsed, stopwatchMulti.Elapsed);

            WriteSeparator();




            /**********************************************************************************/
            /***************************** New smaller matrixes *******************************/
            /**********************************************************************************/
            ExitOrContinue();
            WriteSeparator();
            rowsAndCols = 40;
            Console.WriteLine("Matrixes will have {0} rows and cols", rowsAndCols);
            Console.WriteLine("Generating matrixes...");
            A = Matrix<MatrixNumber>.GetUninitializedMatrix(rowsAndCols, rowsAndCols);
            B = Matrix<MatrixNumber>.GetUninitializedMatrix(rowsAndCols, rowsAndCols);

            for (int i = 0; i < rowsAndCols; i++)
            {
                for (int j = 0; j < rowsAndCols; j++)
                {
                    A.WriteNumber(i, j, new MatrixNumber(rdm.Next(-10, 10)));
                    B.WriteNumber(i, j, new MatrixNumber(rdm.Next(-10, 10)));
                }
            }
            WriteSeparator();




            /********** Adjugate **********/
            Console.WriteLine("Adjugate...");
            stopwatchSingle.Restart();
            resultSingle = AlteringOperations.Adjugate(A);
            stopwatchSingle.Stop();

            Console.WriteLine("Multi-threaded adjugate...");
            stopwatchMulti.Restart();
            resultMulti = ParallelAlteringOperations.Adjugate(A);
            stopwatchMulti.Stop();

            if (resultSingle == resultMulti) { Console.WriteLine("Results are the same."); }
            else { ColoredWriteLine("Results are different!"); }
            Console.WriteLine("Single-threaded: {0}; Multi-threaded: {1}", stopwatchSingle.Elapsed, stopwatchMulti.Elapsed);

            WriteSeparator();


            /**/


            ExitOrContinue();
            /**********************************************************************************/
            /***************************** MATRIX PARALIZATION ********************************/
            /**********************************************************************************/

            WriteSeparator(); WriteSeparator("MATRIX PARALIZATION");
            rowsAndCols = 5000;
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
        }
    }
}
