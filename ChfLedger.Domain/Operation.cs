namespace ChfLedger.Domain;

public class Operation
{
	public IReadOnlyList<Mouvement> Mouvements => _mouvements;
	private readonly List<Mouvement> _mouvements = new();
	// Constructeur privé dédié à l'usage d'EF afin de ne pas dépendre des Mouvements à la création de l'objet.
	// Aucun appelant dans la codebase, ne pas supprimer
	private Operation() { }
    public Operation(IEnumerable<Mouvement> mouvements)
    {
        var liste = mouvements.ToList();
        if (liste.Count < 2 )
            throw new ArgumentException(
                "Une opération doit avoir deux mouvements au minimum", nameof(mouvements));
        if (liste.Sum(m => m.Montant) != 0)
            throw new ArgumentException(
                "La somme des mouvements doit faire 0", nameof(mouvements));
        _mouvements.AddRange(liste);
    }
}

