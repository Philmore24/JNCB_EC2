using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JNCB.Models
{
    public class EditP : IdentityUser
    {
        [Required]
        [Display(Name = "First Name")]
        [StringLength(60, MinimumLength = 3)]
        public string firstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(60, MinimumLength = 3)]
        public string lastName { get; set; }



    }
}
