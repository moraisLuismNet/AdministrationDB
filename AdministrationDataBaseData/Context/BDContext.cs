using Microsoft.EntityFrameworkCore;
using AdministrationDataBaseData.Models;

namespace AdministrationDataBase.Context
{
    public class BDContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<MassagesCustomer> MassagesCustomers { get; set; }
        public DbSet<Pathology> Pathologies { get; set; }
        public DbSet<MassagesCustomerPathology> MassagesCustomerPathologies { get; set; }
        public DbSet<Observation> Observations { get; set; }
        public DbSet<Massage> Massages { get; set; }
        public DbSet<PilatesCustomer> PilatesCustomers { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Objective> Objectives { get; set; }
        public DbSet<PilatesCustomerObjective> PilatesCustomerObjectives { get; set; }

        public BDContext()
        {
        }

        public BDContext(DbContextOptions<BDContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relationship between Pathology and Observation (one to many)
            modelBuilder.Entity<Observation>()
                .HasOne(o => o.Pathology)
                .WithMany(p => p.Observations)
                .HasForeignKey(o => o.PathologyId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade elimination

            // Relationship between Observation and MassagesCustomer 
            modelBuilder.Entity<Observation>()
                .HasOne(o => o.MassagesCustomer)
                .WithMany(c => c.Observations) // MassagesCustomer has many observations
                .HasForeignKey(o => o.MassagesCustomerId) // We use MassagesCustomerId as a foreign key
                .OnDelete(DeleteBehavior.Cascade); // Cascade elimination

            // Setting up the many-to-many relationship between MassagesCustomer and Pathology
            modelBuilder.Entity<MassagesCustomerPathology>()
                .HasKey(cp => new { cp.MassagesCustomerId, cp.PathologyId });

            modelBuilder.Entity<MassagesCustomerPathology>()
                .HasOne(cp => cp.MassagesCustomer)
                .WithMany(c => c.MassagesCustomerPathologies)
                .HasForeignKey(cp => cp.MassagesCustomerId);

            modelBuilder.Entity<MassagesCustomerPathology>()
                .HasOne(cp => cp.Pathology)
                .WithMany(p => p.MassagesCustomerPathologies)
                .HasForeignKey(cp => cp.PathologyId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade elimination 

            // Setting up the one-to-many relationship between MassagesCustomer
            modelBuilder.Entity<Massage>()
                .HasOne(i => i.MassagesCustomer)
                .WithMany(c => c.Massages)
                .HasForeignKey(i => i.IdMassagesCustomer)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-many relationship between PilatesCustomer and Session
            modelBuilder.Entity<Session>()
                .HasOne(c => c.PilatesCustomer)
                .WithMany(c => c.Sessions)
                .HasForeignKey(c => c.PilatesCustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Many-to-Many Relationship PilatesCustomer Objective
            modelBuilder.Entity<PilatesCustomerObjective>()
                .HasKey(co => new { co.PilatesCustomerId, co.ObjectiveId });

            modelBuilder.Entity<PilatesCustomerObjective>()
                .HasOne(co => co.PilatesCustomer)
                .WithMany(c => c.PilatesCustomerObjectives)
                .HasForeignKey(co => co.PilatesCustomerId);

            modelBuilder.Entity<PilatesCustomerObjective>()
                .HasOne(co => co.Objective)
                .WithMany(o => o.PilatesCustomerObjectives)
                .HasForeignKey(co => co.ObjectiveId);
        }

    }
}

