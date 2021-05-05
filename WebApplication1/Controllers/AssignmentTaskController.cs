using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShoppingCart.Application.Interfaces;
using ShoppingCart.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AssignmentTaskController : Controller
    {
        private readonly ITasksService _taskService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _host;
        private readonly ILogger<ProductsController> _logger;
        public AssignmentTaskController(ITasksService taskService, IWebHostEnvironment host, ILogger<ProductsController> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _host = host;
            _taskService = taskService;
            _userManager = userManager;
        }


        [Authorize]
        public async Task<IActionResult> IndexAsync()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

            var list = (dynamic)null;

            //If user is a teacher, retrieve tasks created by them
            if (currentUser.Teacher == null)
            {
                list = _taskService.GetTasks(currentUser.Email);
            }
            //If user is a student, retrieve tasks created by their teacher
            else
            {
                list = _taskService.GetTasks(currentUser.Teacher);
            }


            return View(list);
        }

        [Authorize(Roles = "Teacher")]
        public IActionResult CreateTask() {
            
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public IActionResult CreateTask(AssignmentTaskViewModel task)
        {
            _taskService.AddTask(task);
            TempData["message"] = "Task was added Successfully";

            return RedirectToAction("Index", "AssignmentTask");
        }
    }
}
