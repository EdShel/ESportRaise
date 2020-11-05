using System.Net;

namespace ESportRaise.BackEnd.BLL.Exceptions
{
    public class ForbiddenException : CustomHttpException
    {
        public ForbiddenException(string message)
            : base(message, HttpStatusCode.Forbidden)
        {

        }
    }
}
