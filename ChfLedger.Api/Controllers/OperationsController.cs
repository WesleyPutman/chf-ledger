using Microsoft.AspNetCore.Mvc;
using ChfLedger.Api.Dtos;
using ChfLedger.Domain;
using ChfLedger.Api.Data;

namespace ChfLedger.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OperationsController : ControllerBase
{
	private readonly LedgerDbContext _context;

	public OperationsController(LedgerDbContext context)
	{
		_context = context;
	}
	[HttpPost]
	public async Task<ActionResult<OperationEnregistree>> Creer(NouvelleOperation nouvelleOperation)
	{
		var operation = new Operation(nouvelleOperation.Mouvements.Select(m => new Mouvement(m.CompteId, m.Montant)), nouvelleOperation.Code);
		_context.Operations.Add(operation);
		await _context.SaveChangesAsync();
		return new OperationEnregistree(operation.Id, operation.Mouvements.Select(m => new MouvementEnregistre( m.CompteId, m.Montant)).ToList(), operation.Code);
	}
}