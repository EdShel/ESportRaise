namespace ESportRaise.BackEnd.DAL.Entities
{
    public sealed class Coach : AppUser
    {
        public string FullName { get; set; }

        public int TeamId { get; set; }

        public int UserId { get; set; }
    }
}
