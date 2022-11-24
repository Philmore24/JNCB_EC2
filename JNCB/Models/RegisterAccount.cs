using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JNCB.Models
{
    public class RegisterAccount
    {
        [Required]
        [EmailAddress]
        [Remote(action: "IsEmailInUse", controller: "AdministratorController")]
        [Display(Name = "Enter Customer Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Enter Customer Password")]
        public string Password { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and Confirmation password do not match.")]

        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 3)]
        [Display(Name = "Enter Customer First Name")]
        public string firstName { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 3)]
        [Display(Name = "Enter Customer Last Name")]
        public string lastName { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 3)]
        [Display(Name = "Enter Customer Address")]
        public string streetAddress { get; set; }
        [Required]
        public long accountNumber { get; set; }
        [Required]
        [Display(Name = "Enter Customer Opening Balance")]
        public float balance { get; set; }

        public string cardNum { get; set; }

        public float availableAmount { get; set; }
        [Required]
        [Display(Name = "Select Type Of Account")]
        public string type { get; set; }

        public string userID { get; set; }
    }
}
