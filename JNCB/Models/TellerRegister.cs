using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JNCB.Models
{
    public class TellerRegister
    {
        [Required]
        [Display(Name = "First Name")]
        [StringLength(60, MinimumLength = 3)]
        public string firstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        [StringLength(60, MinimumLength = 3)]
        public string lastName { get; set; }

        [Required]
        [EmailAddress]
        [Remote(action: "IsEmailInUse", controller: "AdministratorController")]
        [Display(Name = "Enter Customer Email")]
        public string email { get; set; }

        [Required]
        [Display(Name = "Employee ID")]
        public int empid { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Enter Customer Password")]
        public string Password { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and Confirmation password do not match.")]

        public string ConfirmPassword { get; set; }

    }
}
