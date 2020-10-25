namespace ESportRaise.BackEnd.DAL.Interfaces
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);

        bool VerifyPassword(string hashedPassword, string password);
    }
}
