using System.ComponentModel.DataAnnotations;

namespace EcommerceManagement.Models.Domain
{
    public class CategoryModel
    {
        [Key]
        public Guid CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string CategoryDescription { get; set; }

        public virtual ICollection<ProductModel> Products { get; set; }
    }
}
