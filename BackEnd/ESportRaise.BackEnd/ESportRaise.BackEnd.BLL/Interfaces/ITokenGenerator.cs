using System.Collections.Generic;
using System.Security.Claims;

namespace ESportRaise.BackEnd.BLL.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateTokenForClaims(IEnumerable<Claim>);
    }
}
