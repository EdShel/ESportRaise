using ESportRaise.BackEnd.API.Models.Auth;
using ESportRaise.BackEnd.BLL.Constants;
using ESportRaise.BackEnd.BLL.DTOs.AppUser;
using ESportRaise.BackEnd.BLL.DTOs.Auth;
using ESportRaise.BackEnd.BLL.Exceptions;
using ESportRaise.BackEnd.BLL.Interfaces;
using ESportRaise.BackEnd.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.API.Controllers
{
    [Route("[controller]"), ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthAsyncService authService;

        private readonly AppUserService appUserService;

        public AuthController(IAuthAsyncService authService, AppUserService appUserService)
        {
            this.authService = authService;
            this.appUserService = appUserService;
        }


        [HttpPost("register")]
        public async Task<ActionResult> RegisterAsync([FromBody] RegisterDTO request)
        {
            if (request.Role == AuthConstants.ADMIN_ROLE)
            {
                bool userIsAdmin = User.IsInRole(AuthConstants.ADMIN_ROLE);
                bool adminsAreRegistered = await appUserService.DoesAnyAdminExistAsync();
                if (!userIsAdmin && adminsAreRegistered)
                {
                    throw new ForbiddenException("You need to be an admin to register admins!");
                }
            }
            await authService.RegisterAsync(request);

            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult> LoginAsync([FromBody] LoginRequestDTO request)
        {
            LoginResponseDTO response = await authService.LoginAsync(request);

            return Ok(response);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult> RefreshTokenAsync([FromBody] TokenRefreshAPIRequest request)
        {
            var requestDTO = new TokenRefreshRequestDTO
            {
                RefreshToken = request.RefreshToken,
                UserName = User.Identity.Name
            };
            TokenRefreshResponseDTO response = await authService.RefreshTokenAsync(requestDTO);

            return Ok(response);
        }

        [HttpPost("revoke"), Authorize]
        public async Task<ActionResult> RevokeTokenAsync([FromBody] TokenRevokeAPIRequest request)
        {
            var requestDTO = new TokenRevokeDTO
            {
                RefreshToken = request.RefreshToken,
                UserName = User.Identity.Name
            };
            await authService.RevokeTokenAsync(requestDTO);

            return Ok();
        }

        [HttpGet("me"), Authorize]
        public async Task<IActionResult> GetInfoAboutCurrentUserAsync()
        {
            AppUserDTO currentUser = await appUserService.GetUserAsync(User.Identity.Name);
            return new JsonResult(new
            {
                Id = currentUser.Id,
                Name = currentUser.UserName,
                Email = currentUser.Email
            });
        }
    }
}
