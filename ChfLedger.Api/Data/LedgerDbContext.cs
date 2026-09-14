using Microsoft.EntityFrameworkCore;
using ChfLedger.Domain;

namespace ChfLedger.Api.Data
{
	//Classe qui hérite de DbContext
	public class LedgerDbContext : DbContext{
		//Constructeur qui transmets au parent le Contexte
		public LedgerDbContext(DbContextOptions<LedgerDbContext> options) : base(options) { }
		public DbSet<Compte> Clients {get; set; }
		public DbSet<Operation> Operations {get; set; }
	}
}