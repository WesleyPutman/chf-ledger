using ChfLedger.Domain;

namespace ChfLedger.Tests;

public class SoldeTests
{
	[Fact]
	public void Somme_DesMouvements_Correct()
	{
		var listeMouvement = new List<Mouvement>{new Mouvement(1,15.5m), new Mouvement(1,-5m)};
		var resultat = Solde.Calculer(listeMouvement, 1);
		Assert.Equal(10.5m, resultat);
	}
	[Fact]
	public void Aucun_Mouvement_PourLeCompte()
	{
		var listeMouvement = new List<Mouvement>{new Mouvement(1,15.5m), new Mouvement(1,-5m)};
		var resultat = Solde.Calculer(listeMouvement, 0);
		Assert.Equal(0, resultat);
	}
	[Fact]
	public void Melange_DeCompte()
	{
		var listeMouvement = new List<Mouvement>{new Mouvement(1,60m), new Mouvement(1,-20m), new Mouvement(2,30m)};
		var resultat = Solde.Calculer(listeMouvement, 1);
		Assert.Equal(40, resultat);
	}
}