using System.Collections.Generic;

namespace ESportRaise.BackEnd.BLL.DTOs.Auth
{
    public sealed class TokenServiceRevokeResponse : ErrorProneOperationResponse
    {
        public TokenServiceRevokeResponse(string error)
             : base(new[] { new OperationError(error) })
        {

        }

        public TokenServiceRevokeResponse(IEnumerable<OperationError> errors = null) : base(errors)
        {
        }
    }
}
