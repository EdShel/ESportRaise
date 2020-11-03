using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using ESportRaise.BackEnd.BLL.DTOs.Auth;
using System.Data.SqlClient;
using ESportRaise.BackEnd.BLL.Interfaces;
using ESportRaise.BackEnd.API.Models.Auth;

namespace ESportRaise.BackEnd.API.Controllers
{
    [Route("[controller]"), ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthAsyncService authService;

        public AuthController(IAuthAsyncService authService)
        {
            this.authService = authService;
        }

        // TODO: allow Admin registration only by admins
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterServiceRequest request)
        {
            RegisterServiceResponse response = await authService.RegisterAsync(request);

            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginServiceRequest request)
        {
            LoginServiceResponse response = await authService.LoginAsync(request);

            return Ok(response);
        }

        // TODO: check whether User.Identity is accessible after the token has expired
        [HttpPost("refresh")]
        public async Task<ActionResult> RefreshToken([FromBody] TokenRefreshAPIRequest request)
        {
            var requestDTO = new TokenServiceRefreshRequest
            {
                RefreshToken = request.RefreshToken,
                UserName = User.Identity.Name
            };
            TokenServiceRefreshResponse response = await authService.RefreshTokenAsync(requestDTO);

            return Ok(response);
        }

        [HttpPost("revoke"), Authorize]
        public async Task<ActionResult> RevokeToken([FromBody] TokenRevokeAPIRequest request)
        {
            var requestDTO = new TokenServiceRevokeRequest
            {
                RefreshToken = request.RefreshToken,
                UserName = User.Identity.Name
            };
            TokenServiceRevokeResponse response = await authService.RevokeTokenAsync(requestDTO);

            return Ok();
        }

        [Authorize]
        [HttpGet]
        public string Check()
        {
            return "You're allowed to see it";
        }
    }
}
