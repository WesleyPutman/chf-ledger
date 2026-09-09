namespace ChfLedger.Domain;

public static class Solde
{
	public static decimal Calculer(IEnumerable<Mouvement>mouvements,int compteId)
	{
		return mouvements.Where(s => s.CompteId == compteId).Sum(s => s.Montant);
	}
}