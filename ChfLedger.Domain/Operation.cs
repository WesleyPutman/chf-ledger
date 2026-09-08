namespace ChfLedger.Domain;

public class Operation
{
    public IReadOnlyList<Mouvement> Mouvements { get; }
    public Operation(IEnumerable<Mouvement> mouvements)
    {
        var liste = mouvements.ToList();
        if (liste.Count < 2 )
            throw new ArgumentException(
                "Une opération doit avoir deux mouvements au minimum", nameof(mouvements));
        if (liste.Sum(m => m.Montant) != 0)
            throw new ArgumentException(
                "La somme des mouvements doit faire 0", nameof(mouvements));
        Mouvements = liste;
    }
}