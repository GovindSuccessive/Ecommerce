using System.ComponentModel.DataAnnotations;

namespace EcommerceManagement.Models.Domain
{
    public class ForgatePasswordModel
    {
        [Key]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string UserName { get; set; }

        [Required]
        public string Otp {  get; set; }
    }
}
