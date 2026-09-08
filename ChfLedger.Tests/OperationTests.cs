using ChfLedger.Domain;

namespace ChfLedger.Tests;

public class OperationsTests
{
    [Fact]
    public void Si_UnSeul_Mouvement_LeveUneException()
    {
        var unSeulMouvement = new List<Mouvement>{new Mouvement(1,10m)};
        Assert.Throws<ArgumentException>(() => new Operation(unSeulMouvement));
    }
    [Fact]
    public void Si_Somme_DesDeuxMouvements_PasNulle_LeveUneException()
    {
        var mouvementsNonNuls = new List<Mouvement>{new Mouvement(1,10m), new Mouvement(5,15m)};
        Assert.Throws<ArgumentException>(()=> new Operation(mouvementsNonNuls));
    }
    [Fact]
    public void Si_Somme_Nulle_ConstruitLOperation()
    {
        var listeMouvement = new List<Mouvement>{new Mouvement(1,15.5m), new Mouvement(7,-15.5m)};
        var operation = new Operation(listeMouvement);
        Assert.Equal(2, operation.Mouvements.Count);
    }
}