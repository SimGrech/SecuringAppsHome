using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ShoppingCart.Application.Interfaces;
using ShoppingCart.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.ActionFilters
{
    public class AddCommentAuthorize: ActionFilterAttribute
    {
        //•	Students can comment on their work BUT input should be handled securely .  Use ActionFilter.

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                CommentViewModel comment = (CommentViewModel)context.ActionArguments["comment"];

                //string decryptedid = Utility.Encryption.SymmetricDecrypt(eid);

                Guid id = Guid.Parse(comment.Submission.Id.ToString());

                var currentLoggedInUser = context.HttpContext.User.Identity.Name;

                //IProductsService prodService = (IProductsService)context.HttpContext.RequestServices.GetService(typeof(IProductsService));
                ITasksService tasksService = (ITasksService)context.HttpContext.RequestServices.GetService(typeof(ITasksService));

                bool isUserTeacher = context.HttpContext.User.IsInRole("Teacher");
                var submission = tasksService.GetSubmission(id);
                //var product = prodService.GetProduct(id);
                if ((submission.Owner != currentLoggedInUser) && !isUserTeacher)
                {
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
