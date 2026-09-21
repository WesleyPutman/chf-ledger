using ChfLedger.Domain;

namespace ChfLedger.Api.Dtos;

public record NouvelleOperation(List<NouveauMouvement> Mouvements, CodeOperation Code);