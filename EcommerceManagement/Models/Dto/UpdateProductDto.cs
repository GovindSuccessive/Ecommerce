﻿using EcommerceManagement.Models.Domain;

namespace EcommerceManagement.Models.Dto
{
    public class UpdateProductDto
    {
        public  Guid ProductId;
        public string ProductName { get; set; }

        public string ProductDes { get; set; } // Product description

        public int ProductPrice { get; set; }

        public string ProductImage { get; set; }

        public bool IsAvailable { get; set; }

        public bool IsTrending { get; set; }

        public CategoryModel Category { get; set; }

        public Guid CategoryRefId { get; set; }
    }
}

