const int celkemPolozek = 1_000_000;

int[] pole = new int[celkemPolozek];

// Naplnime pole nahodnou hodnotou
Random r = new Random();
for (int i = 0; i < pole.Length; i++)
    pole[i] = r.Next();

// Vyhledame duplicity
Dictionary<int, int> dupl = new Dictionary<int, int>();
foreach (int hodnota in pole)
{
    if (dupl.ContainsKey(hodnota))
    {
        // Duplicitni hodnota, zvysime vyskyt
        dupl[hodnota]++;
    }
    else
    {
        // Nalezena nova hodnota
        dupl.Add(hodnota, 1);
    }
}

// Vypiseme duplicity
foreach (var hodnotaVyskyt in dupl.Where(x => x.Value > 1).OrderBy(x => x.Key))
    Console.WriteLine($"Cislo {hodnotaVyskyt.Key} nalezeno {hodnotaVyskyt.Value} krat");

// Konec
Console.WriteLine("Hotovo!");
