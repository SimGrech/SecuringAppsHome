using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Utility;
using System.Net.Mail;
using MimeKit;
using MailKit.Net.Smtp;
using System.Text.Encodings.Web;

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
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> IndexAsync(NewStudentModel newUser) {

            //Set Teacher when creating user
            //newUser.Teacher = User.Identity.Name;

            AsymmetricKeys keys = Encryption.GenerateAsymmetricKeys();
            //Set when creating new user
            //newUser.PublicKey = keys.PublicKey;
            //newUser.PrivateKey = keys.PrivateKey;
            newUser.FirstName = HtmlEncoder.Default.Encode(newUser.FirstName);
            newUser.LastName = HtmlEncoder.Default.Encode(newUser.FirstName);

            //https://seanmccammon.com/random-password-generator-in-c/ (Class created in utility folder)
            string password = PasswordGenerator.GeneratePassword(true, true, true, true, 12);

            var user = new ApplicationUser { UserName = newUser.Email, Email = newUser.Email, Teacher = User.Identity.Name, PublicKey = keys.PublicKey, PrivateKey = keys.PrivateKey };

            var checkUser = await _userManager.FindByNameAsync(newUser.Email);
            if (checkUser == null)
            {

                var result = await _userManager.CreateAsync(user, password);

                //User Created Successfully, allocate role
                if (result.Succeeded)
                {
                    TempData["message"] = "User created";
                    var userResult = await _userManager.AddToRoleAsync(user, "Student");

                    if (userResult != null)
                    {
                        string emailTo = user.Email;
                        string emailSubject = "Securing Apps Assignment Password";
                        string emailBody = "Your credentials are \nEmail: " + user.Email + "\nPassword: " + password;
                        //https://www.youtube.com/watch?v=6syBcnXvSrE&ab_channel=HarithaComputers%26Technology
                        //Add code to send email with email and password
                        //simsecuringhome@gmail.com
                        //myVerySecurePassword123!
                        /*

                        MailMessage mailMessage = new MailMessage("simsecuringhome@gmail.com", emailTo, emailSubject, emailBody);
                        mailMessage.IsBodyHtml = false;
                        SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                        smtp.Port = 587;
                        smtp.UseDefaultCredentials = true;
                        smtp.EnableSsl = true;
                        smtp.Credentials = new System.Net.NetworkCredential("simsecuringhome@gmail.com", "myVerySecurePassword123!");
                        smtp.Send(mailMessage);
                        */

                        
                        var message = new MimeMessage();
                        message.From.Add(new MailboxAddress("Securing Home App", "simsecuringhome@gmail.com"));
                        message.To.Add(new MailboxAddress(user.FirstName, emailTo));
                        message.Subject = emailSubject;
                        message.Body = new TextPart("plain")
                        {
                            Text = emailBody
                        };
                        using (var client = new MailKit.Net.Smtp.SmtpClient())
                        {
                            client.Connect("smtp.gmail.com", 587, false);
                            client.Authenticate("simsecuringhome@gmail.com", "myVerySecurePassword123!");
                            client.Send(message);

                            client.Disconnect(true);
                        }
                    }
                    else
                    {
                        TempData["error"] = "Something went wrong.";
                    }
                }
                else
                {
                    TempData["error"] = "Something went wrong.";
                }
            }
            else {
                TempData["error"] = "User already exists.";
            }

            return View();
        }
    }
}
