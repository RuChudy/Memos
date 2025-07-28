using System.Diagnostics;

namespace Ukol3.BL;

/// <summary>
/// Člen posádky.
/// </summary>
[DebuggerDisplay("FullName = {FullName}")]
internal class ClenPosadky
{
    /// <summary>
    /// Příjmení.
    /// </summary>
    public string LastName { get; private set; }

    /// <summary>
    /// Jméno křestní.
    /// </summary>
    public string? FirstName { get; private set; }

    /// <summary>
    /// Je to muž ?
    /// </summary>
    public bool IsMen { get; private set; }

    /// <summary>
    /// Nadřízený.
    /// </summary>
    public ClenPosadky? Manager { get; set; }

    /// <summary>
    /// Celé jméno.
    /// </summary>
    public string FullName => string.IsNullOrWhiteSpace(FirstName) ? LastName : $"{FirstName} {LastName}";

    /// <summary>
    /// Konstruktor.
    /// </summary>
    /// <param name="firstName">Křestní jméno nebo null, pokud nemá křestní.</param>
    /// <param name="lastName">Příjmení</param>
    /// <param name="isMen">Je to muž ?</param>
    /// <param name="manager">Nadřízený</param>
    public ClenPosadky(string? firstName, string lastName, bool isMen, ClenPosadky? manager = null)
    {
        LastName = lastName;
        FirstName = firstName;
        IsMen = isMen;
        Manager = manager;
    }

    /// <summary>
    /// Zjistí zda celé jméno odpovídá.
    /// </summary>
    /// <param name="fullName">Jméno a příjmení.</param>
    /// <returns>True pokud celé jméno odpovídá.</returns>
    public bool HasFirstAndLastName(string? fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return false;

        return string.Equals(FullName, fullName);
    }

    /// <summary>
    /// Zjisti zda jméno a příjmení odpovídá.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>True pokud jméno a příjmení odpovídá.</returns>
    public bool HasName(string? firstName, string lastName)
    {
        return string.Equals(FirstName, firstName) && string.Equals(LastName, lastName);
    }

    /// <summary>
    /// Kód hash pro aktuální ho člena posádky.
    /// </summary>
    /// <returns>Kód hash.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(FirstName, LastName, IsMen);
    }
}

/// <summary>
/// Posádka.
/// </summary>
internal class Posadka : List<ClenPosadky>
{
    /// <summary>
    /// Vytvoří posádku lodi Enterprice.
    /// </summary>
    /// <returns>Seznam posádky.</returns>
    public static Posadka Enterprice()
    {
        // Seznam všech členů posádky
        Posadka psdkaEnterprice = new Posadka()
        {
            new ClenPosadky("Jean Luc", "Pickard", true),

            new ClenPosadky("William", "Riker", true),
            new ClenPosadky("Dea na", "Troi", false),
            new ClenPosadky("Jordi La", "Forge", true),

            new ClenPosadky("Worf son of", "Mog", true),
            new ClenPosadky(null, "Guinan", false),
            new ClenPosadky("Beverly", "Crusher", false),
            new ClenPosadky("Lwaxana", "Troi", false),
            new ClenPosadky("Reginald", "Barkley", true),
            new ClenPosadky("Mr.", "Data", true),
            new ClenPosadky("Miles", "O´Brien", true),

            new ClenPosadky("Tasha", "Yar", false),
            new ClenPosadky("K´", "Ehleyr", false),
            new ClenPosadky("Weslley", "Crusher", true),
            new ClenPosadky("Allysa", "Ogawa", false),

            new ClenPosadky("Alexander", "Rozhalenko", true),
            new ClenPosadky("Julian", "Bashir", true)
        };

        // Podřízení první úrovně
        {
            var pickard = psdkaEnterprice.NajdiClena("Jean Luc", "Pickard");
            psdkaEnterprice.NajdiClena("William", "Riker").Manager = pickard;
            psdkaEnterprice.NajdiClena("Dea na", "Troi").Manager = pickard;
            psdkaEnterprice.NajdiClena("Jordi La", "Forge").Manager = pickard;
        }

        // Podřízení druhé úrovně
        {
            var riker = psdkaEnterprice.NajdiClena("William", "Riker");
            psdkaEnterprice.NajdiClena("Worf son of", "Mog").Manager = riker;
            psdkaEnterprice.NajdiClena(null, "Guinan").Manager = riker;
            psdkaEnterprice.NajdiClena("Beverly", "Crusher").Manager = riker;
        }

        {
            var troi = psdkaEnterprice.NajdiClena("Dea na", "Troi");
            psdkaEnterprice.NajdiClena("Lwaxana", "Troi").Manager = troi;
            psdkaEnterprice.NajdiClena("Reginald", "Barkley").Manager = troi;
        }

        {
            var forge = psdkaEnterprice.NajdiClena("Jordi La", "Forge");
            psdkaEnterprice.NajdiClena("Mr.", "Data").Manager = forge;
            psdkaEnterprice.NajdiClena("Miles", "O´Brien").Manager = forge;
        }

        // Podřízení třetí úrovně
        {
            var mog = psdkaEnterprice.NajdiClena("Worf son of", "Mog");
            psdkaEnterprice.NajdiClena("Tasha", "Yar").Manager = mog;
            psdkaEnterprice.NajdiClena("K´", "Ehleyr").Manager = mog;
        }

        {
            var crusher = psdkaEnterprice.NajdiClena("Beverly", "Crusher");
            psdkaEnterprice.NajdiClena("Weslley", "Crusher").Manager = crusher;
            psdkaEnterprice.NajdiClena("Allysa", "Ogawa").Manager = crusher;
        }

        // Podřízení čtvrté úrovně
        psdkaEnterprice.NajdiClena("Alexander", "Rozhalenko").Manager = psdkaEnterprice.NajdiClena("K´", "Ehleyr");
        psdkaEnterprice.NajdiClena("Julian", "Bashir").Manager = psdkaEnterprice.NajdiClena("Allysa", "Ogawa");

        return psdkaEnterprice;
    }

    /// <summary>
    /// Vyhledá jednoznačného člena posádky dle jeho jména.
    /// </summary>
    /// <param name="memberFirstName">Jméno člena nebo null, pokud nemá jméno.</param>
    /// <param name="memberLastName">Příjmení člena.</param>
    /// <returns>Člen posádky.</returns>
    public ClenPosadky NajdiClena(string? memberFirstName, string memberLastName) => this.Single(m => m.HasName(memberFirstName, memberLastName));

    /// <summary>
    /// Zjistí seznam nadřízených.
    /// </summary>
    /// <param name="memberFirstName">Jméno člena nebo null, pokud nemá jméno.</param>
    /// <param name="memberLastName">Příjmení člena.</param>
    /// <returns>Seznam nadřízených.</returns>
    public IEnumerable<ClenPosadky> SeznamNadrizenych(string? memberFirstName, string memberLastName)
    {
        ClenPosadky clen = NajdiClena(memberFirstName, memberLastName);
        while (clen.Manager != null)
        {
            yield return clen.Manager;
            clen = clen.Manager;
        }
    }

    /// <summary>
    /// Zjistí seznam všech podřízených.
    /// </summary>
    /// <param name="memberFirstName">Jméno člena nebo null, pokud nemá jméno.</param>
    /// <param name="memberLastName">Příjmení člena.</param>
    /// <returns>Seznam podřízených.</returns>
    public IEnumerable<ClenPosadky> SeznamPodrizenych(string? memberFirstName, string memberLastName)
    {
        foreach (var clen in this)
        {
            if (clen.Manager != null && string.Equals(clen.Manager.FirstName, memberFirstName) && string.Equals(clen.Manager.LastName, memberLastName))
            {
                foreach(var podrizeny in SeznamPodrizenych(clen.FirstName, clen.LastName))
                    yield return podrizeny;

                yield return clen;
            }
        }
    }
}
