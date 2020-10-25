using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using ESportRaise.BackEnd.BLL.DTOs.Tokens;
using System.Data.SqlClient;
using ESportRaise.BackEnd.BLL.Interfaces;

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
        public async Task<ActionResult> Register([FromBody] RegisterRequest request)
        {
            RegisterResponse response = await authService.RegisterAsync(request);
            if (!response.Success)
            {
                return BadRequest(response.Errors);
            }

            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            LoginResponse response = await authService.LoginAsync(request);
            if (!response.Success)
            {
                return BadRequest(response.Errors);
            }

            return Ok(response);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult> RefreshToken([FromBody] TokenRefreshRequest request)
        {
            TokenRefreshResponse response = await authService.RefreshTokenAsync(request);
            if (!response.Success)
            {
                return BadRequest(response.Errors);
            }

            return Ok(response);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult> RevokeToken([FromBody] TokenRevokeRequest request)
        {
            TokenRevokeResponse response = await authService.RevokeTokenAsync(request);
            if (!response.Success)
            {
                return BadRequest(response.Errors);
            }

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
