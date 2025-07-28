using Ukol3.BL;

namespace Ukol3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Posadka posadkaEnterprise = Posadka.Enterprice();

            Console.WriteLine("===");
            SeznamPosadky(posadkaEnterprise);

            Console.WriteLine("===");
            {
                Random random = new Random();
                for (int i = 0; i < 5; i++)
                    SeznamPodrizenych(posadkaEnterprise, i + 1, posadkaEnterprise[random.Next(posadkaEnterprise.Count)]);
            }

            Console.WriteLine("===");
            {
                Random random = new Random();
                for (int i = 0; i < 5; i++)
                    SeznamNakazy(posadkaEnterprise, i + 1, posadkaEnterprise[random.Next(posadkaEnterprise.Count)]);

            }

            // Konec
            Console.WriteLine("Hotovo!");
        }

        static void SeznamPosadky(Posadka posadkaEnterprise)
        {
            Console.WriteLine("Členové posádky lodi Enterprise (abecedně):");

            foreach (var clen in posadkaEnterprise.OrderBy(c => c.FullName))
                Console.WriteLine(clen.FullName);
        }

        static void SeznamPodrizenych(Posadka posadkaEnterprise, int poradi, ClenPosadky clen)
        {
            var podrizeny = posadkaEnterprise.SeznamPodrizenych(clen.FirstName, clen.LastName).ToList();
            if (podrizeny.Count == 0)
            {
                Console.WriteLine();
                Console.WriteLine($"{poradi}. {clen.FullName} nemá podřízené.");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine($"{poradi}. {clen.FullName} má podřízené:");

                podrizeny.ForEach(p =>
                {
                    Console.Write(" <- ");
                    Console.WriteLine(p.FullName);
                });
            }
        }

        static void SeznamNakazy(Posadka posadkaEnterprise, int poradi, ClenPosadky clen)
        {
            Console.WriteLine();
            Console.WriteLine($"{poradi}. {clen.FullName} má nákazu, nakzail:");

            // Kapitan se neresi
            if (clen.HasName("Jean Luc", "Pickard"))
                return;

            HashSet<ClenPosadky> jizNakazen = new HashSet<ClenPosadky>() { clen };

            // vsechny sve podrizene
            foreach (var podrizeny in posadkaEnterprise.SeznamPodrizenych(clen.FirstName, clen.LastName))
            {
                if (!jizNakazen.Contains(podrizeny))
                {
                    jizNakazen.Add(podrizeny);
                    Console.Write(" <- ");
                    Console.WriteLine(podrizeny.FullName);
                }
            }

            // vsechny sve nadrizene
            foreach (var nadrizeny in posadkaEnterprise.SeznamNadrizenych(clen.FirstName, clen.LastName))
            {
                // nenakazil kapitana
                if (nadrizeny.HasName("Jean Luc", "Pickard"))
                    continue;

                jizNakazen.Add(nadrizeny);
                Console.Write(" -> ");
                Console.WriteLine(nadrizeny.FullName);

                foreach (var podrizeny in posadkaEnterprise.SeznamPodrizenych(nadrizeny.FirstName, nadrizeny.LastName))
                {
                    if (!jizNakazen.Contains(podrizeny))
                    {
                        jizNakazen.Add(podrizeny);
                        Console.Write(" <- ");
                        Console.WriteLine(podrizeny.FullName);
                    }
                }
            }
        }
    }
}
