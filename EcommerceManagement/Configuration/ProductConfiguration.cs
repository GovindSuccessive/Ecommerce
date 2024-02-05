using EcommerceManagement.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceManagement.Configuration
{
    public class ProductConfiguration : IEntityTypeConfiguration<ProductModel>
    {
        public void Configure(EntityTypeBuilder<ProductModel> builder)
        {
            builder.ToTable("Product");
            builder.Property(x => x.ProductId).IsUnicode(true).ValueGeneratedOnAdd();
            builder.HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryRefId)
                .IsRequired(false);
                

            builder.HasOne(x => x.Cart)
                .WithOne(x => x.Product)
                .HasPrincipalKey<ProductModel>(x => x.ProductId)
                .IsRequired(false);
        }
    }
}
