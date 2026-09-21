using ChfLedger.Domain;

namespace ChfLedger.Api.Dtos;

public record OperationEnregistree(int Id, IReadOnlyList<MouvementEnregistre> Mouvements, CodeOperation Code);