namespace ESportRaise.BackEnd.DAL.Entities
{
    public class AppUser
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string HashedPassword { get; set; }

        public string UserRole { get; set; }

    }
}
