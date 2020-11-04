using ESportRaise.BackEnd.BLL.Exceptions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
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

                var errorObject = new
                {
                    Code = context.Response.StatusCode,
                    Message = exception.Message
                };
                string jsonError = JsonConvert.SerializeObject(errorObject);

                await context.Response.Body.WriteAsync(
                    Encoding.UTF8.GetBytes(jsonError));
            }
        }
    }
}
