using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JNCB.Models
{
    public class EditRoles
    {

        public EditRoles()
        {
            User = new List<string>();
        }
        public string Id { get; set; }

        [Required(ErrorMessage = "Role Name is Required")]
        public string RoleName { get; set; }

        public List<string> User { get; set; }
    }
}
