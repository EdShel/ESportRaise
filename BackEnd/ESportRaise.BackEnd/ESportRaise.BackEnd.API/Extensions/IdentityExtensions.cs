using ESportRaise.BackEnd.BLL.Constants;
using ESportRaise.BackEnd.BLL.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace System.Security.Claims
{
    public static class IdentityExtensions
    {
        public static int GetUserId(this ClaimsPrincipal principal)
        {
            return Convert.ToInt32(principal.Claims
                .First(cl => cl.Type == AuthConstants.ID_CLAIM_TYPE).Value);
        }

        public static async Task<bool> IsAuthorizedToAccessTeamAsync(
            this ClaimsPrincipal principal, 
            int teamId, 
            ITeamMemberService teamMemberService)
        {
            if (principal.IsInRole(AuthConstants.ADMIN_ROLE))
            {
                return true;
            }

            if (await teamMemberService.GetTeamIdAsync(principal.GetUserId()) == teamId)
            {
                return true;
            }

            return false;
        }
    }
}
