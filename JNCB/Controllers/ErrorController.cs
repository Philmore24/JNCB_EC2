using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EC2_1701497.Controllers
{
    public class ErrorController : Controller
    {
               
            [Route("Error/{statusCode}")]
            public IActionResult HttpStatusCodeHandler(int statuscode)
           {
                switch(statuscode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Sorry, COuld not find the page you requested ";
                    break;
            }

            return View("Not Found");

            }
           
    }
}
