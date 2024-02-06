using EcommerceManagement.Models.Domain;

namespace EcommerceManagement.Models.Dto
{
    public class UpdateProductDto
    {
        public Guid ProdId { get; set; }
        public string ProductName { get; set; }

        public string ProductDes { get; set; } // Product description

        public int ProductPrice { get; set; }

        public IFormFile ProductImage { get; set; }

        public string ImagePath { get; set; }
        public bool IsAvailable { get; set; }

        public bool IsTrending { get; set; }

        public bool IsActive { get; set; }

        public DateTime ProductCreated { get; set; }

        public CategoryModel Category { get; set; }

        public Guid CategoryRefId { get; set; }
    }
}

