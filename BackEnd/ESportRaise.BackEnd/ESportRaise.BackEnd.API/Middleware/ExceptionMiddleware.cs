using ESportRaise.BackEnd.BLL.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.API.Middleware
{
    public sealed class ExceptionMiddleware
    {
        private readonly RequestDelegate next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (Exception exception)
            {
                context.Response.StatusCode =
                    (exception as CustomHttpException)?.StatusCode ?? 500;

                context.Response.ContentType = "application/json";

                await context.Response.Body.WriteAsync(
                    Encoding.UTF8.GetBytes(exception.Message));
            }
        }
    }
}
