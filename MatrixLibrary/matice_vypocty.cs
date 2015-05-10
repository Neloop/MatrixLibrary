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
                                    if (pricti == upravena.Cols) { return new T(); }
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

            uprav = Upravit.Gauss(uprav); // První gaussovka

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
                uprav = Upravit.Gauss(uprav);
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
        public static Matrix<T> Rovnice<T>(string rce, Matrix<T>[] matice) where T : MatrixNumberBase, new() // vstupem je string s popisem rovnice a pole Matic použitých v rovnici
        {
            /*
             * Rovnice musí mít tvar X = ...:
             * - první matice (X) není reprezentovaná v poli Matic
             * - Názvy matic musí být jednopísmenné z okruhu A-Z či a-z, bez diakritiky
             * - i když se matice v rovnici opakují je nutno je znovu zařadit do pole Matic
             * - je povolené jakékoliv odsazování (mezery/prázdná místa)
             * Operace, které je možno v rovnici použít:
             * - násobení matic (reprezentované znakem *)
             * - násobení číslem (stejné jako s násobením matic) - (číslo může být buď pouze před maticí nebo před závorkou, samozřejmě včetně operace *)
             * - sčítání a odčítání matic (znak +/-)
             * - inverzní matice (posloupnost znaků ^-1)
             * - umocňování matice (znaky ^x , kde x je libovolné celé číslo)
             * - libovolné uzávorkování (pouze znaky () )
             */

            Matrix<T> vysledek;

            /* 
             * 
             * Kontrola vstupního stringu a převedení do postfixu:
             * 
             */
            Stack<char> znamenka_prevod = new Stack<char>(); // Slouží k převodu na postfix
            Queue<char> rovnice_postfix = new Queue<char>(); // Slouží k uložení notace: C - číslo, A - neupravená matice, B - upravená matice, O - operace
            Queue<T> cisla_postfix = new Queue<T>();
            Queue<char> operace_postfix = new Queue<char>();
            Queue<Matrix<T>> matice_postfix = new Queue<Matrix<T>>();
            Queue<Matrix<T>> matice_upr_postfix = new Queue<Matrix<T>>();
            bool cislo = true;
            bool pismeno = true;
            bool operace_krat = false;
            bool operace_plus = false;
            bool l_zavorka = true;
            bool p_zavorka = false;
            bool konec = false; // Pokud se v průběhu nastaví na true, pak má výraz špatnou syntaxi
            bool x = false; // True je pokud je na začátku X = ...
            int zacni = 0;

            while (rce[zacni] != '=') // nalezení fragmentu X = ...
            {
                if (rce[zacni] == ' ' || rce[zacni] == 'X' || rce[zacni] == 'x')
                {
                    if (rce[zacni] == 'X' || rce[zacni] == 'x')
                    {
                        x = true;
                    }
                }
                else { konec = true; break; }
                if ((zacni + 1) < rce.Length) { zacni++; }
                else { konec = true; break; }
            }
            if (x == false) { konec = true; }
            else { zacni++; }
            for (int i = zacni; i < rce.Length; i++)
            {
                if (konec == true) { break; }
                if (rce[i] >= '0' && rce[i] <= '9') // pokud se našlo číslo
                {
                    if (cislo == false) { konec = true; break; }
                    else { cislo = false; pismeno = false; operace_krat = true; operace_plus = false; l_zavorka = false; p_zavorka = false; }
                    int tmp_cislo = 48 - rce[i];
                    i++;
                    while (rce[i] >= '0' && rce[i] <= '9')
                    {
                        tmp_cislo = (tmp_cislo * 10) + (48 - rce[i]);
                        i++;
                    }
                    rovnice_postfix.Enqueue('C');
                    T tmp = new T();
                    tmp.AddInt(tmp_cislo);
                    cisla_postfix.Enqueue(tmp);
                }

                if (rce[i] == ' ') { continue; } // Prázdný znak/mezera je přeskočena

                if ((rce[i] >= 'A' && rce[i] <= 'Z') || (rce[i] >= 'a' && rce[i] <= 'z')) // Byla nalezena matice
                {
                    if (pismeno == false) { konec = true; break; }
                    else { cislo = false; pismeno = false; operace_krat = true; operace_plus = true; l_zavorka = false; p_zavorka = true; }

                    if ((i + 1) < rce.Length)
                    {
                        i++;
                        while (rce[i] == ' ' || rce[i] == '^') // Nalezeno umocňování, či inverzní matice
                        {
                            if (rce[i] == ' ')
                            {
                                if ((i + 1) < rce.Length)
                                {
                                    i++;
                                    continue;
                                }
                                else { break; }
                            }
                            if (rce[i] == '^')
                            {
                                if ((i + 1) < rce.Length)
                                {
                                    i++;
                                    while (rce[i] == ' ') { if ((i + 1) < rce.Length) { i++; } else { konec = true; break; } }
                                }
                                if (rce[i] == '-')
                                {
                                    if ((i + 1) < rce.Length)
                                    {
                                        i++;
                                        while (rce[i] == ' ') { if ((i + 1) < rce.Length) { i++; } else { konec = true; break; } }
                                    }
                                    else { konec = true; break; }
                                    int tmp_cislo = 48 - rce[i];
                                    if ((i + 1) < rce.Length)
                                    {
                                        i++;
                                        while (rce[i] >= '0' && rce[i] <= '9')
                                        {
                                            tmp_cislo = (tmp_cislo * 10) + (48 - rce[i]);
                                            if ((i + 1) < rce.Length)
                                            {
                                                i++;
                                            }
                                            else { break; }
                                        }
                                    }
                                    else { konec = true; break; }
                                    Matrix<T> inverz;
                                    inverz = ClassicOperations.Exponentiate(matice[matice_postfix.Count + matice_upr_postfix.Count], tmp_cislo);
                                    inverz = Upravit.Inverzni(inverz);
                                    rovnice_postfix.Enqueue('B');
                                    matice_upr_postfix.Enqueue(inverz);
                                    break;
                                }
                                else
                                {
                                    if (rce[i] >= '0' && rce[i] <= '9')
                                    {
                                        int tmp_cislo = 48 - rce[i];
                                        if ((i + 1) < rce.Length)
                                        {
                                            i++;
                                            while (rce[i] >= '0' && rce[i] <= '9')
                                            {
                                                tmp_cislo = (tmp_cislo * 10) + (48 - rce[i]);
                                                if ((i + 1) < rce.Length)
                                                {
                                                    i++;
                                                }
                                                else { break; }
                                            }
                                        }

                                        Matrix<T> umocnit = ClassicOperations.Exponentiate(matice[matice_postfix.Count + matice_upr_postfix.Count], tmp_cislo);
                                        rovnice_postfix.Enqueue('B');
                                        matice_upr_postfix.Enqueue(umocnit);
                                        break;
                                    }
                                    else { konec = true; break; }
                                }
                            }
                        }
                    }
                    if (konec == true) { break; }

                    rovnice_postfix.Enqueue('A');
                    matice_postfix.Enqueue(matice[matice_postfix.Count + matice_upr_postfix.Count]);
                }

                if (rce[i] == '*') // pokud se našlo násobení
                {
                    if (operace_krat == false) { konec = true; break; }
                    else { cislo = false; pismeno = true; operace_krat = false; operace_plus = false; l_zavorka = true; p_zavorka = false; }

                    while (znamenka_prevod.Count != 0 && znamenka_prevod.Peek() == '*')
                    {
                        znamenka_prevod.Pop();
                        rovnice_postfix.Enqueue('O');
                        operace_postfix.Enqueue('*');
                        if (znamenka_prevod.Count == 0) { break; }
                    }
                    znamenka_prevod.Push('*');
                }

                if (rce[i] == '+' || rce[i] == '-') // pokud se našlo sčítání/odčítání
                {
                    if (operace_plus == false) { konec = true; break; }
                    else { cislo = false; pismeno = true; operace_krat = false; operace_plus = false; l_zavorka = true; p_zavorka = false; }

                    while (znamenka_prevod.Count != 0 && (znamenka_prevod.Peek() == '*' || znamenka_prevod.Peek() == '+' || znamenka_prevod.Peek() == '-'))
                    {
                        char znak = znamenka_prevod.Pop();
                        rovnice_postfix.Enqueue('O');
                        operace_postfix.Enqueue(znak);
                        if (znamenka_prevod.Count == 0) { break; }
                    }
                    znamenka_prevod.Push(rce[i]);
                }

                if (rce[i] == '(') // pokud se našla levá závorka
                {
                    if (l_zavorka == false) { konec = true; break; }
                    else { cislo = true; pismeno = true; operace_krat = false; operace_plus = false; l_zavorka = true; p_zavorka = false; }

                    znamenka_prevod.Push(rce[i]);
                }

                if (rce[i] == ')') // pokud se našla pravá závorka
                {
                    if (p_zavorka == false) { konec = true; break; }
                    else { cislo = false; pismeno = false; operace_krat = true; operace_plus = true; l_zavorka = false; p_zavorka = true; }

                    while (znamenka_prevod.Count != 0 && znamenka_prevod.Peek() != '(')
                    {
                        char znak = znamenka_prevod.Pop();
                        rovnice_postfix.Enqueue('O');
                        operace_postfix.Enqueue(znak);
                        if (znamenka_prevod.Count == 0) { konec = true; break; }
                    }
                    znamenka_prevod.Pop();
                }
                if (konec == true) { break; }

                if (rce[i] == ' ' || rce[i] == '(' || rce[i] == ')' || rce[i] == '*' || rce[i] == '+' || rce[i] == '-' ||
                    (rce[i] >= '0' && rce[i] <= '9') || (rce[i] >= 'A' && rce[i] <= 'Z') || (rce[i] >= 'a' && rce[i] <= 'z'))
                { }
                else { konec = true; break; }
            }
            int count = znamenka_prevod.Count;
            for (int i = 0; i < count; i++) // na konci převodu zajišťuje vyprázdnění zásobníku
            {
                char znak = znamenka_prevod.Pop();
                rovnice_postfix.Enqueue('O');
                operace_postfix.Enqueue(znak);
            }
            /*
             * 
             * Konec kontroly a převádění do postfixu
             * 
             */

            // Samotné vyhodnocení výrazu v postfixu:
            if (konec == false)
            {
                Stack<char> zasobnik_vyhodnot = new Stack<char>(); // Uložené hodnoty: A - matice, B - upravená matice, C - číslo
                Stack<Matrix<T>> matice_vyhodnot = new Stack<Matrix<T>>();
                Stack<Matrix<T>> matice_upr_vyhodnot = new Stack<Matrix<T>>();
                Stack<T> cislo_vyhodnot = new Stack<T>();

                count = rovnice_postfix.Count;
                for (int i = 0; i < count; i++)
                {
                    char znak = rovnice_postfix.Dequeue();
                    switch (znak)
                    {
                        case 'C':
                            zasobnik_vyhodnot.Push('C');
                            T cislo_tmp = cisla_postfix.Dequeue();
                            cislo_vyhodnot.Push(cislo_tmp);
                            break;
                        case 'A':
                            zasobnik_vyhodnot.Push('A');
                            Matrix<T> matice_tmp = matice_postfix.Dequeue();
                            matice_vyhodnot.Push(matice_tmp);
                            break;
                        case 'B':
                            zasobnik_vyhodnot.Push('B');
                            Matrix<T> matice_upr_tmp = matice_upr_postfix.Dequeue();
                            matice_upr_vyhodnot.Push(matice_upr_tmp);
                            break;
                        case 'O':
                            char operace = operace_postfix.Dequeue();
                            switch (operace)
                            {
                                case '*':
                                    char tmp_1 = zasobnik_vyhodnot.Pop();
                                    char tmp_2 = zasobnik_vyhodnot.Pop();
                                    if (tmp_1 == 'C' || tmp_2 == 'C') // Pokud je jedno číslo...
                                    {
                                        if (tmp_2 == 'C')
                                        {
                                            T A = cislo_vyhodnot.Pop();
                                            if (tmp_1 == 'A')
                                            {
                                                Matrix<T> B = matice_vyhodnot.Pop();
                                                Matrix<T> vysl;
                                                vysl = ClassicOperations.MultiplyWithNumber(B, A);
                                                zasobnik_vyhodnot.Push('B');
                                                matice_upr_vyhodnot.Push(vysl);
                                            }
                                            else
                                            {
                                                Matrix<T> B = matice_upr_vyhodnot.Pop();
                                                B = ClassicOperations.MultiplyWithNumber(B, A);
                                                zasobnik_vyhodnot.Push('B');
                                                matice_upr_vyhodnot.Push(B);
                                            }
                                        }
                                        else
                                        {
                                            T A = cislo_vyhodnot.Pop();
                                            if (tmp_2 == 'A')
                                            {
                                                Matrix<T> B = matice_vyhodnot.Pop();
                                                Matrix<T> vysl;
                                                vysl = ClassicOperations.MultiplyWithNumber(B, A);
                                                zasobnik_vyhodnot.Push('B');
                                                matice_upr_vyhodnot.Push(vysl);
                                            }
                                            else
                                            {
                                                Matrix<T> B = matice_upr_vyhodnot.Pop();
                                                B = ClassicOperations.MultiplyWithNumber(B, A);
                                                zasobnik_vyhodnot.Push('B');
                                                matice_upr_vyhodnot.Push(B);
                                            }
                                        }
                                    }
                                    else // Pokud jsou obě matice
                                    {
                                        if (tmp_1 == tmp_2) // stejného typu
                                        {
                                            if (tmp_1 == 'A')
                                            {
                                                Matrix<T> A = matice_vyhodnot.Pop();
                                                Matrix<T> B = matice_vyhodnot.Pop();
                                                Matrix<T> vysl;
                                                vysl = ClassicOperations.StrassenWinograd(B, A);
                                                zasobnik_vyhodnot.Push('B');
                                                matice_upr_vyhodnot.Push(vysl);
                                            }
                                            else
                                            {
                                                Matrix<T> A = matice_upr_vyhodnot.Pop();
                                                Matrix<T> B = matice_upr_vyhodnot.Pop();
                                                A = ClassicOperations.StrassenWinograd(B, A);
                                                zasobnik_vyhodnot.Push('B');
                                                matice_upr_vyhodnot.Push(A);
                                            }
                                        }
                                        else // různého typu
                                        {
                                            if (tmp_1 == 'A')
                                            {
                                                Matrix<T> A = matice_vyhodnot.Pop();
                                                Matrix<T> B = matice_upr_vyhodnot.Pop();
                                                B = ClassicOperations.StrassenWinograd(B, A);
                                                zasobnik_vyhodnot.Push('B');
                                                matice_upr_vyhodnot.Push(B);
                                            }
                                            else
                                            {
                                                Matrix<T> B = matice_vyhodnot.Pop();
                                                Matrix<T> A = matice_upr_vyhodnot.Pop();
                                                A = ClassicOperations.StrassenWinograd(B, A);
                                                zasobnik_vyhodnot.Push('B');
                                                matice_upr_vyhodnot.Push(A);
                                            }
                                        }
                                    }
                                    break;
                                case '+':
                                    char temp_a = zasobnik_vyhodnot.Pop();
                                    char temp_b = zasobnik_vyhodnot.Pop();
                                    if (temp_a == temp_b) // Obě matice jsou stejného typu
                                    {
                                        if (temp_b == 'A')
                                        {
                                            Matrix<T> A = matice_vyhodnot.Pop();
                                            Matrix<T> B = matice_vyhodnot.Pop();
                                            Matrix<T> vysl;
                                            vysl = ClassicOperations.Addition(A, B);
                                            zasobnik_vyhodnot.Push('B');
                                            matice_upr_vyhodnot.Push(vysl);
                                        }
                                        else
                                        {
                                            Matrix<T> A = matice_upr_vyhodnot.Pop();
                                            Matrix<T> B = matice_upr_vyhodnot.Pop();
                                            A = ClassicOperations.Addition(A, B);
                                            zasobnik_vyhodnot.Push('B');
                                            matice_upr_vyhodnot.Push(A);
                                        }
                                    }
                                    else // každá matice je jiného typu, ale u sčítání je možno vyměnit pořadí
                                    {
                                        Matrix<T> A = matice_vyhodnot.Pop();
                                        Matrix<T> B = matice_upr_vyhodnot.Pop();
                                        B = ClassicOperations.Addition(A, B);
                                        zasobnik_vyhodnot.Push('B');
                                        matice_upr_vyhodnot.Push(B);
                                    }
                                    break;
                                case '-':
                                    char temp_1 = zasobnik_vyhodnot.Pop();
                                    char temp_2 = zasobnik_vyhodnot.Pop();
                                    if (temp_1 == temp_2) // Obě matice jsou stejného typu
                                    {
                                        if (temp_2 == 'A')
                                        {
                                            Matrix<T> A = matice_vyhodnot.Pop();
                                            Matrix<T> B = matice_vyhodnot.Pop();
                                            Matrix<T> vysl;
                                            vysl = ClassicOperations.Subtraction(A, B);
                                            zasobnik_vyhodnot.Push('B');
                                            matice_upr_vyhodnot.Push(vysl);
                                        }
                                        else
                                        {
                                            Matrix<T> A = matice_upr_vyhodnot.Pop();
                                            Matrix<T> B = matice_upr_vyhodnot.Pop();
                                            A = ClassicOperations.Subtraction(A, B);
                                            zasobnik_vyhodnot.Push('B');
                                            matice_upr_vyhodnot.Push(A);
                                        }
                                    }
                                    else // každá matice je jiného typu
                                    {
                                        if (temp_1 == 'A')
                                        {
                                            Matrix<T> A = matice_vyhodnot.Pop();
                                            Matrix<T> B = matice_upr_vyhodnot.Pop();
                                            B = ClassicOperations.Subtraction(A, B);
                                            zasobnik_vyhodnot.Push('B');
                                            matice_upr_vyhodnot.Push(B);
                                        }
                                        else
                                        {
                                            Matrix<T> B = matice_vyhodnot.Pop();
                                            Matrix<T> A = matice_upr_vyhodnot.Pop();
                                            A = ClassicOperations.Subtraction(A, B);
                                            zasobnik_vyhodnot.Push('B');
                                            matice_upr_vyhodnot.Push(A);
                                        }
                                    }
                                    break;
                            }
                            break;
                    }
                }

                char temp = zasobnik_vyhodnot.Pop();
                if (temp == 'A')
                {
                    vysledek = matice_vyhodnot.Pop();
                }
                else
                {
                    if (temp == 'B')
                    {
                        vysledek = matice_upr_vyhodnot.Pop();
                    }
                    else // kdyby náhodou zbylo v zásobníku číslo
                    {
                        T vysl = cislo_vyhodnot.Pop();
                        vysledek = new Matrix<T>(1, 1);
                        vysledek.WriteNumber(0, 0, vysl);
                    }
                }
            }
            else
            {
                int[,] tmp = new int[1, 1];
                tmp[0, 0] = 0;
                vysledek = new Matrix<T>(tmp);
            }

            return vysledek;
        }
    }
}
