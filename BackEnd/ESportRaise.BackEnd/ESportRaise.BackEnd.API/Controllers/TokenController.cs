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

namespace ESportRaise.BackEnd.API.Controllers
{
    [Route("[controller]"), ApiController]
    public class TokenController : ControllerBase
    {
        private SqlConnection db;


        [Authorize(Roles = "Coach")]
        [HttpGet]
        public string Check()
        {
            return "You're allowed to see it";
        }
    }
}
