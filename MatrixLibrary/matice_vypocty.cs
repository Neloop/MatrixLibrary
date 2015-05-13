using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MatrixLibrary
{
    public static class Vypocty
    {
        public static T Determinant<T>(Matrix<T> matice) where T : MatrixNumberBase, new() // Pokud matice není regulární je vracena 0
        {
            T vynasobit = new T();
            vynasobit.AddInt(1);

            Matrix<T> upravena;
            int radky, sloupce;
            radky = matice.Rows;
            sloupce = matice.Cols;
            upravena = new Matrix<T>(radky, sloupce);

            for (int i = 0; i < radky; i++) // naplnění matice upravena
            {
                for (int j = 0; j < sloupce; j++)
                {
                    upravena.WriteNumber(i, j, matice.GetNumber(i, j));
                }
            }

            int pricti = 0;

            for (int i = 0; i < radky; i++) // První cyklus projde všechny řádky a od každého spustí další cyklus na vynulování sloupce pod pivotem
            {
                bool pivot = true;
                for (int j = i + pricti; j < radky; j++)
                {
                    if (pivot == true) // Pivot na každém řádku se změní na jedničku
                    {
                        pivot = false;
                        T vydelit = (T)upravena.GetNumber(i, j).Copy();

                        if (upravena.GetNumber(i, j).IsZero())
                        {
                            bool skoncit = false;
                            for (int l = j; l < sloupce; l++)
                            {
                                for (int k = i + 1; k < radky; k++)
                                {
                                    if (!upravena.GetNumber(k, l).IsZero() && upravena.GetNumber(i, l).IsZero()) // Vymění dva řádky, aby ten s méně nenulovými sloupci byl na vrchu
                                    {
                                        for (int s = l; s < sloupce; s++)
                                        {
                                            upravena.SwapElements(k, s, i, s);
                                        }

                                        vynasobit = (T)(-vynasobit);

                                        pricti = pricti + (l - j);
                                        j = j + pricti;
                                        skoncit = true;
                                        break;
                                    }
                                }
                                if (skoncit == true) // Pokud se povedlo vyměnit dva řádky
                                {
                                    vydelit = (T)upravena.GetNumber(i, j).Copy();
                                    vynasobit = (T)(vydelit * vynasobit);
                                    break;
                                }
                                if (upravena.GetNumber(i, l).IsZero()) // Pouze v případě, že byl celý sloupec nulový, tak se přičte 1 ke zpracovávaným indexům sloupců
                                {
                                    pricti++;
                                    j = j + pricti;
                                    if (pricti >= upravena.Cols) { return new T(); }
                                }
                            }
                        }

                        vynasobit = (T)(vydelit * vynasobit); // Číslo, kterým se bude determinant ve výsledku násobit

                        for (int k = i + pricti; k < sloupce; k++)
                        {
                            upravena.WriteNumber(i, k, (T)(upravena.GetNumber(i, k) / vydelit));
                        }
                    }
                    else
                    {
                        T vydelit = (T)upravena.GetNumber(j, i + pricti).Copy();

                        if (vydelit.IsZero()) // pokud je už prvek vynulován, pokračuje se na dalším řádku
                        {
                            continue;
                        }

                        vynasobit = (T)(vydelit * vynasobit);

                        for (int k = i + pricti; k < sloupce; k++)
                        {
                            upravena.WriteNumber(j, k, (T)(upravena.GetNumber(j, k) / vydelit));
                        }
                        for (int l = i + pricti; l < sloupce; l++)
                        {
                            T tmp = upravena.GetNumber(j, l);
                            tmp = (T)(tmp - upravena.GetNumber(i, l));
                            upravena.WriteNumber(j, l, tmp);
                        }
                    }
                }
            }

            T vysledek = new T();
            vysledek.AddInt(1);
            for (int i = 0; i < radky; i++) // Vynásobí prvky na diagonále
            {
                vysledek = (T)(upravena.GetNumber(i, i) * vysledek);
            }

            vysledek = (T)(vynasobit * vysledek);

            return vysledek;
        }
        public static Matrix<T> Cramer<T>(Matrix<T> matice, Matrix<T> b) where T : MatrixNumberBase, new() // Vrací vlastně vektor n*1; vstupem musí být regulární matice
        {
            Matrix<T> vysledek;
            int radky, sloupce;
            radky = matice.Rows;
            sloupce = matice.Cols;
            vysledek = new Matrix<T>(radky, 1);

            Matrix<T> det = new Matrix<T>(radky, sloupce);

            T determinant = Vypocty.Determinant(matice);

            for (int i = 0; i < sloupce; i++)
            {
                for (int k = 0; k < radky; k++) // sestavení matice, kde je i-tý sloupec nahrazen sloupcem b
                {
                    for (int l = 0; l < sloupce; l++)
                    {
                        if (i == l)
                        {
                            det.WriteNumber(k, l, b.GetNumber(k, 0));
                        }
                        else
                        {
                            det.WriteNumber(k, l, matice.GetNumber(k, l));
                        }
                    }
                }

                T x = Vypocty.Determinant(det);
                x = (T)(x / determinant);

                vysledek.WriteNumber(i, 0, x);
            }

            return vysledek;
        }
        public static Matrix<T> SoustavaRovnic<T>(Matrix<T> matice, Matrix<T> b) where T : MatrixNumberBase, new() // Vrací sloupcové vektory: první je partikulární část, další jsou obecné části (jeden sloupec = jeden parametr)
        {
            /*
             * 
             * Výsledek je ve tvaru: x = (x1,x2,x3,...) + [t*(t1,t2,t3,...) + s*(s1,s2,s3,...) + ...]
             *  - vektory jsou ve výsledné matici sloupce
             * 
             */

            Matrix<T> vysledek;
            int radky, sloupce;
            radky = matice.Rows;
            sloupce = matice.Cols;
            int pulka_radky;
            int pulka_sloupce;
            if ((radky % 2) == 0) { pulka_radky = radky / 2; }
            else { pulka_radky = (radky / 2) + 1; }
            if ((sloupce % 2) == 0) { pulka_sloupce = sloupce / 2; }
            else { pulka_sloupce = (sloupce / 2) + 1; }

            Matrix<T> uprav = new Matrix<T>(radky, sloupce + 1);

            for (int i = 0; i < radky; i++) // Naplnění matice 'uprav' maticí 'matice' a sloupcem 'b'
            {
                uprav.WriteNumber(i, sloupce, b.GetNumber(i, 0));
                for (int j = 0; j < sloupce; j++)
                {
                    uprav.WriteNumber(i, j, matice.GetNumber(i, j));
                }
            }

            uprav = AlteringOperations.Gauss(uprav); // První gaussovka

            /*
             * 
             * Následuje projití matice 'uprav' jestli v ní nejsou nulové řádky a vyměnění prvků pro druhou gaussovku
             *  a pak podmínka která se rozděluje:
             *   - Pokud matice není regulární nebo má nulové řádky
             *      > potom se neprovádí druhá gaussovka a rovnou se sestavuje řešení
             *   - Matice má stejný počet řádků a sloupců
             *      > matice 'uprav' se projede druhou gaussovkou a tím se získá řešení
             * 
             */

            int nulovy = 0;
            for (int i = 0; i < pulka_radky; i++) // Vymění se prvky v matici 'uprav'
            {
                int nuly_1 = 0;
                int nuly_2 = 0;

                uprav.SwapElements(i, sloupce, radky - i - 1, sloupce);

                for (int j = 0; j < sloupce; j++)
                {
                    if (uprav.GetNumber(i, j).IsZero()) { nuly_1++; }
                    if (uprav.GetNumber(radky - i - 1, sloupce - j - 1).IsZero()) { nuly_2++; }
                    if ((radky % 2) == 1 && pulka_sloupce == j && (pulka_radky - 1) == i) { break; }

                    uprav.SwapElements(i, j, radky - i - 1, sloupce - j - 1);
                }
                if (nuly_2 == sloupce)
                {
                    nulovy++;

                    if (!uprav.GetNumber(i, sloupce).IsZero())
                    {
                        return new Matrix<T>(1, 1);
                    }
                }
                if (nuly_1 == sloupce)
                {
                    nulovy++;

                    if (!uprav.GetNumber(radky - i - 1, sloupce).IsZero())
                    {
                        return new Matrix<T>(1, 1);
                    }
                }
            }

            if ((radky - nulovy) == sloupce) // Matice má jedno možné řešení
            {
                uprav = AlteringOperations.Gauss(uprav);
                vysledek = new Matrix<T>(radky, 1);
                for (int i = 0; i < radky; i++)
                {
                    vysledek.WriteNumber(i, 0, uprav.GetNumber(radky - i - 1, sloupce));
                }
            }
            else
            {
                vysledek = new Matrix<T>(radky, nulovy + 1);
                List<int> parametry = new List<int>();
                Matrix<T> vyparametrizuj = new Matrix<T>(radky - nulovy, sloupce + 1);
                int pricti = 0;
                for (int i = 0; i < vyparametrizuj.Rows; i++) // Zapsání a zpřeházení z matice 'uprav' do matice 'vyparametrizuj'
                {
                    vyparametrizuj.WriteNumber(i, sloupce, uprav.GetNumber(radky - i - 1, sloupce));

                    for (int j = 0; j < sloupce; j++)
                    {
                        vyparametrizuj.WriteNumber(i, j, uprav.GetNumber(radky - i - 1, sloupce - j - 1));
                    }
                    for (int j = i + pricti; j < sloupce; j++) // Určení, co budou parametry
                    {
                        if (!vyparametrizuj.GetNumber(i, j).IsOne())
                        {
                            parametry.Add(j);
                            pricti++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                for (int i = vyparametrizuj.Rows + parametry.Count; i < (vyparametrizuj.Cols - 1); i++) // Dopsání zbývající parametrů
                {
                    parametry.Add(i);
                }

                for (int i = 0; i < parametry.Count; i++) // Zapsání parametrů do výsledku
                {
                    T jedna = new T();
                    jedna.AddInt(1);
                    vysledek.WriteNumber(parametry[i], i + 1, jedna);
                }

                for (int i = (vyparametrizuj.Rows - 1); i >= 0; i--)
                {
                    for (int j = 0; j < vyparametrizuj.Cols; j++)
                    {
                        if (vyparametrizuj.GetNumber(i, j).IsOne())
                        {
                            vysledek.WriteNumber(j, 0, vyparametrizuj.GetNumber(i, vyparametrizuj.Cols - 1));
                            for (int k = (j + 1); k < (vyparametrizuj.Cols - 1); k++) // Jde po prvcích v matici vyparametrizuj
                            {
                                for (int l = 0; l < vysledek.Cols; l++) // Dosazuje z už vypočítaných výsledků
                                {
                                    T temp = (T)(vysledek.GetNumber(k, l) * (-vyparametrizuj.GetNumber(j, k)));
                                    temp = (T)(temp + vysledek.GetNumber(j, l));
                                    vysledek.WriteNumber(j, l, temp);
                                }
                            }
                            break;
                        }
                    }
                }
            }

            return vysledek;
        }
    }
}
