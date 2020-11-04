using System.Collections.Generic;
using System.Security.Claims;

namespace ESportRaise.BackEnd.BLL.Interfaces
{
    public interface IAuthTokenFactory
    {
        string GenerateTokenForClaims(IEnumerable<Claim> claims);
    }
}
