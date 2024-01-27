using EcommerceManagement.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceManagement.Configuration
{
    public class CategoryConfiguration : IEntityTypeConfiguration<CategoryModel>
    {
        public void Configure(EntityTypeBuilder<CategoryModel> builder)
        {
           builder.ToTable("Category");
            builder.Property(x => x.CategoryId).IsUnicode(true).ValueGeneratedOnAdd();
            builder.HasMany(x => x.Products)
                .WithOne(x => x.Category)
                .HasPrincipalKey(x => x.CategoryId)
                .HasForeignKey(x => x.CategoryRefId);
        }
    }
}
