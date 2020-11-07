using ESportRaise.BackEnd.BLL.Interfaces;
using System;
using System.Security.Cryptography;

namespace ESportRaise.BackEnd.BLL.Services
{
    public class RefreshTokenFactory : IRefreshTokenFactory
    {
        public string GenerateToken()
        {
            const int tokenLengthInBytes = 32;
            var randomNumber = new byte[tokenLengthInBytes];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
