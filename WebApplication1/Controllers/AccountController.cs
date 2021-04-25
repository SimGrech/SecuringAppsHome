using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        
        public AccountController(UserManager<ApplicationUser> userManager)
        { _userManager = userManager; }
        
        /*
        public async Task<IActionResult> GenerateAccount(StudentCreationModel model)
        {


            var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName };
            var result = await _userManager.CreateAsync(user, model.Password);



            return View();
        }
        */
    }
}
