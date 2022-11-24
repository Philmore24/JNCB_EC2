using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JNCB.Models
{
    public class EditAc
    {
        [Key]
        [Display(Name = "Account Number")]
        public long accountNumber { get; set; }

        [Display(Name = "Available Balance")]
        public float balance { get; set; }
        [Display(Name = "Card Number")]
        public string cardNum { get; set; }

        [Display(Name = "Available Balance")]
        public float availableAmount { get; set; }

        [Required]
        [Display(Name = "Select Type Of Account")]
        public string type { get; set; }

        public string userID { get; set; }
    }
}
