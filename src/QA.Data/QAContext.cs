using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using QA.Models.Models;

namespace QA.Common.Data
{
    public class QaContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // todo move to some settings storage
            optionsBuilder.UseSqlServer("Server=localhost;initial catalog=QA_DB;user ID=sa;Password=yourStrong(!)Password;TrustServerCertificate=True");
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QACategory>()
                .HasIndex(c => c.Name)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasKey(x => x.Id);
            
            modelBuilder.Entity<QACategory>()
                .HasOne(c => c.Author)
                .WithMany(u => u.CategoriesCreated)
                // todo delete categories and questions on user delete
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<User>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
            
            modelBuilder.Entity<User>()
                .HasMany(u => u.FavoriteCategories)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "UserFavoriteCategories",
                    j => j.HasOne<QACategory>().WithMany().HasForeignKey("QACategoryId").OnDelete(DeleteBehavior.NoAction),
                    j => j.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.NoAction),
                    j =>
                    {
                        j.HasKey("UserId", "QACategoryId");
                    });
            
            modelBuilder.Entity<User>()
                .HasMany(u => u.FavoriteElements)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "UserFavoriteElements",
                    j => j.HasOne<QAElement>().WithMany().HasForeignKey("QAElementId").OnDelete(DeleteBehavior.NoAction),
                    j => j.HasOne<User>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.NoAction),
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

        public QaContext(DbContextOptions<QaContext> opt) : base(opt)
        {

        }

        public DbSet<QACategory> Categories { get; set; }
        public DbSet<QAElement> Elements { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<FeedBack> Feedbacks { get; set; }
    }
    
    public class QaContextFactory : IDesignTimeDbContextFactory<QaContext>
    {
        public QaContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<QaContext>();
            // todo move to some settings storage
            optionsBuilder.UseSqlServer("Server=localhost;initial catalog=QA_DB;user ID=sa;Password=yourStrong(!)Password;TrustServerCertificate=True");

            return new QaContext(optionsBuilder.Options);
        }
    }
}
