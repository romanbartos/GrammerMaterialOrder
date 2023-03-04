using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace GrammerMaterialOrder.Utilities
{
    public class Helper
    {
        public static string path = @"C:\Users\pc\source\repos\GrammerMaterialOrder\bin\Release\net6.0-windows\Import\";
        public struct Zakazka
        {
            public string VyrobniZakazka { get; set; }
            public int PocetKusu { get; set; }
            public string CisloDisponenta { get; set; }
            public string CisloSedacky { get; set; }
            public string Vyrobek { get; set; }

            public Zakazka(string vyrobniZakazka, int pocetKusu, string cisloDisponenta, string cisloSedacky, string vyrobek)
            {
                VyrobniZakazka = vyrobniZakazka;
                PocetKusu = pocetKusu;
                CisloDisponenta = cisloDisponenta;
                CisloSedacky = cisloSedacky;
                Vyrobek = vyrobek;
            }
        }

        public static List<Zakazka> zakazky;

        // konverze souboru CESKA_ORDER_HEADER, obsahuje za každým znakem binární 0
        // vytvoří se soubor WriteText.txt
        public class ConvertFile
        {
            public static bool ConvertFileWithoutZero()
            {
                string str;

                try
                {
                    //string readText = File.ReadAllText(path);
                    // Open the text file using a stream reader.

                    using StreamReader sr = new(path + @"CESKA_ORDER_HEADER.txt", System.Text.Encoding.UTF8);
                    int j = 0;
                    str = sr.ReadToEnd();
                    char[] newStr = new char[str.Length / 2];
                    for (int i = 0; i < str.Length; i++)
                    {
                        if (i % 2 == 0)
                        {
                            newStr[j++] = str[i];
                        }
                    }
                    string charsStr = new(newStr);
                    //ExampleAsync(charsStr);

                    using FileStream fs = File.Create(path + @"WriteText.txt");
                    using StreamWriter soubor = new(fs);
                    soubor.Write(charsStr);
                    return true;
                }
                catch (IOException ex)
                {
                    _ = MessageBox.Show(@"Při čtení souboru došlo k problému:" + (char)10 + ex.Message, @"Upozornění", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

            }

            public static bool GetValueZakazka()
            {
                string vyrobniZakazka;
                int pocetKusu;
                string cisloDisponenta;
                string cisloSedacky;

                string[] value2;
                string[] value3;
                string[] value4;

                string str;
                zakazky = new();
                try
                {
                    using StreamReader sr = new(path + @"WriteText.txt", System.Text.Encoding.UTF8);
                    str = sr.ReadToEnd();
                    char[] buffer = new char[147];
                    int delkaSouboru = str.Length;
                    int pocetZakazek = delkaSouboru / 147;
                    string vyrobek;
                    for (int i = 0; i < pocetZakazek; i++)
                    {
                        buffer = new char[147];
                        for (int j = 0; j < 147; j++)
                        {
                            buffer[j] = str[j + (i * 147)];
                        }

                        string charsStr = new(buffer);
                        value2 = Regex.Split(charsStr, @"\s{2,}");
                        vyrobniZakazka = value2[0].Substring(0, 12);
                        //od pozice 16 3 znaky
                        cisloDisponenta = value2[0].Substring(16, 3);

                        value3 = value2[1].Split(' ');
                        cisloSedacky = value3[2];
                        vyrobek = value3[2];
                        value4 = value3[0].Split(',');

                        pocetKusu = Convert.ToInt32(value4[0]);

                        zakazky.Add(new Zakazka(vyrobniZakazka, pocetKusu, cisloDisponenta, cisloSedacky, vyrobek));
                    }

                    return true;
                }
                catch (IOException ex)
                {
                    _ = MessageBox.Show(@"Při čtení souboru došlo k problému:" + (char)10 + ex.Message, @"Upozornění", MessageBoxButton.OK, MessageBoxImage.Warning);
                    //_ = MessageBox.Show("The file could not be read:");
                    //_ = MessageBox.Show(ex.Message);

                    return false;
                }
            }
        }
    }
}
