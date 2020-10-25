using System;
using System.Collections.Generic;
using System.Text;

namespace ESportRaise.BackEnd.DAL.Entities
{
    public sealed class Coach : User
    {
        public string FullName { get; set; }

        public int TeamId { get; set; }

        public int UserId { get; set; }
    }

    public class User
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string HashedPassword { get; set; }

        public string Role { get; set; }

    }

    public class RefreshToken
    {
        public int UserId { get; set; }

        public string IP { get; set; }

        public string Token { get; set; }

        public DateTime ExpirationDate { get; set; }

    }
}
