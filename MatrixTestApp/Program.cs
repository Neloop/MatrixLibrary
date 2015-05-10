using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MatrixLibrary;
using System.Diagnostics;

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
            resultSingle = Vypocty.Cramer(cram, b);
            WriteTwoMatrixes(cram, b, resultSingle, "=");

            Console.WriteLine();

            inputMatrix = new int[4, 1] {{2}, {5}, {-1}, {1}};
            b = new Matrix<MatrixNumber>(inputMatrix);

            Console.WriteLine("Soustava rovnic");
            resultSingle = Vypocty.SoustavaRovnic(B, b);
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

            Matrix<MatrixNumber>[] array = new Matrix<MatrixNumber>[3];
            array[0] = A;
            array[1] = B;
            array[2] = cram;
            resultSingle = Vypocty.Rovnice("X = A * B + C", array);
            WriteMatrix(resultSingle, resultSingle, "X = A * B + C");

            /*
             * 
             * Operace s jednou maticí...
             * 
             * 
             */

            inputMatrix = new int[3, 3] {{1, 0, 5}, {1, 1, 6}, {0, 1, 7}};
            //vstup = new int[4, 4] {{1, 0, 5, 6}, {1, 1, 6, 7}, {0, 1, 7, 8}, {10, 7, 8, 3}};

            A = new Matrix<MatrixNumber>(inputMatrix);

            WriteSeparator();

            resultSingle = Upravit.Transponuj(A);
            WriteMatrix(A, resultSingle, "Transponovaná");

            Console.WriteLine();

            MatrixNumber det = Vypocty.Determinant(B);
            zeroM = new int[1, 1];
            resultSingle = new Matrix<MatrixNumber>(zeroM);
            resultSingle.WriteNumber(0, 0, det);
            WriteMatrix(B, resultSingle, "Determinant");

            Console.WriteLine();

            inputMatrix = new int[2, 2] {{0, 5}, {1, 7}};

            Matrix<MatrixNumber> C = new Matrix<MatrixNumber>(inputMatrix);

            det = Vypocty.Determinant(C);
            resultSingle = new Matrix<MatrixNumber>(zeroM);
            resultSingle.WriteNumber(0, 0, det);
            WriteMatrix(C, resultSingle, "Determinant");

            Console.WriteLine();

            resultSingle = Upravit.Gauss(B);
            WriteMatrix(B, resultSingle, "Gauss");

            Console.WriteLine();

            resultSingle = Upravit.GaussJordan(B);
            WriteMatrix(B, resultSingle, "GaussJordan");

            Console.WriteLine();

            resultSingle = Upravit.Adjungovana(A);
            WriteMatrix(A, resultSingle, "Adjungovaná");

            Console.WriteLine();

            bool regular = Vlastnosti.Regularnost(B);
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
            regular = Vlastnosti.Regularnost(D);
            number = new int[1, 1];
            resultSingle = new Matrix<MatrixNumber>(number);
            if (regular == true) { resultSingle.WriteNumber(0, 0, one); }
            else { resultSingle.WriteNumber(0, 0, zero); }
            WriteMatrix(D, resultSingle, "Regulární");

            Console.WriteLine();

            bool orthogonal = Vlastnosti.Ortogonalnost(B);
            number = new int[1, 1];
            resultSingle = new Matrix<MatrixNumber>(number);
            if (orthogonal == true) { resultSingle.WriteNumber(0, 0, one); }
            else { resultSingle.WriteNumber(0, 0, zero); }
            WriteMatrix(B, resultSingle, "Ortogonální");

            Console.WriteLine();

            resultSingle = Upravit.Inverzni(A);
            WriteMatrix(A, resultSingle, "Inverzní");

            Console.WriteLine();

            resultSingle = Upravit.Zesymetrizuj(A);
            WriteMatrix(A, resultSingle, "Zesymetrizování");

            Console.WriteLine();

            resultSingle = Upravit.Ortogonalizace(B);
            WriteMatrix(B, resultSingle, "Ortogonalizace");

            Console.WriteLine();

            try { resultSingle = Rozklady.CholeskyRozklad(A); }
            catch { resultSingle = new Matrix<MatrixNumber>(1, 1); }
            finally { WriteMatrix(A, resultSingle, "Choleského rozklad"); }

            Console.WriteLine();

            Matrix<MatrixNumber> Q, R;
            resultSingle = Rozklady.QRRozklad(A, out Q, out R);
            WriteMatrix(A, resultSingle, "QR-rozklad");

            Console.WriteLine();

            zeroM = new int[2, 2] {{2, 1}, {0, 1}};
            A = new Matrix<MatrixNumber>(zeroM);

            Vl_cisla<MatrixNumber> tmp = Charakteristika.Vlastni_cisla(A, 0);
            resultSingle = new Matrix<MatrixNumber>(tmp.Pocet(), 1);
            for (int i = 0; i < resultSingle.Rows; i++)
            {
                resultSingle.WriteNumber(i, 0, tmp.Vrat_cislo(i));
            }
            WriteMatrix(A, resultSingle, "Vlastní čísla");

            Console.WriteLine();

            resultSingle = Charakteristika.Vlastni_vektory(A, out tmp, 0);
            WriteMatrix(A, resultSingle, "Vlastní vektory");

            Console.WriteLine();

            Matrix<MatrixNumber> S;
            resultSingle = Charakteristika.Diagonalizovat(A, out S, 0);
            WriteMatrix(A, resultSingle, "Diagonalizovat");

            Console.WriteLine();

            resultSingle = ClassicOperations.Exponentiate(A, 4);
            WriteMatrix(A, resultSingle, "Umocnit A^4");





            Console.WriteLine();
            Console.WriteLine("Press ENTER to continue calculations...");
            Console.ReadLine();
            /********************************* Big matrixes ***********************************/

            WriteSeparator(); WriteSeparator("BIG MATRIXES");
            int rowsAndCols = 100;
            Console.WriteLine("Matrixes will have {0} rows and cols", rowsAndCols);
            Console.WriteLine("Generating matrixes...");
            Random rdm = new Random();
            A = new Matrix<MatrixNumber>(rowsAndCols, rowsAndCols);
            B = new Matrix<MatrixNumber>(rowsAndCols, rowsAndCols);

            for (int i = 0; i < rowsAndCols; i++)
            {
                for (int j = 0; j < rowsAndCols; j++)
                {
                    A.WriteNumber(i, j, new MatrixNumber(rdm.Next()));
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
            resultMulti = ClassicOperations.Addition_MultiThreaded(A, B);
            stopwatchMulti.Stop();

            if (resultSingle == resultMulti) { Console.WriteLine("Results are the same."); }
            else { Console.WriteLine("Results are different!"); }
            Console.WriteLine("Single-threaded: {0}; Multi-threaded: {1}", stopwatchSingle.Elapsed, stopwatchMulti.Elapsed);

            WriteSeparator();


            /********** Subtraction **********/
            Console.WriteLine("Subtraction...");
            stopwatchSingle.Restart();
            resultSingle = ClassicOperations.Subtraction(A, B);
            stopwatchSingle.Stop();

            Console.WriteLine("Multi-threaded subtraction...");
            stopwatchMulti.Restart();
            resultMulti = ClassicOperations.Subtraction_MultiThreaded(A, B);
            stopwatchMulti.Stop();

            if (resultSingle == resultMulti) { Console.WriteLine("Results are the same."); }
            else { Console.WriteLine("Results are different!"); }
            Console.WriteLine("Single-threaded: {0}; Multi-threaded: {1}", stopwatchSingle.Elapsed, stopwatchMulti.Elapsed);

            WriteSeparator();


            /********** Multiplication **********/
            Console.WriteLine("Multiplication...");
            stopwatchSingle.Restart();
            resultSingle = ClassicOperations.Multiplication(A, B);
            stopwatchSingle.Stop();

            Console.WriteLine("Multi-threaded multiplication...");
            stopwatchMulti.Restart();
            resultMulti = ClassicOperations.Multiplication_MultiThreaded(A, B);
            stopwatchMulti.Stop();

            if (resultSingle == resultMulti) { Console.WriteLine("Results are the same."); }
            else { Console.WriteLine("Results are different!"); }
            Console.WriteLine("Single-threaded: {0}; Multi-threaded: {1}", stopwatchSingle.Elapsed, stopwatchMulti.Elapsed);

            WriteSeparator();


            /********** StrassenWinograd **********/
            /*Console.WriteLine("StrassenWinograd...");
            stopwatchSingle.Restart();
            resultSingle = ClassicOperations.StrassenWinograd(A, B);
            stopwatchSingle.Stop();

            Console.WriteLine("Multi-threaded StrassenWinograd...");
            stopwatchMulti.Restart();
            //resultMulti = ClassicOperations.StrassenWinograd_MultiThreaded(A, B);
            stopwatchMulti.Stop();

            if (resultSingle == resultMulti) { Console.WriteLine("Results are the same."); }
            else { Console.WriteLine("Results are different!"); }
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
            resultMulti = ClassicOperations.MultiplyWithNumber_MultiThreaded(A, multiplyNumber);
            stopwatchMulti.Stop();

            if (resultSingle == resultMulti) { Console.WriteLine("Results are the same."); }
            else { Console.WriteLine("Results are different!"); }
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
            resultMulti = ClassicOperations.Exponentiate_MultiThreaded(A, exponent);
            stopwatchMulti.Stop();

            if (resultSingle == resultMulti) { Console.WriteLine("Results are the same."); }
            else { Console.WriteLine("Results are different!"); }
            Console.WriteLine("Single-threaded: {0}; Multi-threaded: {1}", stopwatchSingle.Elapsed, stopwatchMulti.Elapsed);

            WriteSeparator();
        }
    }
}
