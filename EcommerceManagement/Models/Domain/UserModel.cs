﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EcommerceManagement.Models.Domain
{
    public class UserModel : IdentityUser
    {
        [Key]
        public override string Id { get; set; }
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(60)]
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string? PhoneNo { get; set; }

        [DataType(DataType.MultilineText)]
        public string? Address { get; set; }

        public string? Role { get; set; } = "User";

        [DefaultValue(true)]
        public bool IsActivate {  get; set; }


        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords don't match.")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public virtual CartModel Cart { get; set; }

        public virtual ICollection<FavoritModel> Favorit { get; set; }



    }
}
