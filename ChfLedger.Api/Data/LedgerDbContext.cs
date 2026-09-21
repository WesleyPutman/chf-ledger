using Microsoft.EntityFrameworkCore;
using ChfLedger.Domain;

namespace ChfLedger.Api.Data
{
	//Classe qui hérite de DbContext
	public class LedgerDbContext : DbContext{
		//Constructeur qui transmets au parent le Contexte
		public LedgerDbContext(DbContextOptions<LedgerDbContext> options) : base(options) { }
		public DbSet<Compte> Comptes {get; set; }
		public DbSet<Operation> Operations {get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
			{
				base.OnModelCreating(modelBuilder);
				// Conversion de l'enum en String pour éviter les changements de places à l'ajout de nouvelles valeurs.
				modelBuilder.Entity<Compte>().Property(c => c.Type).HasConversion<string>();
				//Ajout de la clé d'ombre pour les Mouvements qui n'est pas renseignée dans le domaine.
				//Décision expliquée dans ADR-011
				modelBuilder.Entity<Mouvement>().Property<int>("Id");
				modelBuilder.Entity<Mouvement>().HasKey("Id");
				// Mouvement au pluriel pour correspondre aux autre tables dans le projet.
				modelBuilder.Entity<Mouvement>().ToTable("Mouvements");
				// Clé étrangère liée au compte sans navigation.
				modelBuilder.Entity<Mouvement>().HasOne<Compte>()
												.WithMany()
												.HasForeignKey("CompteId")
												.OnDelete(DeleteBehavior.Restrict);
				// Conversion de l'enum en String pour éviter les changements de places à l'ajout de nouvelles valeurs.
				modelBuilder.Entity<Operation>().Property(o => o.Code).HasConversion<string>();
				// Relation vers Operation obligatoire pour respecter l'invariant.
				modelBuilder.Entity<Operation>().HasMany(o => o.Mouvements)
												.WithOne()
												.IsRequired()
												.OnDelete(DeleteBehavior.Restrict);
				// Précision du montant pour trancher les arrondis dans le futur avant de créer le mouvement, 2 après la virgule.
				modelBuilder.Entity<Mouvement>().Property(m => m.Montant).HasPrecision(19, 2);
			}
	}
}