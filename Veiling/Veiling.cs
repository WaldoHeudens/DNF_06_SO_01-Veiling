namespace Veiling;

/// <summary>
/// Gegevens die bij een aanvaard bod horen. We geven ze mee met het event.
/// </summary>
public class BodEventArgs(string bieder, decimal bedrag) : EventArgs
{
    public string Bieder => bieder;
    public decimal Bedrag => bedrag;
}

/// <summary>
/// Een eenvoudige veiling. Ze houdt het hoogste bod bij en verwittigt
/// abonnees via een event zodra er een geldig hoger bod binnenkomt.
/// De logica staat los van Console, zodat we ze kunnen testen.
/// </summary>
public class Veiling
{
    public decimal HoogsteBod { get; private set; }

    public string? HoogsteBieder { get; private set; }

    /// <summary>
    /// Vuurt af telkens er een geldig, hoger bod wordt aanvaard.
    /// </summary>
    public event EventHandler<BodEventArgs>? BodUitgebracht;

    /// <summary>
    /// Brengt een bod uit. Het bod wordt enkel aanvaard als het strikt hoger
    /// is dan het huidige hoogste bod. Bij aanvaarding vuurt het event af.
    /// </summary>
    /// <returns>true als het bod aanvaard werd, anders false.</returns>
    public bool Bied(string bieder, decimal bedrag)
    {
        if (string.IsNullOrWhiteSpace(bieder))
        {
            throw new ArgumentException("Bieder mag niet leeg zijn.", nameof(bieder));
        }

        if (bedrag <= HoogsteBod)
        {
            return false;
        }

        HoogsteBod = bedrag;
        HoogsteBieder = bieder.Trim();

        // Afvuren. ?.Invoke doet niets als er geen abonnees zijn.
        BodUitgebracht?.Invoke(this, new BodEventArgs(HoogsteBieder, bedrag));
        return true;
    }
}
