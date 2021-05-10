using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ShoppingCart.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WebApplication1.Controllers;
using Microsoft.AspNetCore.Http;

namespace WebApplication1.ActionFilters
{
    public class ViewSubmissionAuthorize: ActionFilterAttribute
    {
        //•	Students can comment on their work BUT input should be handled securely .  Use ActionFilter.

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var eid = context.ActionArguments["eid"].ToString();
                string decryptedid = Utility.Encryption.SymmetricDecrypt(eid);

                Guid id = Guid.Parse(decryptedid);

                var currentLoggedInUser = context.HttpContext.User.Identity.Name;

                //IProductsService prodService = (IProductsService)context.HttpContext.RequestServices.GetService(typeof(IProductsService));
                ITasksService tasksService = (ITasksService)context.HttpContext.RequestServices.GetService(typeof(ITasksService));

                ILogger<AssignmentTaskController> logger = (ILogger<AssignmentTaskController>)context.HttpContext.RequestServices.GetService(typeof(ILogger<AssignmentTaskController>));

                bool isUserTeacher = context.HttpContext.User.IsInRole("Teacher");
                var submission = tasksService.GetSubmission(id);
                //var product = prodService.GetProduct(id);
                if ((submission.Owner != currentLoggedInUser) && !isUserTeacher)
                {
                    string logString = $"User {currentLoggedInUser} tried to access file with id {submission.Id}. IP: {context.HttpContext.Connection.RemoteIpAddress.ToString()}, Timestamp: {DateTime.Now}";
                    logger.LogWarning(logString);
                    context.Result = new UnauthorizedObjectResult("Access Denied");
                }
            }
            catch(Exception ex)
            {
                context.Result = new BadRequestObjectResult("Bad Request");
            }
        }
    }
}
