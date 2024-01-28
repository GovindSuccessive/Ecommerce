using EcommerceManagement.Constant;
using Microsoft.Identity.Client;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EcommerceManagement.Models.Dto
{
    public class UpdateUserModel
    {
        public UpdateUserModel()
        {
            Claims = new List<string>();
            Roles= new List<string>();
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNo { get; set; }

        [Required][EmailAddress]
        public string Email { get; set; }

        [Required][PasswordPropertyText]
        public string Password { get; set; }

        public string Address { get; set; }

        public List<string> Claims { get; set; }

        public List<string> Roles {  get; set; }

    }
}
