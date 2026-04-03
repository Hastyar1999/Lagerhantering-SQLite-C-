using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Butik
{
    public struct Shop                     // skriver ett OOP, detta handlar om att hantera en butik. 
                                           // koden skrivs i class
    {
        public int ArtikelNummer;
        public string Vara;
        public double Pris;
        public int lager;
        public string Datum;

        public void PrintOut() // vi behöver dett metod för att Skriva ut resultatet. 
        {
            Console.WriteLine($"ArtikelNummer:{ArtikelNummer}, Vara: {Vara}, Pris: {Pris}, Lagersald: {lager}, Datum: {Datum}");
        }
    }


    internal class Program
    {  // Koppla Databasen 
        static string connectionString = "Data Source=Butik.db";
        // Här körs programmet
        static void Main(string[] args)
        {
            skapaDatabas();

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            bool avsluta = false;
            while (!avsluta)
            {
                Console.Clear();
                Console.WriteLine("=== BUTIKSDATABAS (SQL) ===");
                Console.WriteLine("A) Visa alla varor");
                Console.WriteLine("B) Sortera varor");
                Console.WriteLine("C) Lägg till en vara");
                Console.WriteLine("D) Ta bort en vara");
                Console.WriteLine("E) Redigera pris/lager");
                Console.WriteLine("F) Sök efter vara");
                Console.WriteLine("G) Visa varor som behöver fyllas på");
                Console.WriteLine("H) Skapa test varor");
                Console.WriteLine("X) Avsluta");
                Console.WriteLine("---------------------------");
                Console.Write("Val: ");

                string val = Console.ReadLine().Trim().ToUpper();

                switch (val)
                {
                    case "A":
                        visaData();
                        break;
                    case "B":
                        SorteraEfterPris();
                        break;
                    case "C":
                        läggTillVara();
                        break;
                    case "D":
                        tarbortVara();
                        break;
                    case "E":
                        RedigeraData();
                        break;
                    case "F":
                        Sökväg();
                        break;
                    case "G":
                        rekomend();
                        break;
                    case "H":
                        läggInVärde();
                        break;
                    case "X":
                        avsluta = true;
                        Console.WriteLine("Avslutar...");
                        break;
                    default:
                        Console.WriteLine("Felaktigt val.");
                        break;
                }

                if (!avsluta)
                {
                    Console.WriteLine("\nTryck valfri tangent för att fortsätta...");
                    Console.ReadKey();
                }
            }
        }
        // Här slutar Main()       
        static void skapaDatabas()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"CREATE TABLE IF NOT EXISTS varor(id INTEGER PRIMARY KEY AUTOINCREMENT,
                        namn TEXT NOT NULL,
                        pris REAL,
                        lager INTEGER,
                        datum TEXT
                    )";
                cmd.ExecuteNonQuery();
            }
        }


        static void läggTillVara()
        {
            Console.WriteLine("===lägg till vara===");
            Console.WriteLine("");
            Console.Write("Namn"); string namn = Console.ReadLine();
            Console.Write("Pris: "); double pris;
            while (!double.TryParse(Console.ReadLine(), out pris))
            {
                Console.WriteLine("Du måste ange pris");
            }
            Console.Write("Lager: "); int lager; while (!int.TryParse(Console.ReadLine(), out lager))
            {
                Console.WriteLine("Du måste ange lagersaldot");
            }
            string datum = DateTime.Now.ToString("yyyy-MM-dd");


            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"INSERT INTO Varor (namn, pris, lager, datum) VALUES (@namn, @pris, @lager, @datum)";
                cmd.Parameters.AddWithValue("@namn", namn);
                cmd.Parameters.AddWithValue("@pris", pris);
                cmd.Parameters.AddWithValue("@lager", lager);
                cmd.Parameters.AddWithValue("@datum", datum);
                cmd.ExecuteNonQuery();
            }
            Console.WriteLine("varan har sparat nu!");
        }
        static void Sökväg()
        {
            int id = -1;       // Ge ett standardvärde
            string sök = "";   // Ge ett standardvärde
            bool körSökning = false;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Sök ===");
                Console.WriteLine("A) Sök på Artikelnummer");
                Console.WriteLine("B) Sök på Varans namn");
                Console.WriteLine("X) Avbryt");
                Console.Write("Val: ");

                string val = Console.ReadLine().Trim().ToUpper();

                if (val == "X") return; // Gå direkt ur metoden

                if (val == "A")
                {
                    Console.Write("Ange ID: ");
                    if (int.TryParse(Console.ReadLine(), out id))
                    {
                        // Vi nollställer namn-sökningen så vi bara söker på ID
                        sök = "";
                        körSökning = true;
                        break; // Bryt loopen och gå till SQL-delen
                    }
                }
                else if (val == "B")
                {
                    Console.Write("Ange sökord: ");
                    sök = Console.ReadLine();
                    id = -1; // Nollställ ID-sökningen
                    körSökning = true;
                    break; // Bryt loopen
                }
                else
                {
                    Console.WriteLine("Ogiltigt val, försök igen.");
                    Console.ReadKey();
                }
            }

            if (körSökning)
            {
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();

                    // Vi använder en smart SQL-fråga som kollar antingen ID eller Namn
                    cmd.CommandText = "SELECT * FROM varor WHERE id = @id OR namn LIKE @sök";
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@sök", "%" + sök + "%");

                    using (var reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("\nSökresultat:");
                        bool hittad = false;
                        while (reader.Read())
                        {
                            hittad = true;
                            // Skriv ut direkt från reader
                            Console.WriteLine($"ID: {reader.GetInt32(0)} | Namn: {reader.GetString(1)} | Pris: {reader.GetDouble(2)}");
                        }

                        if (!hittad) Console.WriteLine("Ingen vara matchade din sökning.");
                    }
                }
                Console.WriteLine("\nTryck på valfri tangent...");
                Console.ReadKey();
            }
        }
        //Denna metod gör så användaren kan redigera befintlig data genom att ange artikelnummer, sen kan ange ny pris och lagersaldo.
        // returnerar inget och List butik som parameter
        static void RedigeraData()
        {
            Console.WriteLine("Ange varans ID");
            int id;
            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Du måste ange ID");
            }

            Console.Write("Ange nytt pris");
            double pris;
            while (!double.TryParse(Console.ReadLine(), out pris))
            {
                Console.WriteLine("Du måste ange ID");
            }
            Console.Write("Ange nytt lagersaldo");
            int lager;
            while (!int.TryParse(Console.ReadLine(), out lager))
            {
                Console.WriteLine("Du måste ange lagersaldo");
            }


            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"UPDATE varor SET pris=@pris, lager=@lager WHERE id=@id";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@pris", pris);
                cmd.Parameters.AddWithValue("@lager", lager);
                cmd.ExecuteNonQuery();
            }
            Console.WriteLine("Ändringen lyckades");

        }

        //Metod Tittar om Lagersaldo är under 5 om den är så skrivs det ut att varans lagersaldo behövs fylla på.
        //Returnerar inget, har list butik som parameter.
        static void rekomend()
        {

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"SELECT id,namn,lager FROM varor WHERE lager <= 5";
                using (var reader = cmd.ExecuteReader())
                {
                    bool hitta = false;
                    while (reader.Read())
                    {
                        Console.WriteLine($"Id: {reader.GetInt32(0)}| Namn: {reader.GetString(1)} | Lagersaldo: {reader.GetInt32(2)}");
                        Console.WriteLine("Dessa varor måste fylls på omedelbart!");
                        hitta = true;
                    }
                    if (!hitta)
                    {
                        Console.WriteLine("Ingen påfyllning behövs!");

                    }

                }

            }
        }


        //Metod Sorterar varor efter pris eller lagersaldo, även stigandes eller fallande beroende på input av användare.
        static void SorteraEfterPris()
        {
            bool loop = false;

            while (!loop)
            {
                Console.WriteLine("Sortera efter:");
                Console.WriteLine("1 = Pris");
                Console.WriteLine("2 = Lagersaldo");
                Console.WriteLine("0 = Avsluta");
                Console.Write("Val: ");
                if (!int.TryParse(Console.ReadLine(), out int val))
                {
                    Console.WriteLine("Du måste välja rätt!");
                    continue;
                }

                if (val == 0)
                {
                    Console.WriteLine("Avslutar...");
                    return;
                }

                Console.WriteLine("Sortera:");
                Console.WriteLine("1 = Stigande");
                Console.WriteLine("2 = Fallande");
                Console.Write("Val: ");

                if (!int.TryParse(Console.ReadLine(), out int valS))
                {
                    Console.WriteLine("Du måste välja rätt!");
                    continue;
                }

                string orderby = val == 1 ? "pris" : "lager";   // använder jämförelse i SQL istället för att skriva vilkor här på C# 
                string direction = valS == 1 ? "ASC" : "DESC";       // på så sätt håller jag koden kort fattad. 
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = $"SELECT id,namn,pris,lager,datum FROM varor ORDER BY {orderby} {direction}";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"Id: {reader.GetInt32(0)} | Namn: {reader.GetString(1)} | Pris: {reader.GetDouble(2)} | Lagersaldo: {reader.GetInt32(3)} | Datum: {reader.GetString(4)}");
                        }
                    }
                }

            }
        }

        //Metoden tar bort en rad beroende på artikelnummer angivet av användaren.
        static void tarbortVara()
        {
            Console.WriteLine("Ange varans ID");
            int id;
            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Du måste ge rätt datatyp.Bara sifror tack!");
            }
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"DELETE FROM varor WHERE id=@id";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            Console.WriteLine("Varan är borttagen.");
        }


        //Metod skriver ut alla världen i fil genom att anropa metod VisaVarden
        //Metod returnerar inget, List som parameter
        static void visaData()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"SELECT id, namn, pris, lager, datum FROM varor";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"Id: {reader.GetInt32(0)}, Namn: {reader.GetString(1)}, Pris: {reader.GetDouble(2)}, Lagersaldo: {reader.GetInt32(3)}, Datum: {reader.GetString(4)}");
                    }
                }




            }
        }
        static void läggInVärde()  // bara för test
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();

                List<Shop> shop = new List<Shop>
                  {
                 new Shop { Vara = "Sneakers Nike Air", Pris = 1299, lager = 15, Datum = "2026-03-01" },
                 new Shop { Vara = "Adidas Ultraboost", Pris = 1599, lager = 20, Datum = "2026-03-02" },
                new Shop { Vara = "Puma RS-X", Pris = 1199, lager = 18, Datum = "2026-03-03" },
                new Shop { Vara = "Timberland Känga", Pris = 2100, lager = 8, Datum = "2026-03-04" },
                new Shop { Vara = "Birkenstock Sandaler", Pris = 950, lager = 30, Datum = "2026-03-05" },
                  new Shop { Vara = "Converse Chuck Taylor", Pris = 799, lager = 25, Datum = "2026-03-06" },
                  new Shop { Vara = "Vans Old Skool", Pris = 899, lager = 22, Datum = "2026-03-07" },
                 new Shop { Vara = "New Balance 574", Pris = 1100, lager = 17, Datum = "2026-03-08" },
                 new Shop { Vara = "Reebok Classic", Pris = 999, lager = 14, Datum =  "2026-03-09" },
                  new Shop { Vara = "Crocs Classic", Pris = 499, lager = 40, Datum = "2026-03-10" },
                new Shop { Vara = "Chelsea Boot", Pris = 1750, lager = 9, Datum = "2026-03-11" },
                new Shop { Vara = "Läderstövel North", Pris = 1999, lager = 12, Datum = "2026-03-12" },
                   new Shop { Vara = "Gympasko Puma", Pris = 1200, lager = 18, Datum = "2026-03-13" },
                new Shop { Vara = "Flipflops Basic", Pris = 199, lager = 50, Datum = "2026-03-14" },
                 new Shop { Vara = "Tofflor Cozy", Pris = 299, lager = 35, Datum = "2026-03-15" },
                  new Shop { Vara = "Löparsko Asics", Pris = 1450, lager = 21, Datum = "2026-03-16" },
                new Shop { Vara = "Festsko Läder", Pris = 1800, lager = 5, Datum = "2026-03-17" },
                  new Shop { Vara = "Hiking Boot Gore-Tex", Pris = 2300, lager = 7, Datum = "2026-03-18" },
                 new Shop { Vara = "Slippers SoftCloud", Pris = 249, lager = 28, Datum = "2026-03-19" },
                     new Shop { Vara = "Nike Air Max 90", Pris = 1499, lager = 16, Datum = "2026-03-20" } };


                foreach (var v in shop)
                {
                    cmd.Parameters.Clear();  // RENSAR GAMLA PARAMETRAR Detta är väldigt viktigt! Glöm aldrig!
                    cmd.CommandText = $@"INSERT INTO varor(namn,pris,lager,datum) VALUES (@namn,@pris,@lager,@datum)";
                    cmd.Parameters.AddWithValue("@namn", v.Vara);
                    cmd.Parameters.AddWithValue("@pris", v.Pris);
                    cmd.Parameters.AddWithValue("@lager", v.lager);
                    cmd.Parameters.AddWithValue("@datum", v.Datum);
                    cmd.ExecuteNonQuery();
                }
                Console.WriteLine("testdatan sparades");
            }

        }
    }
}

       

        
