using EcommerceManagement.Configuration;
using EcommerceManagement.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EcommerceManagement.Data
{
    public class EcommerceDbContext : IdentityDbContext<UserModel, IdentityRole<string>, string>
    {
        public EcommerceDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<FavoritModel> Favorits { get; set; }
        public DbSet<UserModel> Users { get; set; }

        public DbSet<IdentityRole> Roles { get; set; }

        public DbSet<ProductModel> Products { get; set; }

        public DbSet<CategoryModel> Categories { get; set; }

        public DbSet<CartModel> Carts { get; set; }

        public DbSet<ForgatePasswordModel> ForgatePassword  { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            modelBuilder.Entity<UserModel>().ToTable("AspNetUsers");
            modelBuilder.ApplyConfiguration(new FavoritConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new CartConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
