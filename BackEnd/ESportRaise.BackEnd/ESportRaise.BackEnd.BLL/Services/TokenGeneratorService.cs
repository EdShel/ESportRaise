using ESportRaise.BackEnd.BLL.Interfaces;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace ESportRaise.BackEnd.BLL.Services
{
    public class JWTTokenGeneratorService : ITokenGenerator
    {
        public string GenerateTokenForClaims(IEnumerable<Claim> claims)
        {
            throw new System.NotImplementedException();
        }
    }
}
