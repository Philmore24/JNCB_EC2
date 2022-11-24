using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using JNCB.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JNCB.Controllers
{
   
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;


        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }



        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "Home");
        }




        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {

            return View();
        }



        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginView model, string returnUrl)
        {


            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);




                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl))
                    {

                        return LocalRedirect(returnUrl);

                    }
                    else
                    {
                        var user = await userManager.FindByEmailAsync(model.Email);

                        HttpContext.Session.SetString("userID", user.Id);


                        var userrole = await userManager.IsInRoleAsync(user, "Customer");

                        if (userrole == true)
                        {
                            HttpContext.Session.SetString("user", user.Id);
                            TempData["customer"] = "true";
                            HttpContext.Session.SetString("username", model.Email);
                            HttpContext.Session.SetString("firstname", user.firstName);
                            HttpContext.Session.SetString("lastname", user.lastName);



                            // string id = null;
                            return RedirectToAction("CustomerTrans", "Transactions", user.Id);
                        }
                        else
                        {
                            TempData["customer"] = "false";

                            return RedirectToAction("index", "Home");
                        }





                    }
                }


                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");


            }
            return View(model);
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

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}