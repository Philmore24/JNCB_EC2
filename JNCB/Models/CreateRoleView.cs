using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JNCB.Models
{
    public class CreateRoleView
    {
        [Required]
        public string RoleName { get; set; }
    }
}
