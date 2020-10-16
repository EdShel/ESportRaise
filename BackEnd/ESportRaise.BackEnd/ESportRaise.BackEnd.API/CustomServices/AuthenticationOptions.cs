using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.API.CustomServices
{
    public class AuthenticationOptions : TokenValidationParameters
    {
        private string TokenIssuer => "ESportRaiseBackEnd";

        private string TokenClient => "ESportRaiseClient";

        private string CypherKey => "extremelyStrongKeyHere";

        public int TokenLifetimeMinutes => 1;

        public AuthenticationOptions() : base()
        {
            ValidateIssuer = true;
            ValidIssuer = TokenIssuer;

            ValidateAudience = true;
            ValidAudience = TokenClient;

            ValidateLifetime = true;
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(CypherKey));

            ValidateIssuerSigningKey = true;
        }
    }
}
