using Microsoft.EntityFrameworkCore;
using QA_API.Models;

namespace QA_API.Data
{
    public class QAContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QACategory>()
                .HasIndex(c => c.Name)
                .IsUnique();

            modelBuilder.Entity<UserState>()
                .HasKey(x => x.Id);
            
            modelBuilder.Entity<UserState>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
        }

        public QAContext(DbContextOptions<QAContext> opt) : base(opt)
        {

        }

        public DbSet<QACategory> Categories { get; set; }
        public DbSet<QAElement> Elements { get; set; }
        public DbSet<UserState> UserStates { get; set; }
    }
}
