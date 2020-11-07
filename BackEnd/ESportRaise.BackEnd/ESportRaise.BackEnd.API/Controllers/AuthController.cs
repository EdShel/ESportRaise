using ESportRaise.BackEnd.API.Models.Auth;
using ESportRaise.BackEnd.BLL.Constants;
using ESportRaise.BackEnd.BLL.DTOs.AppUser;
using ESportRaise.BackEnd.BLL.DTOs.Auth;
using ESportRaise.BackEnd.BLL.Exceptions;
using ESportRaise.BackEnd.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.API.Controllers
{
    [Route("[controller]"), ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthAsyncService authService;

        private readonly IAppUserService appUserService;

        public AuthController(IAuthAsyncService authService, IAppUserService appUserService)
        {
            this.authService = authService;
            this.appUserService = appUserService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterAsync([FromBody] RegisterRequest request)
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
            var registerDTO = new RegisterDTO
            {
                Email = request.Email,
                UserName = request.UserName,
                Password = request.Password,
                Role = request.Role
            };
            await authService.RegisterAsync(registerDTO);

            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            var loginDTO = new LoginRequestDTO
            {
                EmailOrUserName = request.EmailOrUserName,
                Password = request.Password
            };
            LoginResponseDTO response = await authService.LoginAsync(loginDTO);

            return Ok(response);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult> RefreshTokenAsync([FromBody] TokenRefreshRequest request)
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
        public async Task<ActionResult> RevokeTokenAsync([FromBody] TokenRevokeRequest request)
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

        [HttpGet("user"), Authorize]
        public async Task<IActionResult> GetInfoAboutAnotherUserAsync(int id)
        {
            AppUserDTO user = await appUserService.GetUserAsync(id);
            return new JsonResult(new
            {
                Id = user.Id,
                Name = user.UserName,
                Email = user.Email
            });
        }
    }
}
