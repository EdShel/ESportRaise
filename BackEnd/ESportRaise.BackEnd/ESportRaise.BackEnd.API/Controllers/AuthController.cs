using ESportRaise.BackEnd.API.Models.Auth;
using ESportRaise.BackEnd.BLL.DTOs.AppUser;
using ESportRaise.BackEnd.BLL.DTOs.Auth;
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


        // TODO: allow Admin registration only by admins
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDTO request)
        {
            await authService.RegisterAsync(request);

            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequestDTO request)
        {
            LoginResponseDTO response = await authService.LoginAsync(request);

            return Ok(response);
        }

        // TODO: check whether User.Identity is accessible after the token has expired
        [HttpPost("refresh")]
        public async Task<ActionResult> RefreshToken([FromBody] TokenRefreshAPIRequest request)
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
        public async Task<ActionResult> RevokeToken([FromBody] TokenRevokeAPIRequest request)
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
        public async Task<IActionResult> GetInfoAboutCurrentUser()
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
