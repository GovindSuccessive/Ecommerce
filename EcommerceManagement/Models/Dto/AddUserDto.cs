using System.Text;

namespace EcommerceManagement.Models.Dto
{
    public class AddUserDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNo { get; set; }
        public string Address { get; set; }

        public string Role { get; set; }

        public StringBuilder Password { get; set; }
    }
}
