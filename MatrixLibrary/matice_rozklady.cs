using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLibrary
{
    public static class Rozklady
    {
        public static Matrix<T> CholeskyRozklad<T>(Matrix<T> matice) where T : MatrixNumberBase, new() // Je vrácena dolní trojúhelníková matice L, vstupní matice nemusí být symetrická, bude zesymetriována
        {
            /*
             * 
             * Choleského rozklad: A = L * L^(T)
             *  - matice A musí být Positivně-definitní a typu n*n
             * 
             */

            Matrix<T> vysledek;
            Matrix<T> symetricka = AlteringOperations.Symmetric(matice);
            if (Properties.Definity(symetricka) == Properties.DefinityClassification.PositiveDefinite)
            {
                int rozmer;
                rozmer = matice.Rows;
                vysledek = new Matrix<T>(rozmer, rozmer);

                for (int i = 0; i < rozmer; i++)
                {
                    T secti = new T();
                    for (int j = 0; j < i; j++)
                    {
                        secti = (T)(secti + vysledek.GetNumber(i, j).__Exponentiate(2));
                    }
                    T zapis = (T)(symetricka.GetNumber(i, i) - secti);
                    zapis = (T)zapis.__SquareRoot();
                    vysledek.WriteNumber(i, i, zapis);

                    for (int j = i + 1; j < rozmer; j++)
                    {
                        for (int k = 0; k < i; k++)
                        {
                            if (k != 0) { secti = (T)(secti + vysledek.GetNumber(i, k) * vysledek.GetNumber(j, k)); }
                            else { secti = (T)(vysledek.GetNumber(i, k) * vysledek.GetNumber(j, k)); }
                        }
                        zapis = (T)(symetricka.GetNumber(j, i) - secti);
                        zapis = (T)(zapis / vysledek.GetNumber(i, i));
                        vysledek.WriteNumber(j, i, zapis);
                    }
                }
            }
            else
            {
                throw new MatrixLibraryException("Given matrix is not positive-definite");
            }
            return vysledek;
        }
        public static Matrix<T> QRRozklad<T>(Matrix<T> matice, out Matrix<T> Q, out Matrix<T> R) where T : MatrixNumberBase, new() // Vrácena je matice R*Q
        {
            Matrix<T> vysledek;
            int radky = matice.Rows;
            int sloupce = matice.Cols;
            Q = new Matrix<T>(matice.Rows, matice.Cols);
            R = new Matrix<T>(matice.Rows, matice.Cols);

            for (int i = 0; i < radky; i++) // řádky
            {
                for (int j = 0; j < sloupce; j++) // sloupce
                {
                    T suma = new T();
                    for (int k = 0; k < i; k++) // suma...
                    {
                        T skal_soucin = new T();
                        for (int l = 0; l < sloupce; l++) // skal. součin
                        {
                            T x = (T)matice.GetNumber(i, l).Copy();
                            T z = (T)Q.GetNumber(k, l).Copy();

                            skal_soucin = (T)((x * z) + skal_soucin);
                        }

                        R.WriteNumber(k, i, skal_soucin);

                        T krat = (T)Q.GetNumber(k, j).Copy();
                        krat = (T)(skal_soucin * krat);

                        suma = (T)(suma + krat);
                    }

                    T zapis = (T)matice.GetNumber(i, j).Copy();
                    zapis = (T)(zapis - suma);

                    Q.WriteNumber(i, j, zapis);
                }

                T norma = new T();
                for (int j = 0; j < sloupce; j++) // vypočítá normu
                {
                    norma = (T)(norma + Q.GetNumber(i, j).__Exponentiate(2));
                }
                norma = (T)norma.__SquareRoot();
                R.WriteNumber(i, i, norma);
                for (int j = 0; j < sloupce; j++) // vydělí všechny složky vektoru
                {
                    Q.WriteNumber(i, j, (T)(Q.GetNumber(i, j) / norma));
                }
            }

            vysledek = ClassicOperations.StrassenWinograd(R, Q);

            return vysledek;
        }
    }
}
