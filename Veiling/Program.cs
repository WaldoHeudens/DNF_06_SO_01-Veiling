using Veiling;

// Kleine demo van de Veiling-klasse. De eigenlijke logica staat in Veiling.cs,
// los van Console, zodat we ze kunnen testen. Hier tonen we enkel het gebruik:
// we abonneren ons op het event en brengen een paar biedingen uit.

var veiling = new Veiling.Veiling();

// Abonneren op het event met een lambda. Deze code loopt telkens een bod
// aanvaard wordt.
veiling.BodUitgebracht += (afzender, e) =>
    Console.WriteLine($"Nieuw hoogste bod: {e.Bedrag} euro door {e.Bieder}");

// Een tweede abonnee (een gewone method). Een event mag meerdere abonnees hebben.
veiling.BodUitgebracht += LogNaarConsole;

// Een reeks biedingen. Een bod telt enkel als het strikt hoger is dan het
// huidige hoogste bod; anders wordt het geweigerd en vuurt het event niet af.
Bied("Ann", 100m);
Bied("Bob", 90m);  // te laag, wordt geweigerd
Bied("Bob", 120m);
Bied("Ann", 120m); // niet strikt hoger, wordt geweigerd

Console.WriteLine($"Verkocht aan {veiling.HoogsteBieder} voor {veiling.HoogsteBod} euro");

void Bied(string bieder, decimal bedrag)
{
    bool aanvaard = veiling.Bied(bieder, bedrag);
    if (!aanvaard)
    {
        Console.WriteLine($"Bod van {bieder} ({bedrag} euro) geweigerd: niet hoog genoeg.");
    }
}

static void LogNaarConsole(object? afzender, BodEventArgs e) =>
    Console.WriteLine($"[log] bod aanvaard: {e.Bieder} => {e.Bedrag} euro");
