namespace ESportRaise.BackEnd.DAL.Entities
{
    public class AppUserInfo
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string UserRole { get; set; }

        public int TeamId { get; set; }

        public string TeamName { get; set; }
    }
}
