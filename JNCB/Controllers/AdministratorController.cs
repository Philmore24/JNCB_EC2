using JNCB.Data;
using JNCB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JNCB.Controllers
{

    public class AdministratorController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        private readonly RoleManager<IdentityRole> roleManager;

        private readonly MvCJNCB _context;



        public AdministratorController(RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, MvCJNCB
            context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            _context = context;
        }

        //[Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }


        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Confirm(Customer customer, Account account)
        {
            ViewBag.accountNumber = HttpContext.Session.GetString("accountNum");
            ViewBag.address = HttpContext.Session.GetString("address");
            ViewBag.cardNum = HttpContext.Session.GetString("cardnum");
            ViewBag.balance = HttpContext.Session.GetString("balance");
            ViewBag.type = HttpContext.Session.GetString("type");
            ViewBag.email = HttpContext.Session.GetString("email");
            ViewBag.ID = HttpContext.Session.GetString("userID");

            customer.id = ViewBag.ID;
            customer.address = ViewBag.address;

            var users = userManager.Users.Where(x => x.Id.Equals(customer.id));

            account.accountNumber = Convert.ToInt64(ViewBag.accountNumber);
            account.cardNum = ViewBag.cardNum;
            account.balance = Convert.ToSingle(ViewBag.balance);
            account.availableAmount = Convert.ToSingle(ViewBag.balance);
            account.type = ViewBag.type;
            account.userID = ViewBag.ID;


            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                _context.Add(account);
                await _context.SaveChangesAsync();
            }

            return View(users);
        }

        //[Authorize(Roles = "Admin")]

        [HttpPost]
        public async Task<IActionResult> Create(RegisterAccount model)
        {
            // string accountNum = Convert.ToString(model.accountNumber);
            string address = model.streetAddress;
            // string cardnum = model.cardNum;
            string balance = Convert.ToString(model.balance);
            string type = model.type;
            string email = model.Email;

            /* assign an account number starting with 212(seven digits) and card num(12 digits) starting with 4001. last part of
            numbers will be increment base on existing numbers from database or default from start*/

            // Account account = null;


            int countAccount = _context.Account.Count();

            if (countAccount < 1)
            {
                int accountFinish = 2000;
                string accountLast = Convert.ToString(accountFinish);
                string accountStart = String.Concat("919", accountLast);
                long finalAccount = Convert.ToInt64(accountStart); //because accountNum is of type long
                                                                   // account.accountNumber = finalAccount; //first account num is 2121000
                HttpContext.Session.SetString("accountNum", accountStart);

                int cardNumFinsh = 8765432;
                string cardLast = Convert.ToString(cardNumFinsh);
                string cardStart = "9505" + cardLast;
                // account.cardNum = cardStart;
                HttpContext.Session.SetString("cardnum", cardStart);

            }
            else
            {
                int accountFinish = 2000;
                int newAccountFinish = (countAccount * 2) + accountFinish;
                string accountLast = Convert.ToString(newAccountFinish);
                string accountStart = "919" + accountLast;
                long finalAccount = Convert.ToInt64(accountStart);
                //account.accountNumber = finalAccount;
                HttpContext.Session.SetString("accountNum", accountStart);

                int cardNumFinsh = 8765432;
                int newCardFinish = countAccount + cardNumFinsh;
                string cardLast = Convert.ToString(newCardFinish);
                string cardStart = "9505" + cardLast;
                //  account.cardNum = cardStart;
                HttpContext.Session.SetString("cardnum", cardStart);

            }

            //  HttpContext.Session.SetString("accountNum", );
            HttpContext.Session.SetString("address", address);
            // HttpContext.Session.SetString("cardnum", cardnum);
            HttpContext.Session.SetString("balance", balance);
            HttpContext.Session.SetString("type", type);
            HttpContext.Session.SetString("email", email);

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, firstName = model.firstName, lastName = model.lastName };
                var result = await userManager.CreateAsync(user, model.Password);
                HttpContext.Session.SetString("userID", user.Id);

                if (result.Succeeded)
                {
                    return RedirectToAction("Confirm", "Administrator");
                    
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [Authorize(Roles = "Admin,Teller")]
        public async Task<IActionResult> viewAll()
        {         
            var users = await userManager.GetUsersInRoleAsync("Customer");
            return View(users);
        }

        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> viewAllTeller()
        {
            var users = await userManager.GetUsersInRoleAsync("Teller");
            return View(users);
        }

        public IActionResult Open()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {

            ApplicationUser user = await userManager.FindByIdAsync(id);
            Customer customer = await _context.Customer.FirstOrDefaultAsync(y => y.id.Equals(id));
            var teller = await userManager.IsInRoleAsync(user, "Teller");

            HttpContext.Session.SetString("FirstName", user.firstName);
            HttpContext.Session.SetString("LastName", user.lastName);
            HttpContext.Session.SetString("Email", user.UserName);

            if (teller == true)
            {
                HttpContext.Session.SetString("Address", "None"); // start here

            }
            else
            {
                HttpContext.Session.SetString("Address", customer.address); // start here
            }

            HttpContext.Session.SetString("ID", user.Id);
            ViewBag.firstName = HttpContext.Session.GetString("FirstName");
            ViewBag.lastName = HttpContext.Session.GetString("LastName");
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.Address = HttpContext.Session.GetString("Address");
            ViewBag.ID = HttpContext.Session.GetString("ID");

            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Account
                .FirstOrDefaultAsync(m => m.userID.Equals(id));

            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> TellerDetails(string id)
        {

            ApplicationUser user = await userManager.FindByIdAsync(id);
            Teller teller = await _context.Teller.FirstOrDefaultAsync(y => y.id.Equals(id));


            HttpContext.Session.SetString("FirstName", user.firstName);
            HttpContext.Session.SetString("LastName", user.lastName);
            HttpContext.Session.SetString("Email", user.UserName);
            HttpContext.Session.SetString("ID", user.Id);
            ViewBag.firstName = HttpContext.Session.GetString("FirstName");
            ViewBag.lastName = HttpContext.Session.GetString("LastName");
            ViewBag.Email = HttpContext.Session.GetString("Email");
            ViewBag.ID = HttpContext.Session.GetString("ID");

            if (id == null)
            {
                return NotFound();
            }

            var tellers = await _context.Teller
                .FirstOrDefaultAsync(m => m.id.Equals(id));

            if (tellers == null)
            {
                return NotFound();
            }

            return View(tellers);
        }

        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Account
                .FirstOrDefaultAsync(m => m.userID.Equals(id));
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTeller(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teller = await _context.Teller
                .FirstOrDefaultAsync(m => m.id.Equals(id));
            if (teller == null)
            {
                return NotFound();
            }

            return View(teller);
        }

        //[Authorize(Roles = "Admin")]
        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);

            var customer = await _context.Customer.FindAsync(id);
            _context.Customer.Remove(customer);
            await _context.SaveChangesAsync();
            var account = await _context.Account.FirstOrDefaultAsync(a => a.userID.Equals(id));
            _context.Account.Remove(account);
            await _context.SaveChangesAsync();

            var result = await userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                Console.WriteLine("User Deleted");
            }

            return RedirectToAction("ViewAll");

        }

        //[Authorize(Roles = "Admin")]
        // POST: Accounts/Delete/5
        [HttpPost, ActionName("DeleteTeller")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedT(string id)
        {
            ApplicationUser user = await userManager.FindByIdAsync(id);

            var teller = await _context.Teller.FindAsync(id);
            _context.Teller.Remove(teller);
            await _context.SaveChangesAsync();


            var result = await userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                Console.WriteLine("User Deleted");
            }

            return RedirectToAction("ViewAllTeller");

        }

        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with id = {id} cannot be found";
                return View("Not Found");
            }
            else
            {
                var result = await roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("Roles");
            }
        }

        //[Authorize(Roles = "Admin")]
        // GET: Bills/Edit/5
        public async Task<IActionResult> EditP(string id, ApplicationUser model)
        {
            if (id == null)
            {

                return NotFound();
            }

            var user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        //[Authorize(Roles = "Admin")]
        // POST: Bills/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditP(string id, ApplicationUser user, Customer customer)
        {
            ApplicationUser model = await userManager.FindByIdAsync(id);

            if (model == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} Cannot be found";
                return View("NotFound");
            }
            else
            {
                model.firstName = user.firstName;
                model.lastName = user.lastName;
                model.Email = user.Email;

                var result = await userManager.UpdateAsync(model);

                if (result.Succeeded)
                {
                    return RedirectToAction("ViewAll");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        //[Authorize(Roles = "Admin")]

        // GET: Bills/Edit/5
        public async Task<IActionResult> EditT(string id, ApplicationUser model)
        {
            if (id == null)
            {

                return NotFound();
            }

            var user = await userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        //[Authorize(Roles = "Admin")]
        // POST: Bills/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditT(string id, ApplicationUser user, Teller teller)
        {
            ApplicationUser model = await userManager.FindByIdAsync(id);

            if (model == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} Cannot be found";
                return View("NotFound");
            }
            else
            {
                model.firstName = user.firstName;
                model.lastName = user.lastName;
                model.Email = user.Email;

                var result = await userManager.UpdateAsync(model);

                if (result.Succeeded)
                {
                    return RedirectToAction("ViewAllTeller");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        //[Authorize(Roles = "Admin")]

        // GET: Bills/Edit/5
        public async Task<IActionResult> EditAd(string id)
        {
            if (id == null)
            {

                return NotFound();
            }

            var customer = await _context.Customer.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }


        //[Authorize(Roles = "Admin")]
        // POST: Bills/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAd(string id, Customer customer)
        {
            var newcustomer = await _context.Customer.FindAsync(id);

            newcustomer.id = customer.id;
            newcustomer.address = customer.address;

            if (id != customer.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(newcustomer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("ViewAll");
            }
            return View(customer);
        }

        //[Authorize(Roles = "Admin")]

        // GET: Bills/Edit/5
        public async Task<IActionResult> EditID(string id)
        {
            if (id == null)
            {

                return NotFound();
            }

            var teller = await _context.Teller.FindAsync(id);

            if (teller == null)
            {
                return NotFound();
            }
            return View(teller);
        }

        //[Authorize(Roles = "Admin")]

        // POST: Bills/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditID(string id, Teller teller)
        {
            var newTeller = await _context.Teller.FindAsync(id);

            newTeller.id = teller.id;
            newTeller.empid = teller.empid;

            if (id != teller.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(newTeller);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(teller.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("ViewAllTeller");
            }
            return View(teller);
        }

        //[Authorize(Roles = "Admin")]

        // GET: Bills/Edit/5
        public async Task<IActionResult> EditAc(string id)
        {
            if (id == null)
            {

                return NotFound();
            }


            var account = await _context.Account.FirstOrDefaultAsync(a => a.userID.Equals(id));

            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }

        //[Authorize(Roles = "Admin")]
        // POST: Bills/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAc(string id, Account account)
        {
            var newaccount = await _context.Account.FirstOrDefaultAsync(a => a.userID.Equals(id));


            newaccount.type = account.type;


            if (id != account.userID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(newaccount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.userID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("ViewAll");
            }
            return View(account);
        }

        private bool CustomerExists(string id)
        {
            return _context.Customer.Any(e => e.id == id);
        }

        private bool AccountExists(string id)
        {
            return _context.Account.Any(e => e.userID == id);
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<ActionResult> IsEmailInUse(string Email)
        {
            var user = await userManager.FindByEmailAsync(Email);

            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {Email} is already in use");
            }
        }



        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleView model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };

                IdentityResult result = await roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Administrator");

                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);

                }
            }
            return View();
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} Cannot be found";
                return View("NotFound");
            }

            var model = new EditRoles
            {
                Id = role.Id,
                RoleName = role.Name
            };

            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.User.Add(user.UserName);
                }
            }
            return View(model);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoles model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.Id} Cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }


        }

        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> EditUserInRoles(string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with ID = {roleId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRoles>();

            foreach (var user in userManager.Users)
            {
                var userRoleViewModel = new UserRoles
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }

            return View(model);
        }


        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> EditUserInRoles(List<UserRoles> model, string roleId)
        {

            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with ID = {roleId} cannot be found";
                return View("NotFound");
            }


            for (int i = 0; i < model.Count; i++)
            {

                var users = await userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result = null;

                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(users, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(users, role.Name);
                }
                else if (!model[i].IsSelected && (await userManager.IsInRoleAsync(users, role.Name)))
                {
                    result = await userManager.RemoveFromRoleAsync(users, role.Name);

                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                    {
                        continue;
                    }
                    else
                    {
                        return RedirectToAction("EditRole", new { Id = roleId });
                    }
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });

        }

        //[Authorize(Roles = "Admin")]
        public IActionResult CreateTeller()
        {
            return View();
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateTeller(TellerRegister model)
        {


            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.email, Email = model.email, firstName = model.firstName, lastName = model.lastName };
                var result = await userManager.CreateAsync(user, model.Password);
                HttpContext.Session.SetString("userID", user.Id);
                HttpContext.Session.SetString("empID", Convert.ToString(model.empid));
                HttpContext.Session.SetString("email", model.email);

                if (result.Succeeded)
                {
                    return RedirectToAction("ConfirmTeller", "Administrator");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ConfirmTeller(Teller teller)
        {

            ViewBag.email = HttpContext.Session.GetString("email");
            ViewBag.empid = HttpContext.Session.GetString("empID");
            ViewBag.ID = HttpContext.Session.GetString("userID");
            int empid = Convert.ToInt32(ViewBag.empid);


            teller.id = ViewBag.ID;
            teller.empid = empid;

            var users = userManager.Users.Where(x => x.Id.Equals(teller.id));


            if (ModelState.IsValid)
            {
                _context.Add(teller);
                await _context.SaveChangesAsync();
            }

            return View(users);
        }



    }
}