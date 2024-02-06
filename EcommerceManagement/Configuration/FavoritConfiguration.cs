using EcommerceManagement.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceManagement.Configuration
{
    public class FavoritConfiguration:IEntityTypeConfiguration<FavoritModel>
    {
        public void Configure(EntityTypeBuilder<FavoritModel> builder)
        {
            /*builder.ToTable("Cart");
            builder.Property(z => z.CartId).IsUnicode(true).ValueGeneratedOnAdd();
            builder.HasOne(x => x.User)
                .WithOne(x => x.Cart)
                .HasForeignKey<CartModel>(c => c.UserRefId)
                .IsRequired(false);

            builder.HasOne(x => x.Product)
                .WithOne(x => x.Cart)
                .HasForeignKey<CartModel>(x => x.ProductId)
                .IsRequired(false);*/

            builder.ToTable("Favorit");
            builder.Property(z => z.FavoritId);
            builder.HasOne(x => x.User)
                .WithMany(x => x.Favorit)
                .HasForeignKey(x => x.UserRefId)
                .IsRequired(false);

            builder.HasOne(x => x.Product)
                .WithOne(x=>x.Favorit)
                .HasForeignKey<FavoritModel>(x => x.ProductRefId)
                .IsRequired(true);






        }
    }
}
