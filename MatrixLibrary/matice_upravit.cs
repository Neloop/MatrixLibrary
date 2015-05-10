using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLibrary
{
    public static class Upravit
    {
        public static Matrix<T> Transponuj<T>(Matrix<T> matice) where T : MatrixNumberBase, new()
        {
            Matrix<T> vysledek;
            int radky, sloupce;
            radky = matice.Rows;
            sloupce = matice.Cols;
            vysledek = new Matrix<T>(sloupce, radky);

            for (int i = 0; i < sloupce; i++)
            {
                for (int j = 0; j < radky; j++)
                {
                    vysledek.WriteNumber(i, j, matice.GetNumber(j, i));
                }
            }
            
            return vysledek;
        }
        public static Matrix<T> Zesymetrizuj<T>(Matrix<T> matice) where T : MatrixNumberBase, new() // Zesymetrizuje zadanou matici, v případě nerovnosti řádků a sloupců vyhazuje vyjimku
        {
            Matrix<T> vysledek;
            if (matice.Rows == matice.Cols)
            {
                int rozmer;
                rozmer = matice.Rows;
                vysledek = new Matrix<T>(rozmer, rozmer);

                Matrix<T> transponovana;
                transponovana = Upravit.Transponuj(matice);
                vysledek = ClassicOperations.Addition(matice, transponovana);

                T dva = new T();
                dva.AddInt(2);
                for (int i = 0; i < rozmer; i++) // Vydělí všechna čísla dvěma
                {
                    for (int j = 0; j < rozmer; j++)
                    {
                        vysledek.WriteNumber(i, j, (T)(vysledek.GetNumber(i, j) / dva));
                    }
                }
            }
            else
            {
                throw new MatrixLibraryException("Given matrix does not have same number of rows and columns");
            }
            return vysledek;
        }
        public static Matrix<T> Gauss<T>(Matrix<T> matice) where T : MatrixNumberBase, new() // Pouze Gaussova eliminace, postupně se prochází řádky matice a "hledají" se pivoty
        {
            Matrix<T> vysledek;
            int radky, sloupce;
            radky = matice.Rows;
            sloupce = matice.Cols;
            vysledek = new Matrix<T>(radky, sloupce);

            for (int i = 0; i < radky; i++) // naplnění matice vysledek
            {
                for (int j = 0; j < sloupce; j++)
                {
                    vysledek.WriteNumber(i, j, matice.GetNumber(i, j));
                }
            }

            for (int i = 0; i < radky; i++)
            {
                for (int j = i; j < sloupce; j++)
                {
                    if (vysledek.GetNumber(i, j).IsZero())
                    {
                        // pokud je prvek nula, tak se koukne pod něj a případně prohodí řádek a vydělí řádky pod ním (potom se breakne), 
                        // pokud i pod ním jsou nuly, pak se breakne (nemusí prostě se nechá doběhnout) cyklus a jde na další sloupec
                        for (int k = i + 1; k < radky; k++)
                        {
                            if (!vysledek.GetNumber(k, j).IsZero())
                            {
                                for (int l = j; l < sloupce; l++)
                                {
                                    vysledek.WriteNumber(k, l, vysledek.GetNumber(i, l));
                                    vysledek.WriteNumber(i, l, vysledek.GetNumber(k, l));
                                }

                                T vydelit = vysledek.GetNumber(i, j);
                                for (int l = j; l < sloupce; l++)
                                {
                                    vysledek.WriteNumber(i, l, (T)(vysledek.GetNumber(i, l) / vydelit));
                                }

                                for (int a = i + 1; a < radky; a++)
                                {
                                    vydelit = vysledek.GetNumber(k, j);
                                    if (!vydelit.IsZero())
                                    {
                                        for (int b = j; b < sloupce; b++)
                                        {
                                            vysledek.WriteNumber(a, b, (T)(vysledek.GetNumber(a, b) / vydelit));
                                            vysledek.WriteNumber(a, b, (T)(vysledek.GetNumber(a, b) - vysledek.GetNumber(i, b)));
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                    else // Pokud se narazilo na nenulový prvek (=pivot)
                    {
                        T vydelit = vysledek.GetNumber(i, j);
                        for (int k = j; k < sloupce; k++)
                        {
                            vysledek.WriteNumber(i, k, (T)(vysledek.GetNumber(i, k) / vydelit));
                        }

                        for (int k = i + 1; k < radky; k++)
                        {
                            vydelit = vysledek.GetNumber(k, j);
                            if (!vydelit.IsZero())
                            {
                                for (int l = j; l < sloupce; l++)
                                {
                                    vysledek.WriteNumber(k, l, (T)(vysledek.GetNumber(k, l) / vydelit));
                                    vysledek.WriteNumber(k, l, (T)(vysledek.GetNumber(k, l) - vysledek.GetNumber(i, l)));
                                }
                            }
                        }

                        break;
                    }
                }
            }

            return vysledek;
        }
        public static Matrix<T> GaussJordan<T>(Matrix<T> matice) where T : MatrixNumberBase, new() // K počítání se používá pouze normální Gaussova eliminace, nejdříve na původní matici a pak na "obrácenou"
        {
            Matrix<T> vysledek;
            int radky, sloupce;
            radky = matice.Rows;
            sloupce = matice.Cols;
            vysledek = new Matrix<T>(radky, sloupce);
            int pulka_radky;
            int pulka_sloupce;
            if ((radky % 2) == 0) { pulka_radky = radky / 2; }
            else { pulka_radky = (radky / 2) + 1; }
            if ((sloupce % 2) == 0) { pulka_sloupce = sloupce / 2; }
            else { pulka_sloupce = (sloupce / 2) + 1; }

            vysledek = Upravit.Gauss(matice); // První Gaussovka

            int nul_radek_1, nul_radek_2;
            bool nul_radek = false;
            for (int i = 0; i < pulka_radky; i++) // Vymění se prvky v matici a následně se provede "obrácená" Gaussovka
            {
                nul_radek_1 = 0;
                nul_radek_2 = 0;
                for (int j = 0; j < sloupce; j++)
                {
                    if ((radky % 2) == 1 && pulka_sloupce == j && (pulka_radky - 1) == i) { break; }

                    if (vysledek.GetNumber(radky - i - 1, sloupce - j - 1).IsZero()) { nul_radek_1++; }
                    if (vysledek.GetNumber(i, j).IsZero()) { nul_radek_2++; }

                    vysledek.SwapElements(i, j, radky - i - 1, sloupce - j - 1);
                }
                if (nul_radek_1 == sloupce || nul_radek_2 == sloupce) { nul_radek = true; }
            }
            if (nul_radek == false) { vysledek = Upravit.Gauss(vysledek); }

            for (int i = 0; i < pulka_radky; i++) // Vymění se prvky v matici a vrátí výsledek
            {
                for (int j = 0; j < sloupce; j++)
                {
                    if ((radky % 2) == 1 && pulka_sloupce == j && (pulka_radky - 1) == i) { break; }

                    vysledek.SwapElements(i, j, radky - i - 1, sloupce - j - 1);
                }
            }

            return vysledek;
        }
        public static Matrix<T> Inverzni<T>(Matrix<T> matice) where T : MatrixNumberBase, new() // Pokud matice není regulární, tak vyhazuje vyjimku
        {
            Matrix<T> vysledek;
            if (Vlastnosti.Regularnost(matice) == true)
            {
                int radky, sloupce;
                radky = matice.Rows;
                sloupce = matice.Cols;
                vysledek = new Matrix<T>(radky, sloupce);
                int pulka_radky;
                int pulka_sloupce;
                if ((radky % 2) == 0) { pulka_radky = radky / 2; }
                else { pulka_radky = (radky / 2) + 1; }
                if ((sloupce % 2) == 0) { pulka_sloupce = sloupce / 2; }
                else { pulka_sloupce = (sloupce / 2) + 1; }

                Matrix<T> upravit; // Matice upravit má stejný počet řádků jako matice a 2x větší počet sloupců
                upravit = new Matrix<T>(radky, sloupce * 2);

                for (int i = 0; i < radky; i++) // Zapíše do matice upravit původní matici
                {
                    for (int j = 0; j < sloupce; j++)
                    {
                        upravit.WriteNumber(i, j, matice.GetNumber(i, j));
                    }
                }
                T jedna = new T();
                jedna.AddInt(1);
                T nula = new T();
                for (int i = 0; i < radky; i++) // Zapíše do matice upravit jednotkovou matici
                {
                    for (int j = sloupce; j < (sloupce * 2); j++)
                    {
                        if (i == (j - sloupce)) { upravit.WriteNumber(i, j, jedna); }
                        else { upravit.WriteNumber(i, j, nula); }
                    }
                }

                upravit = Upravit.Gauss(upravit); // První Gaussovka

                // Převrácení a druhá Gaussovka:

                for (int i = 0; i < pulka_radky; i++) // Převrácení
                {
                    for (int j = 0; j < sloupce; j++)
                    {
                        if ((radky % 2) == 1 && pulka_sloupce == j && (pulka_radky - 1) == i) { break; }
                        upravit.SwapElements(i, j, radky - i - 1, sloupce - j - 1);

                        // Převrácení původně jednotkové matice:
                        upravit.SwapElements(i, sloupce + j, radky - i - 1, (sloupce * 2) - j - 1);
                    }
                }

                upravit = Upravit.Gauss(upravit); // Druhá Gaussovka

                for (int i = 0; i < radky; i++) // Převrácení a složení výsledné matice
                {
                    for (int j = 0; j < sloupce; j++)
                    {
                        vysledek.WriteNumber(radky - i - 1, sloupce - j - 1, upravit.GetNumber(i, j + sloupce));
                    }
                }
            }
            else
            {
                throw new MatrixLibraryException("Given matrix is not regular");
            }
            return vysledek;
        }
        public static Matrix<T> Adjungovana<T>(Matrix<T> matice) where T : MatrixNumberBase, new()
        {
            Matrix<T> vysledek;
            int radky, sloupce;
            radky = matice.Rows;
            sloupce = matice.Cols;
            vysledek = new Matrix<T>(radky, sloupce);

            for (int i = 0; i < radky; i++)
            {
                for (int j = 0; j < sloupce; j++)
                {
                    T nasobit;
                    if ((i + j) % 2 == 0) { nasobit = new T(); nasobit.AddInt(1); }
                    else { nasobit = new T(); nasobit.AddInt(-1); }

                    Matrix<T> upravena;
                    upravena = new Matrix<T>(radky - 1, sloupce - 1);

                    int odectik = 0;

                    for (int k = 0; k < radky; k++)
                    {
                        if (k == j) { odectik = 1; continue; }
                        int odectil = 0;

                        for (int l = 0; l < sloupce; l++)
                        {
                            if (l == i)
                            {
                                odectil = 1; continue;
                            }
                            else
                            {
                                T zapis = matice.GetNumber(k, l);
                                upravena.WriteNumber(k - odectik, l - odectil, zapis);
                            }
                        }
                    }

                    T tmp = Vypocty.Determinant(upravena);
                    tmp = (T)(nasobit * tmp);

                    vysledek.WriteNumber(i, j, tmp);
                }
            }

            return vysledek;
        }
        public static Matrix<T> Ortogonalizace<T>(Matrix<T> matice) where T : MatrixNumberBase, new() // Využívá Gram-Schmidtovu ortogonalizaci, vstupem by měli být lineárně nezávislé vektory
        {
            Matrix<T> vysledek;
            int radky, sloupce;
            radky = matice.Rows;
            sloupce = matice.Cols;
            vysledek = new Matrix<T>(radky, sloupce);

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
                            T z = (T)vysledek.GetNumber(k, l).Copy();

                            skal_soucin = (T)((x * z) + skal_soucin);
                        }

                        T krat = (T)vysledek.GetNumber(k, j).Copy();
                        krat = (T)(skal_soucin * krat);

                        suma = (T)(suma + krat);
                    }

                    T zapis = (T)(matice.GetNumber(i, j) - suma);
                    vysledek.WriteNumber(i, j, zapis);
                }

                T norma = new T();
                for (int j = 0; j < sloupce; j++) // vypočítá normu
                {
                    norma = (T)(norma + vysledek.GetNumber(i, j).__Exponentiate(2));
                }
                norma = (T)norma.__SquareRoot();
                for (int j = 0; j < sloupce; j++) // vydělí všechny složky vektoru
                {
                    vysledek.WriteNumber(i, j, (T)(vysledek.GetNumber(i, j) / norma));
                }
            }

            return vysledek;
        }
    }
}
