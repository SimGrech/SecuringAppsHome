using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShoppingCart.Application.Interfaces;
using ShoppingCart.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using WebApplication1.ActionFilters;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AssignmentTaskController : Controller
    {
        private readonly ITasksService _taskService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _host;
        private readonly ILogger<AssignmentTaskController> _logger;
        public AssignmentTaskController(ITasksService taskService, IWebHostEnvironment host, ILogger<AssignmentTaskController> logger, UserManager<ApplicationUser> userManager)
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
            //If user is a student, retrieve tasks created by their teacher and retrieve their submissions
            else
            {

                list = _taskService.GetTasks(currentUser.Teacher);
                IQueryable<SubmissionViewModel> userSubmissions = _taskService.GetUserSubmissions(User.Identity.Name);
                var taskIds = new List<Guid>();


                foreach (var subm in userSubmissions) {
                    //subm.AssignmentTask.Id
                    taskIds.Add(subm.AssignmentTask.Id);
                }

                ViewBag.userSubmittedTaskIds = taskIds;
            }


            return View(list);
        }

        [Authorize(Roles = "Teacher")]
        public IActionResult CreateTask()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public IActionResult CreateTask(AssignmentTaskViewModel task)
        {
            task.Description = HtmlEncoder.Default.Encode(task.Description);
            task.Title = HtmlEncoder.Default.Encode(task.Title);
            //Teacher Email being set from hidden field (Encoding just in case)
            task.Teacher = HtmlEncoder.Default.Encode(task.Teacher);

            if (task.Deadline > DateTime.Now)
            {
                _taskService.AddTask(task);
                TempData["message"] = "Task was added Successfully";
                return RedirectToAction("Index", "AssignmentTask");
            }
            else {
                TempData["error"] = "Deadline must be in the future";
            }

            return View();
            
        }


        [HttpGet]
        [Authorize(Roles = "Student")]
        public IActionResult UploadSubmission(string eid)
        {
            //Decrypt ID
            string decryptedid = Utility.Encryption.SymmetricDecrypt(eid);

            Guid id = Guid.Parse(decryptedid);

            var thisTask = _taskService.GetTask(id);


            ViewBag.TaskId = thisTask.Id;

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> UploadSubmissionAsync(SubmissionViewModel submission, IFormFile file)
        {
            Guid guid = submission.AssignmentTask.Id;
            submission.AssignmentTask = _taskService.GetTask(guid);
            submission.Owner = User.Identity.Name;
            submission.TimeSubmitted = DateTime.Now;
            submission.FileName = HtmlEncoder.Default.Encode(submission.FileName);
            submission.Description = HtmlEncoder.Default.Encode(submission.Description);

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user != null)
            {

                string uniqueFilename;
                if (System.IO.Path.GetExtension(file.FileName) == ".pdf" && file.Length < 1048576 && submission.FileName != null && submission.Description != null && submission.AssignmentTask.Deadline > DateTime.Now)
                {
                    //25 50 >>>>> 37 80
                    byte[] whitelist = new byte[] { 37, 80 };

                    if (file != null)
                    {
                        MemoryStream userFile = new MemoryStream();
                        file.CopyTo(userFile);

                        using (var f = file.OpenReadStream())
                        {
                            /*int byte1 = f.ReadByte();
                            int byte2 = f.ReadByte();
                            */
                            byte[] buffer = new byte[2];  //how to read an x amount of bytes at 1 go
                            f.Read(buffer, 0, 2); //offset - how many bytes you would lke the pointer to skip

                            for (int i = 0; i < whitelist.Length; i++)
                            {
                                if (whitelist[i] == buffer[i])
                                {

                                }
                                else
                                {
                                    //the file is not acceptable
                                    TempData["error"] = "File type is not acceptable";
                                    //ModelState.AddModelError("file", "File is not valid and acceptable");
                                    return View();
                                }
                            }
                            //...other reading of bytes happening
                            f.Position = 0;

                            //uploading the file
                            //correctness
                            uniqueFilename = Guid.NewGuid() + Path.GetExtension(file.FileName);
                            submission.Path = uniqueFilename;

                           
                            submission.Signature = Utility.Encryption.SignData(userFile, user.PrivateKey);

                            byte[] toHash = userFile.ToArray();
                            byte[] hash = Utility.Encryption.Hash(toHash);
                            submission.Hash = Convert.ToBase64String(hash);

                            //string absolutePath1 = _host.WebRootPath + @"\pictures\" + uniqueFilename;
                            //"D:\\School\\Level 6 Year 2\\Semester 2\\Securing Applications\\Assignment\\RyanCodeUpdated\\SecuringAppsSWD62a-main\\WebApplication1\\wwwroot\\pictures\\cc2aa32b-0b64-4e08-a695-6b26a89a2fe6.pdf"
                            string absolutePath = _host.ContentRootPath + @"\ValuableFiles\" + uniqueFilename;
                            try
                            {
                                using (FileStream fsOut = new FileStream(absolutePath, FileMode.CreateNew, FileAccess.Write))
                                {
                                    // throw new Exception();
                                    f.CopyTo(fsOut);
                                }
                                //   f.CopyTo(userFile); //this goes instead writing the file into a folder
                                f.Close();
                                //File upload successful add to database

                                _taskService.AddSubmission(submission);
                            }
                            catch (Exception ex)
                            {
                                //log
                                _logger.LogError(ex, "Error happend while saving file");

                                return View("Error", new ErrorViewModel() { Message = "Error while saving the file. Try again later" });
                            }
                        }
                        TempData["message"] = "Assignment Successfully Submitted";
                        return RedirectToAction("ViewUserSubmissions", "AssignmentTask");

                    }
                }
                else
                {
                    TempData["message"] = "File must be a pdf and must include the file name and file description fields or deadline has elapsed";
                }

            }
            else {
                TempData["error"] = "An error has occured"; //User account error
            }

            return View();
        }

        [Authorize(Roles = "Student")]
        public IActionResult ViewUserSubmissions() {

            var list = _taskService.GetUserSubmissions(User.Identity.Name);

            return View(list);
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public IActionResult ViewTaskSubmissions(string eid) {
            //Decrypt ID
            string decryptedid = Utility.Encryption.SymmetricDecrypt(eid);

            Guid id = Guid.Parse(decryptedid);

            var list = _taskService.GetSubmissions(id);

            return View(list);
        }

        [Authorize(Roles = "Student, Teacher")]
        [ViewSubmissionAuthorize]
        public IActionResult ViewSubmission(string eid) {
            //Decrypt ID
            string decryptedid = Utility.Encryption.SymmetricDecrypt(eid);

            Guid id = Guid.Parse(decryptedid);

            var userSubmission = _taskService.GetSubmission(id);

            var comments = _taskService.GetSubmissionComments(id);

            bool assignmentCopied = _taskService.SubmissionCopied(userSubmission.Hash);


            ViewBag.SubmissionComments = comments;
            ViewBag.Copied = assignmentCopied;

            return View(userSubmission);
        }

        //Submission id parameter
        [Authorize(Roles = "Student, Teacher")]
        [HttpGet]
        [ValidateAntiForgeryToken]
        [ViewSubmissionAuthorize]
        //Implement action filter to see if student has access to this
        public IActionResult AddComment(string eid) {
            //Decrypt ID
            string decryptedid = Utility.Encryption.SymmetricDecrypt(eid);

            Guid id = Guid.Parse(decryptedid);


            //Submission id to be passed on as a hidden field
            ViewBag.SubmissionId = id;

            return View();
        }

        [Authorize(Roles = "Student, Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AddCommentAuthorize]
        //Implement action filter to see if student has access to this
        public IActionResult AddComment(CommentViewModel comment) {
            if (comment.Content != null)
            {
                comment.Content = HtmlEncoder.Default.Encode(comment.Content);
                comment.Author = User.Identity.Name;
                comment.Posted = DateTime.Now;
                _taskService.AddComment(comment);
                TempData["message"] = "Comment Added successfully";
                string encryptedId = Utility.Encryption.SymmetricEncrypt(comment.Submission.Id.ToString());
                return RedirectToAction("ViewSubmission", "AssignmentTask", new { eid = encryptedId });

            }
            else {
                TempData["error"] = "Comment Content must not be empty";
            }
            
            return View();
        }

        [Authorize(Roles = "Student, Teacher")]
        [HttpGet]
        [ViewSubmissionAuthorize]
        public async Task<IActionResult> DownloadFile(string eid) {
            //Decrypt ID
            string decryptedid = Utility.Encryption.SymmetricDecrypt(eid);

            Guid id = Guid.Parse(decryptedid);

            SubmissionViewModel submission = _taskService.GetSubmission(id);

            var submissionOwner = await _userManager.FindByNameAsync(submission.Owner);

            if (submissionOwner != null)
            {
                string path = _host.ContentRootPath + @"\ValuableFiles\" + submission.Path;

                byte[] bytes = System.IO.File.ReadAllBytes(path);

                MemoryStream memoryStream = new MemoryStream(bytes);

                bool isAuthentic = Utility.Encryption.VerifyData(memoryStream, submissionOwner.PublicKey, submission.Signature);

                if (isAuthentic)
                {
                    return (File(memoryStream, "application/octet-stream", submission.Path));
                }
                else
                {
                    TempData["error"] = "Submitted File is not authentic";
                }

            }
            else
            {
                TempData["error"] = "An error has occured";
            }

            return RedirectToAction("ViewSubmission", "AssignmentTask", new { eid = eid });

        }

    }
}
