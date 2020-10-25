using System;

namespace ESportRaise.BackEnd.DAL.Entities
{
    public class RefreshToken
    {
        public int TokenId { get; set; }

        public int UserId { get; set; }

        public string Token { get; set; }

        public DateTime ExpirationDate { get; set; }

    }
}
