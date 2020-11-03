using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

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

    public class BadRequestException : CustomHttpException
    {
        public BadRequestException(string message) 
            : base(message, HttpStatusCode.BadRequest)
        {
        }
    }

    public class NotFoundException : CustomHttpException
    {
        public NotFoundException(string message) 
            : base(message, HttpStatusCode.NotFound)
        {

        }
    }
}
