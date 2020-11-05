using System.Net;

namespace ESportRaise.BackEnd.BLL.Exceptions
{
    public class NotFoundException : CustomHttpException
    {
        public NotFoundException(string message) 
            : base(message, HttpStatusCode.NotFound)
        {

        }
    }
}
