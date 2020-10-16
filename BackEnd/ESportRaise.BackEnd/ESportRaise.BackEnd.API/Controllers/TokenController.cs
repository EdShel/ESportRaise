using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using ESportRaise.BackEnd.API.CustomServices;
using Microsoft.IdentityModel.Tokens;

namespace ESportRaise.BackEnd.API.Controllers
{
    public class AuthenticationModel
    {
        public string Login { set; get; }

        public string Password { set; get; }
    }
    [Route("[controller]"), ApiController]
    public class TokenController : ControllerBase
    {
        [Authorize(Roles = "coach")]
        [HttpGet]
        public string Check()
        {
            return "You're allowed to see it";
        }

        [HttpPost]
        public IActionResult GetToken(string username, string password)
        {
            var identity = GetIdentity(username, password);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid username or password." });
            }

            var tokenOptions = new AuthenticationOptions();
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                    issuer: tokenOptions.ValidIssuer,
                    audience: tokenOptions.ValidAudience,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(tokenOptions.TokenLifetimeMinutes)),
                    signingCredentials: new SigningCredentials(
                        tokenOptions.IssuerSigningKey, 
                        SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };

            return new JsonResult(response);
        }

        private ClaimsIdentity GetIdentity(string username, string password)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, "ivan.petrosyan@gmail.com"),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, "coach")
            };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                claims, 
                "Token", 
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }
    }
}
