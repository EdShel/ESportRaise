using System;
using System.Net;

namespace ESportRaise.BackEnd.BLL.Exceptions
{
    public class CustomHttpException : Exception
    {
        public int StatusCode { get; private set; }

        public CustomHttpException(string message, HttpStatusCode code) : base(message)
        {
            this.StatusCode = (int)code;
        }
    }
}
