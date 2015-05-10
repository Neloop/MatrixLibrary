using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLibrary
{
    public static class Vlastnosti
    {
        public static bool Regularnost<T>(Matrix<T> matice) where T : MatrixNumberBase, new()
        {
            bool vysledek = true;
            if (matice.Rows == matice.Cols)
            {
                int tmp = 0;

                Matrix<T> docasna;
                int radky, sloupce;
                radky = matice.Rows;
                sloupce = matice.Cols;
                docasna = new Matrix<T>(radky, sloupce);

                for (int i = 0; i < radky; i++) // Naplnění dočasné matice
                {
                    for (int j = 0; j < sloupce; j++)
                    {
                        docasna.WriteNumber(i, j, matice.GetNumber(i, j));
                    }
                }

                docasna = Upravit.Gauss(docasna);
                for (int i = 0; i < matice.Rows; i++)
                {
                    for (int j = 0; j < matice.Cols; j++)
                    {
                        tmp = j;
                        if (!matice.GetNumber(i, j).IsZero()) { break; }
                    }
                    if ((tmp + 1) == matice.Cols && matice.GetNumber(i, tmp).IsZero()) { vysledek = false; }
                }
            }
            else
            {
                vysledek = false;
            }
            return vysledek;
        }
        public static int Hodnost<T>(Matrix<T> matice) where T : MatrixNumberBase, new()
        {
            int vysledek = matice.Rows;

            Matrix<T> gauss = Upravit.Gauss(matice);

            for (int i = 0; i < gauss.Rows; i++)
            {
                int nuly = 0;
                for (int j = 0; j < gauss.Cols; j++)
                {
                    if (gauss.GetNumber(i, j).IsZero()) { nuly++; }
                }
                if (nuly == gauss.Cols) { vysledek--; }
            }

            return vysledek;
        }
        public static bool Ortogonalnost<T>(Matrix<T> matice) where T : MatrixNumberBase, new()
        {
            bool vysledek = true;
            Matrix<T> nasobena;
            int radky, sloupce;
            radky = matice.Rows;
            sloupce = matice.Cols;

            if (radky == sloupce)
            {
                Matrix<T> transponovana = Upravit.Transponuj(matice);
                nasobena = ClassicOperations.StrassenWinograd(transponovana, matice);

                for (int i = 0; i < nasobena.Rows; i++)
                {
                    for (int j = 0; j < nasobena.Cols; j++)
                    {
                        if (i == j)
                        {
                            if (!nasobena.GetNumber(i, j).IsOne()) { vysledek = false; }
                        }
                        else
                        {
                            if (!nasobena.GetNumber(i, j).IsZero()) { vysledek = false; }
                        }
                    }
                }
            }
            else { vysledek = false; }

            return vysledek;
        }
        public static int Definitnost<T>(Matrix<T> matice) where T : MatrixNumberBase, new() // Rozlišuje se definitnost (pozitivní/negativní), indefinitnost; podle vráceného čísla
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
            int vysledek = 0;

            if (matice.Rows == matice.Cols)
            {
                int pocet = matice.Rows;
                T[] determinant = new T[pocet];

                for (int i = 0; i < pocet; i++)
                {
                    Matrix<T> det = new Matrix<T>(i + 1, i + 1);
                    for (int k = 0; k < (i + 1); k++)
                    {
                        for (int l = 0; l < (i + 1); l++)
                        {
                            det.WriteNumber(k, l, matice.GetNumber(k, l));
                        }
                    }
                    determinant[i] = Vypocty.Determinant(det);
                }

                int pozitivne = 0;
                bool negativne = true;
                T nula = new T();
                for (int i = 0; i < pocet; i++)
                {
                    if (determinant[i] > nula) { pozitivne++; }
                    if ((i % 2) == 0 && determinant[i] >= nula) { negativne = false; }
                    if ((i % 2) == 1 && determinant[i] <= nula) { negativne = false; }
                }

                if (pozitivne == pocet) { vysledek = 1; }
                if (negativne == true) { vysledek = 2; }
            }
            else
            {
                vysledek = 0;
            }

            return vysledek;
        }
    }
}
