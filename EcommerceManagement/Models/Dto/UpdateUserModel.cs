﻿using EcommerceManagement.Constant;
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

        public Guid Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNo { get; set; }

        [Required][EmailAddress]
        public string Email { get; set; }


        public string? OldPassword { get; set; }

       
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Passwords don't match.")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }

        public string Address { get; set; }

        public IList<string> Claims { get; set; }

        public IList<string> Roles {  get; set; }

    }
}
