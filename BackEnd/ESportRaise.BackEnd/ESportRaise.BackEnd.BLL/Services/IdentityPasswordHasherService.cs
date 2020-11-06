using ESportRaise.BackEnd.DAL.Entities;
using ESportRaise.BackEnd.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace ESportRaise.BackEnd.BLL.Services
{
    public class IdentityPasswordHasherService : IPasswordHasher
    {
        private PasswordHasher<AppUser> passwordHasher;

        public IdentityPasswordHasherService()
        {
            passwordHasher = new PasswordHasher<AppUser>();
        }

        public string HashPassword(string password)
        {
            return passwordHasher.HashPassword(null, password);
        }

        public bool VerifyPassword(string hashedPassword, string password)
        {
            return passwordHasher.VerifyHashedPassword(null, hashedPassword, password) == PasswordVerificationResult.Success;
        }
    }
}
