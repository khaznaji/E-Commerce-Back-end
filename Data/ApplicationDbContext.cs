using E_Commerce.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Data
{

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Product> Products {  get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategory { get; set; }
        public DbSet<User> User { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)  
        {
            modelBuilder.Entity<SubCategory>()
                .Property(s => s.Id)
                .ValueGeneratedOnAdd();
        }

    }
}
