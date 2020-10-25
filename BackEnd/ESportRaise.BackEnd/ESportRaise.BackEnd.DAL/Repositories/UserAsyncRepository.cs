using ESportRaise.BackEnd.DAL.Entities;
using ESportRaise.BackEnd.DAL.Interfaces;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.DAL.Repositories
{
    // TOOD: create table for users
    public class UserAsyncRepository : BasicAsyncRepository<User>
    {
        private IPasswordHasher passwordHasher;

        public UserAsyncRepository(SqlConnection databaseConnection, IPasswordHasher passwordHasher)
            : base(databaseConnection)
        {
            this.passwordHasher = passwordHasher;
        }

        protected override Func<SqlDataReader, User> SelectMapper
        {
            get => r => new User();
        }

        protected override Func<User, object[]> InsertValues { get; }

        protected override Func<User, TablePropertyValuePair[]> UpdatePropertiesAndValuesExtractor { get; }

        protected override TablePropertyExtractor UpdatePredicatePropertyEqualsValue { get; }

        public async Task<User> GetUserOrDefaultByEmailOrUserNameAsync(string emailOrUserName)
        {
            var selectCommand = db.CreateCommand();
            selectCommand.CommandText = "SELECT * FROM User WHERE Email = @emailOrUserName OR UserName = @emailOrUserName";
            selectCommand.Parameters.AddWithValue("@emailOrUserName", emailOrUserName);
            var reader = await selectCommand.ExecuteReaderAsync();

            if (reader.HasRows)
            {
                return SelectMapper(reader);
            }
            return null;
        }

        public bool IsUserPasswordCorrect(User user, string password)
        {
            return passwordHasher.VerifyPassword(user.HashedPassword, password);
        }
    }
}
