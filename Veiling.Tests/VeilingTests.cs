namespace Veiling.Tests;

public class VeilingTests
{
    [Fact]
    public void Bied_EersteBod_WordtAanvaard()
    {
        var veiling = new Veiling();

        bool aanvaard = veiling.Bied("Ann", 100m);

        Assert.True(aanvaard);
        Assert.Equal(100m, veiling.HoogsteBod);
        Assert.Equal("Ann", veiling.HoogsteBieder);
    }

    [Fact]
    public void Bied_HogerBod_VervangtHetHoogste()
    {
        var veiling = new Veiling();
        veiling.Bied("Ann", 100m);

        bool aanvaard = veiling.Bied("Bob", 150m);

        Assert.True(aanvaard);
        Assert.Equal(150m, veiling.HoogsteBod);
        Assert.Equal("Bob", veiling.HoogsteBieder);
    }

    [Theory]
    [InlineData(90)]  // lager
    [InlineData(100)] // gelijk, niet strikt hoger
    public void Bied_NietHogerBod_WordtGeweigerd(int bedrag)
    {
        var veiling = new Veiling();
        veiling.Bied("Ann", 100m);

        bool aanvaard = veiling.Bied("Bob", bedrag);

        Assert.False(aanvaard);
        // Het hoogste bod blijft ongewijzigd.
        Assert.Equal(100m, veiling.HoogsteBod);
        Assert.Equal("Ann", veiling.HoogsteBieder);
    }

    [Fact]
    public void Bied_AanvaardBod_VuurtEventAf()
    {
        var veiling = new Veiling();
        BodEventArgs? ontvangen = null;
        veiling.BodUitgebracht += (afzender, e) => ontvangen = e;

        veiling.Bied("Ann", 100m);

        Assert.NotNull(ontvangen);
        Assert.Equal("Ann", ontvangen!.Bieder);
        Assert.Equal(100m, ontvangen.Bedrag);
    }

    [Fact]
    public void Bied_GeweigerdBod_VuurtGeenEventAf()
    {
        var veiling = new Veiling();
        veiling.Bied("Ann", 100m);
        int aantalKeerAfgevuurd = 0;
        veiling.BodUitgebracht += (afzender, e) => aantalKeerAfgevuurd++;

        veiling.Bied("Bob", 50m); // te laag

        Assert.Equal(0, aantalKeerAfgevuurd);
    }

    [Fact]
    public void Bied_MeerdereAbonnees_WordenAllemaalVerwittigd()
    {
        var veiling = new Veiling();
        int abonneeA = 0;
        int abonneeB = 0;
        veiling.BodUitgebracht += (afzender, e) => abonneeA++;
        veiling.BodUitgebracht += (afzender, e) => abonneeB++;

        veiling.Bied("Ann", 100m);

        Assert.Equal(1, abonneeA);
        Assert.Equal(1, abonneeB);
    }

    [Fact]
    public void Bied_AfgemeldeAbonnee_WordtNietMeerVerwittigd()
    {
        var veiling = new Veiling();
        int teller = 0;
        void Abonnee(object? afzender, BodEventArgs e) => teller++;

        veiling.BodUitgebracht += Abonnee;
        veiling.Bied("Ann", 100m); // teller wordt 1

        veiling.BodUitgebracht -= Abonnee;
        veiling.Bied("Bob", 200m); // niet meer geteld

        Assert.Equal(1, teller);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Bied_LegeBieder_GooitException(string? bieder)
    {
        var veiling = new Veiling();

        Assert.Throws<ArgumentException>(() => veiling.Bied(bieder!, 100m));
    }

    [Fact]
    public void Bied_BiederMetSpaties_WordtGetrimd()
    {
        var veiling = new Veiling();

        veiling.Bied("  Ann  ", 100m);

        Assert.Equal("Ann", veiling.HoogsteBieder);
    }
}
