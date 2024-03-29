﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceManagement.Models.Domain
{
    public class ProductModel
    {
        [Key]
        public Guid ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductDes {  get; set; } // Product description

        public int ProductPrice { get; set; }

        [NotMapped]
        public IFormFile ProductImageUplode { get; set; }

        public string ProductImage { get; set; }

        [DefaultValue(true)]
        public bool IsAvailable { get; set; }

        [DefaultValue(false)]
        public bool IsTrending { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }

        public DateTime ProductCreated { get; set; }

        public virtual CategoryModel Category { get; set; }

        public Guid CategoryRefId { get; set; }

        public virtual CartModel Cart { get; set; }

        public virtual FavoritModel Favorit { get; set; }
    }
}
