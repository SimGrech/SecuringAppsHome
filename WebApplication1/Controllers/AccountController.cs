using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Utility;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        { 
            _userManager = userManager;
            _roleManager = roleManager;
        }


        [Authorize(Roles = "Teacher")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> IndexAsync(NewStudentModel newUser) {

            //Set Teacher when creating user
            //newUser.Teacher = User.Identity.Name;

            AsymmetricKeys keys = Encryption.GenerateAsymmetricKeys();
            //Set when creating new user
            //newUser.PublicKey = keys.PublicKey;
            //newUser.PrivateKey = keys.PrivateKey;

            //https://seanmccammon.com/random-password-generator-in-c/ (Class created in utility folder)
            string password = PasswordGenerator.GeneratePassword(true, true, true, true, 12);

            var user = new ApplicationUser { UserName = newUser.Email, Email = newUser.Email, Teacher = User.Identity.Name, PublicKey = keys.PublicKey, PrivateKey = keys.PrivateKey };

            var result = await _userManager.CreateAsync(user, password);

            //User Created Successfully, allocate role
            if (result.Succeeded)
            {
                TempData["message"] = "User created";
                var userResult = await _userManager.AddToRoleAsync(user, "Student");

                if (userResult != null)
                {
                    //https://www.youtube.com/watch?v=C4O8vqg295o&ab_channel=ASP.NETMVC
                    //Add code to send email with email and password
                    //simsecuringhome@gmail.com
                    //myVerySecurePassword123!
                }
                else {
                    TempData["error"] = "Something went wrong.";
                }

            }
            else {
                TempData["error"] = "Something went wrong.";
            }


            return View();
        }


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
