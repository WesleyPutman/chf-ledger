using Microsoft.EntityFrameworkCore;
using ChfLedger.Api.Data;
using ChfLedger.Api.Errors;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Send Json in RFC 7807 format
builder.Services.AddProblemDetails();
// Class declaration for handling exceptions and returning appropriate error messages to the client
builder.Services.AddExceptionHandler<GestionnaireExceptionsGlobal>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddDbContext<LedgerDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
app.UseExceptionHandler();
// IExceptionHandler pour gérer les exceptions et renvoyer un message d'erreur approprié au client


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
