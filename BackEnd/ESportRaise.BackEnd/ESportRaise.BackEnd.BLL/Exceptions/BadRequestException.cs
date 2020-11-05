﻿using System.Net;

namespace ESportRaise.BackEnd.BLL.Exceptions
{
    public class BadRequestException : CustomHttpException
    {
        public BadRequestException(string message) 
            : base(message, HttpStatusCode.BadRequest)
        {
        }
    }
}
