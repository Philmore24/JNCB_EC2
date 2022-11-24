using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JNCB.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace JNCB.Models
{
    public class TransactionsController : Controller
    {
        private readonly MvCJNCB _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;




        public TransactionsController(MvCJNCB context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            _context = context;
            this.signInManager = signInManager;
        }




        // GET: Transactions
        [HttpGet]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CustomerTrans()
        {
            try
            {

                ViewBag.user = HttpContext.Session.GetString("user"); //userid
                TempData["USERID"] = ViewBag.user;
                string newid = Convert.ToString(ViewBag.user);
                ViewBag.userid = HttpContext.Session.GetString("user");
                var account = await _context.Account.FirstOrDefaultAsync(y => y.userID == newid);
                var customer = await _context.Customer.FirstOrDefaultAsync(x => x.id == newid);
                TempData["Balance"] = account.balance;
                ViewBag.username = HttpContext.Session.GetString("username");
                ViewBag.firstName = HttpContext.Session.GetString("firstname");
                ViewBag.lastName = HttpContext.Session.GetString("lastname");
                ViewBag.address = customer.address;
                ViewBag.accountNum = account.accountNumber;
                ViewBag.available = account.availableAmount;
                ViewBag.type = account.type;
                ViewBag.card = account.cardNum;


                // TempData["ID"] = id;

                var transaction = await _context.accountTransaction.ToListAsync();

                return View(transaction);
            }
            catch (NullReferenceException)
            {

                await signInManager.SignOutAsync();

                return RedirectToAction("None", "Transactions");
            }

        }

        public IActionResult None()
        {
            return View();
        }


        [Authorize(Roles = "Teller")]
        public async Task<IActionResult> Index()
        {

            return View(await _context.accountTransaction.ToListAsync());
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(string id)
        {
            ViewBag.user = HttpContext.Session.GetString("user"); //userid
            TempData["USERID"] = ViewBag.user;

            if (id == null)
            {
                return NotFound();
            }



            var transaction = await _context.accountTransaction
                .FirstOrDefaultAsync(m => m.id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transactions/Create
        [Authorize(Roles = "Teller")]
        public IActionResult Create()
        {

            return View();
        }
        [Authorize(Roles = "Teller")]
        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,transactionDate,Amount,receivingAccount,remarks,type,senderAccount,userID")] Transaction transaction)
        {
           
                
            
            var account1 = await _context.Account.FirstOrDefaultAsync(x => x.accountNumber == transaction.senderAccount);
            try
            {
                if (account1.userID == null)
                {
                    return RedirectToAction("AdminError", "Transactions");
                }
            }
            catch (NullReferenceException)
            {
                return RedirectToAction("AdminError", "Transactions");
            }

                transaction.userID = account1.userID;

                if (ModelState.IsValid)
                {
                    string day = DateTime.Now.Day.ToString("00"); //22
                    string month = DateTime.Now.Month.ToString("00"); // 04
                    string year = Convert.ToString(DateTime.Now.Year); //2020
                    string hour = DateTime.Now.Hour.ToString("00");
                    string min = DateTime.Now.Minute.ToString("00");
                    string second = DateTime.Now.Second.ToString("00");

                    transaction.id = day + month + year + hour + min + second;



                    transaction.transactionDate = DateTime.Now;

                if(transaction.type.Equals("Self Withdrawal") && transaction.Amount > account1.balance)
                {
                   return RedirectToAction("AdminError", "Transactions");
                }

                    _context.Add(transaction);
                    await _context.SaveChangesAsync();

                    transaction.receivingAccount = transaction.senderAccount;
                    //credit receiving account
                    if (transaction.type.Equals("Self Deposit"))
                    {
                        Account account = await _context.Account.FirstOrDefaultAsync(x => x.accountNumber == transaction.receivingAccount);
                        account.balance = account.balance + transaction.Amount;
                        account.availableAmount = account.balance;
                        _context.Update(account);
                        await _context.SaveChangesAsync();
                    }
                    else if (transaction.type.Equals("Self Withdrawal"))
                    {
                        Account account = await _context.Account.FirstOrDefaultAsync(x => x.accountNumber == transaction.receivingAccount);

                        if(account.balance < transaction.Amount)
                    {
                       return RedirectToAction("None", "Transactions");
                    }
                        account.balance = account.balance - transaction.Amount;
                        account.availableAmount = account.balance;
                        _context.Update(account);
                        await _context.SaveChangesAsync();
                    }


                    return RedirectToAction(nameof(Index));
                }
            
            
            return View(transaction);
        }
        [Authorize(Roles = "Teller")]
        public IActionResult AdminError()
        {
            return View();
        }

        [Authorize(Roles = "Teller")]

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.accountTransaction.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            return View(transaction);
        }

        [Authorize(Roles = "Teller")]

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("id,transactionDate,Amount,receivingAccount,remarks,type,senderAccount")] Transaction transaction)
        {
            if (id != transaction.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(transaction);
        }

        [Authorize(Roles = "Teller")]
        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.accountTransaction
                .FirstOrDefaultAsync(m => m.id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        [Authorize(Roles = "Teller")]

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var transaction = await _context.accountTransaction.FindAsync(id);
            _context.accountTransaction.Remove(transaction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(string id)
        {
            return _context.accountTransaction.Any(e => e.id == id);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<ActionResult> CheckAccount(long accountNum)
        {
            var user = await _context.Account.FirstOrDefaultAsync(x => x.accountNumber == accountNum);

            if (user == null)
            {

                return Json($"Account {accountNum} doesn't exist");
            }
            else
            {
                return Json(true);
            }
        }




    }
}