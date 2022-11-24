
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JNCB.Models
{
    public class Transaction
    {

        [Display(Name = "Transaction ID")]
        public string id { get; set; }


        [Display(Name = "Transaction Date")]
        public DateTime transactionDate { get; set; }

        [Required]
        [Display(Name = "Amount")]
        public float Amount { get; set; }

        [Required]
        [Display(Name = "Receiving Account")]
        public long receivingAccount { get; set; }


        [Display(Name = "Remarks")]
        [StringLength(60, MinimumLength = 3)]
        public string remarks { get; set; }

        [Required]
        [Display(Name = "Transaction Type")]
        public string type { get; set; }

        [Required]
        [Display(Name = "Senders Account")]
        [DataType(DataType.Password)]
        [Compare("receivingAccount", ErrorMessage = "Sender and Receiving Account do not match.")]
        // [Remote(action: "CheckBalance", controller: "TransactionsController")]

        public long senderAccount { get; set; }

        public string userID { get; set; }
    }
}
