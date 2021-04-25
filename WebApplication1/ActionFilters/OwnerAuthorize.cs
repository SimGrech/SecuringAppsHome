using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ShoppingCart.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.ActionFilters
{
    public class OwnerAuthorize: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var id = new Guid(context.ActionArguments["id"].ToString());

                var currentLoggedInUser = context.HttpContext.User.Identity.Name;

                IProductsService prodService = (IProductsService)context.HttpContext.RequestServices.GetService(typeof(IProductsService));
                var product = prodService.GetProduct(id);
                if(product.Owner != currentLoggedInUser)
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
