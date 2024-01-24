using EcommerceManagement.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace EcommerceManagement.Data
{
    public class EcommerceDbContext : DbContext
    {
        public EcommerceDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<UserModel> Users { get; set; }
    }
}
