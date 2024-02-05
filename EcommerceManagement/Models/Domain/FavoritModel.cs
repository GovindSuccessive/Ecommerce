using System.ComponentModel.DataAnnotations;

namespace EcommerceManagement.Models.Domain
{
    public class FavoritModel
    {
        [Key]
        public Guid FavoritId { get; set; }

        public Guid ProductRefId { get; set; }

        public virtual ProductModel Product { get; set; }

        public string UserRefId { get; set; }

        public virtual UserModel User { get; set; }
    }
}
