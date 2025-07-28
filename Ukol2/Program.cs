/*
 * Vyhleda lode, jejichz pilot je z planety „Kashyyyk“.
 */
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;

var handler = new HttpClientHandler();
handler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, errors) => true;

using HttpClient client = new(handler);
client.BaseAddress = new Uri("https://swapi.dev/api/");

var planetName = "Kashyyyk";
try
{
    Debug.WriteLine("Vyhledavam..");

    var residentsUri = await SearchResidentsOnPlanet(client, planetName);
    Debug.WriteLine($"Planeta '{planetName}' nalezena.");

    var allShips = await GetAllShips(client);
    Debug.WriteLine($"Celkem {allShips.Count} lodi nalezeno.");

    var shipsWithPilots = await GetShipsWithPilots(client, allShips, residentsUri);
    Debug.WriteLine($"Kapitanovych {shipsWithPilots.Count} lodi nalezeno..");

    string shipsWithPilotsJson = JsonSerializer.Serialize(shipsWithPilots, new JsonSerializerOptions { WriteIndented = true });
    Console.Write(shipsWithPilotsJson);
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex);
}

/* Konec */

// Vyhleda jen lode, kde je nekdo z pilotu.
static async Task<List<ShipPilot>> GetShipsWithPilots(HttpClient client, List<Ship> allShips, List<string> pilotsUri)
{
    if (allShips.Count <= 0 || pilotsUri.Count <= 0)
        return new List<ShipPilot>();

    List<Ship> selectedShips = allShips
        .Where(s => s.pilots is not null && s.pilots.Any(p => pilotsUri.Contains(p)))
        .Select(s => new Ship
        (
            name: s.name,
            pilots: s.pilots.Where(p => pilotsUri.Contains(p)).ToList()
        ))
        .ToList();

    // unikatni seznam nalezenych uri pilotu
    List<string> pilotsOnSelected = selectedShips.SelectMany(s => s.pilots).Distinct().ToList();

    // zjistime jmena pilotu    
    Dictionary<string, string> pilotsOnShipsNames = new Dictionary<string, string>();
    foreach (var pilotUri in pilotsUri)
        pilotsOnShipsNames.Add(pilotUri, (await GetPeople(client, pilotUri)).name);

    // vysledek
    return selectedShips.Select(s => new ShipPilot(
            shipName: s.name,
            pilots: s.pilots.Select(p => new Pilot(pilotsOnShipsNames[p], p)).ToList()
        ))
        .ToList();
}

// Vyhleda osobu.
static async Task<People> GetPeople(HttpClient client, string peopleUri)
{
    return await client.GetFromJsonAsync<People>(peopleUri) ?? throw new ArgumentNullException(nameof(peopleUri));
}

// Vyhleda lidi na planete jmenem planetName.
static async Task<List<string>> SearchResidentsOnPlanet(HttpClient client, string planetName)
{
    PlanetResult planets = await client.GetFromJsonAsync<PlanetResult>($"planets?search={planetName}")
        ?? throw new ArgumentNullException(nameof(planets));

    if (planets.count <= 0 || planets.results is null || planets.results.Count <= 0)
        throw new Exception($"Planet '{planetName}' not found");

    else if (planets.count > 1)
        throw new Exception($"Multiple planets '{planetName}' found.");

    return planets.results[0].residents;
}

// Vyhleda lidi na planete cislo.
static async Task<List<string>> GetResidentsOnPlanet(HttpClient client, int planetIndex)
{
    Planet planet = await client.GetFromJsonAsync<Planet>($"planets/{planetIndex}/")
        ?? throw new ArgumentNullException(nameof(planet));

    return planet.residents;
}

// Vyhleda vsechny vesmirne lode.
static async Task<List<Ship>> GetAllShips(HttpClient client)
{
    List<Ship> allShips = new List<Ship>();

    string getUrl = "starships";
    while (getUrl is not null)
    {
        ShipResult ships = await client.GetFromJsonAsync<ShipResult>(getUrl)
            ?? throw new ArgumentNullException(nameof(ships));

        if (ships.count > 0 && ships.results is not null && ships.results.Count > 0)
            allShips.AddRange(ships.results);

        getUrl = ships.next;
    }

    return allShips;
}

record ShipResult(int count, string next, List<Ship> results);
record Ship(string name, List<string> pilots);
record ShipPilot(string shipName, List<Pilot> pilots);
record Pilot(string pilotName, string pilotUri);
record PlanetResult(int count, string next, List<Planet> results);
record Planet(string name, List<string> residents);
record People(string homeworld, string name, List<string> starships);
