using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChfLedger.Api.Errors;

public class GestionnaireExceptionsGlobal : IExceptionHandler
{
	public async ValueTask<bool> TryHandleAsync(
		HttpContext context,
		Exception exception,
		CancellationToken ct)
	{
		if (exception is not ArgumentException && exception is not DbUpdateException)
		{
			return false;
		}
		var probleme = new ProblemDetails
		{
			Status = StatusCodes.Status400BadRequest,
			Title = "Requête invalide",
			Detail = exception.Message
		};
		context.Response.StatusCode = StatusCodes.Status400BadRequest;
		await context.Response.WriteAsJsonAsync(probleme, ct);
		return true;
	}	
}
