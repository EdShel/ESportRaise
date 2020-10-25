using ESportRaise.BackEnd.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESportRaise.BackEnd.BLL.Services
{
    public class PasswordHasherService : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            throw new NotImplementedException();
        }

        public bool VerifyPassword(string hashedPassword, string password)
        {
            throw new NotImplementedException();
        }
    }
}
