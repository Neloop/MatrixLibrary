using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLibrary
{
    public static class Charakteristika
    {
        public static Vl_cisla<T> Vlastni_cisla<T>(Matrix<T> matice, int limit) where T : MatrixNumberBase, new() // Pokud je limit nula, algoritmus počítá dokud vlastní čísla nenajde, určitý integer pak vyjadřuje počet opakování cyklu na zjištění podobné matice
        {
            Vl_cisla<T> vysledek;
            if (limit < 0) { limit = -limit; }

            if (matice.Rows == matice.Cols)
            {
                Matrix<T> Q, R, RQ;
                Rozklady.QRRozklad(matice, out Q, out R);
                RQ = Upravit.Transponuj(Q) * matice * Q;

                bool skoncit_1 = false;
                bool skoncit_2 = false;
                bool prvni = true;
                while (skoncit_1 == false && skoncit_2 == false)
                {
                    skoncit_1 = true;
                    skoncit_2 = true;
                    if (prvni == false)
                    {
                        Rozklady.QRRozklad(RQ, out Q, out R);
                        RQ = Upravit.Transponuj(Q) * RQ * Q;
                    }
                    prvni = false;
                    for (int i = 1; i < RQ.Rows; i++)
                    {
                        for (int j = 0; j < i; j++)
                        {
                            if (!RQ.GetNumber(i, j).IsZero()) { skoncit_1 = false; }
                        }
                    }
                    for (int i = 1; i < RQ.Cols; i++)
                    {
                        for (int j = 0; j < i; j++)
                        {
                            if (!RQ.GetNumber(j, i).IsZero()) { skoncit_2 = false; }
                        }
                    }
                    if (limit == 1)
                    {
                        if (skoncit_1 == false && skoncit_2 == false)
                        {
                            throw new EigenValuesNotFoundException("Cannot find eigenvalues in specified limit!");
                        }
                    }
                    if (limit != 0) { limit--; }
                }

                int nulovy;
                List<T> vysl = new List<T>();
                for (int i = 0; i < RQ.Rows; i++)
                {
                    bool zapis = true;
                    for (int j = 0; j < vysl.Count; j++)
                    {
                        if (vysl[j] == RQ.GetNumber(i, i)) { zapis = false; }
                    }
                    if (zapis == true)
                    {
                        vysl.Add(RQ.GetNumber(i, i));
                    }

                    nulovy = 0;
                    for (int j = 0; j < RQ.Cols; j++)
                    {
                        if (RQ.GetNumber(i, j).IsZero()) { nulovy++; }
                    }
                    if (nulovy == RQ.Cols)
                    {
                        throw new EigenValuesNotFoundException("Matrix was not regular!");
                    }
                }

                T[] temp = new T[vysl.Count];
                for (int i = 0; i < vysl.Count; i++) { temp[i] = vysl[i]; }
                vysledek = new Vl_cisla<T>(temp, vysl.Count);
            }
            else
            {
                throw new MatrixLibraryException("Rows and cols are not equal");
            }


            return vysledek;
        }
        public static Matrix<T> Vlastni_vektory<T>(Matrix<T> matice, out Vl_cisla<T> vl_cisla, int limit) where T : MatrixNumberBase, new()
        {
            Matrix<T> vysledek;
            if (matice.Rows == matice.Cols)
            {
                vl_cisla = Charakteristika.Vlastni_cisla(matice, limit);
                vysledek = new Matrix<T>(vl_cisla.Pocet(), matice.Cols);
                Matrix<T> upravena = new Matrix<T>(matice.Rows, matice.Cols);
                Matrix<T> nulovy = new Matrix<T>(matice.Rows, 1);
                Matrix<T> soustava;

                for (int i = 0; i < vl_cisla.Pocet(); i++)
                {
                    for (int k = 0; k < upravena.Rows; k++)
                    {
                        for (int l = 0; l < upravena.Cols; l++)
                        {
                            if (k == l)
                            {
                                upravena.WriteNumber(k, l, (T)(matice.GetNumber(k, l) - vl_cisla.Vrat_cislo(i)));
                            }
                            else
                            {
                                upravena.WriteNumber(k, l, matice.GetNumber(k, l));
                            }
                        }
                    }

                    soustava = Vypocty.SoustavaRovnic(upravena, nulovy);

                    for (int k = 0; k < soustava.Rows; k++)
                    {
                        T soucet = null;
                        for (int l = 0; l < soustava.Cols; l++)
                        {
                            if (l != 0) { soucet = (T)(soucet + soustava.GetNumber(k, l)); }
                            else { soucet = (T)soustava.GetNumber(k, l); }
                        }
                        vysledek.WriteNumber(i, k, soucet);
                    }
                }
            }
            else
            {
                throw new MatrixLibraryException("Rows and cols are not equal");
            }

            return vysledek;
        }
        public static Matrix<T> Diagonalizovat<T>(Matrix<T> matice, out Matrix<T> S, int limit) where T : MatrixNumberBase, new() // Pomocí vlastních čísel určí diagonální matici a vrací jí
        {
            Matrix<T> vysledek;

            if (matice.Rows == matice.Cols)
            {
                Vl_cisla<T> vl_cisla;
                S = Charakteristika.Vlastni_vektory(matice, out vl_cisla, limit);

                if (S.Rows == S.Cols)
                {
                    vysledek = new Matrix<T>(S.Rows, S.Cols);
                    for (int i = 0; i < vl_cisla.Pocet(); i++)
                    {
                        vysledek.WriteNumber(i, i, vl_cisla.Vrat_cislo(i));
                    }
                }
                else
                {
                    throw new MatrixLibraryException("Rows and cols are not equal");
                }
            }
            else
            {
                throw new MatrixLibraryException("Rows and cols are not equal");
            }

            return vysledek;
        }
    }
}
