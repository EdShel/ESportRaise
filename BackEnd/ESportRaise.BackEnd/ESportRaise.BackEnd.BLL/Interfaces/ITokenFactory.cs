using System.Collections.Generic;
using System.Security.Claims;

namespace ESportRaise.BackEnd.BLL.Interfaces
{
    public interface ITokenFactory
    {
        string GenerateTokenForClaims(IEnumerable<Claim> claims);
    }

    public interface IRefreshTokenFactory
    {
        string GenerateToken();
    }
}
