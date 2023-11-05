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
            
            modelBuilder.Entity<User>()
                .HasMany(u => u.FavoriteElements)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "UserFavoriteElements",
                    j => j.HasOne<QAElement>().WithMany().HasForeignKey("QAElementId"),
                    j => j.HasOne<User>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "QAElementId");
                    });
            
            modelBuilder.Entity<User>()
                .Property(p => p.UserInputMode)
                .HasConversion<string>();
            
            modelBuilder.Entity<FeedBack>()
                .HasOne(f => f.User)
                .WithMany(u => u.FeedBacks);
            
            modelBuilder.Entity<FeedBack>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
        }

        public QAContext(DbContextOptions<QAContext> opt) : base(opt)
        {

        }

        public DbSet<QACategory> Categories { get; set; }
        public DbSet<QAElement> Elements { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<FeedBack> Feedbacks { get; set; }
    }
}
