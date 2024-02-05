using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceManagement.Models.Domain
{
    public class CartModel
    {
        [Key]
        public Guid CartId { get; set; }

        public int CartItems { get; set; }

        public Guid ProductRefId { get; set; }

        public Guid ProductId { get; set; }

        public virtual ProductModel Product { get; set; }

        public string UserRefId { get; set; }

        public virtual UserModel User { get; set; }
    }
}
