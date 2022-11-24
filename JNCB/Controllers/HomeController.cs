using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using JNCB.Models;
using Microsoft.AspNetCore.Identity;
using JNCB.Data;
using Microsoft.AspNetCore.Http;

namespace JNCB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly MvCJNCB _context;


        public HomeController(ILogger<HomeController> logger, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, MvCJNCB context)
        {
            _logger = logger;
            this.userManager = userManager;
            this.signInManager = signInManager;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.ID = HttpContext.Session.GetString("user");

            TempData["USERID"] = ViewBag.ID;







            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}