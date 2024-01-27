namespace EcommerceManagement.Models.Dto
{
    public class ProductCategoryDto
    {
        public Guid ProductId { get; set; }

        public Guid CatagoryId { get; set; }

        public string ProductName { get; set; }

        public string CatagoryName { get; set; }

        public string CategoryDes {  get; set; }

        public string ProductDes { get; set; } // Product description

        public int ProductPrice { get; set; }

        public string ProductImage { get; set; }

        public bool IsAvailable { get; set; }

        public bool IsTrending { get; set; }
    }
}
