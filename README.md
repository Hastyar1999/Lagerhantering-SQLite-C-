Vad projektet gör
Lägga till nya varor i en SQLite‑databas

Visa alla varor i en snygg utskrift

Sortera efter pris eller lagersaldo (stigande/fallande)

Fyll databasen med 20 testvaror (Nike, Adidas, Puma osv.)

All SQL är parameteriserad så inget strul med injection eller konstiga buggar

Menysystem i konsolen som är enkelt att navigera

Varför jag byggde det
Jag ville få bättre koll på:

Hur man kopplar C# till SQLite

Hur man skriver SQL som inte är spaghetti

Hur man jobbar med structs och listor

Hur man bygger ett menybaserat program som faktiskt känns användbart

Hur man hanterar fel (och jag fick många fel längs vägen…)

Det här projektet var perfekt för att öva på allt det där.

Databasen
Tabellen varor innehåller:

id

namn

pris

lager

datum

Inget fancy, men tillräckligt för att kunna sortera, filtrera och testa olika SQL‑kommandon.

Testdata
Jag lade in 20 varor automatiskt så man slipper mata in allt själv.
Perfekt när man vill testa sortering eller bara se att allt funkar.

Tekniker jag använde
C# (.NET)

SQLite

Microsoft.Data.Sqlite

Structs

Listor

Parameteriserade SQL‑kommandon

Konsolprogram

Avslutning
Det här projektet är mest för att visa vad jag lärt mig och för att ha något konkret att bygga vidare på.
Koden är enkel, tydlig och lätt att förstå — och det var också målet.
Om du vill testa, bygga vidare eller bara kika runt: kör hårt.
