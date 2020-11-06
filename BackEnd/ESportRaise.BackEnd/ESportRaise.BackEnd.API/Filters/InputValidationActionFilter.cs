using ESportRaise.BackEnd.BLL.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ESportRaise.BackEnd.API.Filters
{
    public class InputValidationActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                throw new BadRequestException("Invalid request!");
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
