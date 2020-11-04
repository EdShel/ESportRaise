using ESportRaise.BackEnd.BLL.Constants;
using System;
using System.Linq;
using System.Security.Claims;

namespace System.Security.Claims
{
    public static class IdentityExtensions
    {
        public static int GetUserId(this ClaimsPrincipal principal)
        {
            return Convert.ToInt32(principal.Claims
                .First(cl => cl.Type == AuthConstants.ID_CLAIM_TYPE).Value);
        }
    }
}
