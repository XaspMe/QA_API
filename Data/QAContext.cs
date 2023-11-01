using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using QA_API.Data;
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

            modelBuilder.Entity<User>()
                .HasKey(x => x.Id);
            
            modelBuilder.Entity<User>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
            
            modelBuilder.Entity<User>()
                .HasMany(u => u.FavoriteCategories)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "UserFavoriteCategories",
                    j => j.HasOne<QACategory>().WithMany().HasForeignKey("QACategoryId"),
                    j => j.HasOne<User>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "QACategoryId");
                    });
        }

        public QAContext(DbContextOptions<QAContext> opt) : base(opt)
        {

        }

        public DbSet<QACategory> Categories { get; set; }
        public DbSet<QAElement> Elements { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
