using EcommerceManagement.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceManagement.Configuration
{
    public class CartConfiguration : IEntityTypeConfiguration<CartModel>
    {
        public void Configure(EntityTypeBuilder<CartModel> builder)
        {
            builder.ToTable("Cart");
            builder.Property(z => z.CartId).IsUnicode(true).ValueGeneratedOnAdd();
            builder.HasOne(x => x.User)
                .WithOne(x => x.Cart)
                .HasForeignKey<CartModel>(c => c.UserRefId)
                .IsRequired(false);
            
            builder.HasOne(x=>x.Product)
                .WithOne(x=>x.Cart)
                .HasForeignKey<CartModel>(x=>x.ProductId)
                .IsRequired(false);





        }
    }
}
