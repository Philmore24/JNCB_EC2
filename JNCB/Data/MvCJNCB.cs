using JNCB.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace JNCB.Data
{
    public class MvCJNCB : IdentityDbContext<ApplicationUser>
    {
        public MvCJNCB()
        {

        }

        public MvCJNCB(DbContextOptions<MvCJNCB> options)
            : base(options)
        {
        }

        public DbSet<Account> Account { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Teller> Teller { get; set; }

        public DbSet<Transaction> accountTransaction { get; set; }
    }
}
