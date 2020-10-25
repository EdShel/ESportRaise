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

        public async void CreateRefreshTokenAsync(User user, string refreshToken)
        {
            var tokenExpirationDate = DateTime.Now.AddDays(7);
            var insertCommand = db.CreateCommand();
            insertCommand.CommandText =
                "INSERT INTO RefreshToken(UserId, Token, ExpirationDate) " +
                "VALUES(@userId, @token, @expirationDate)";
            insertCommand.Parameters.AddWithValue("@userId", user.Id);
            insertCommand.Parameters.AddWithValue("@token", refreshToken);
            insertCommand.Parameters.AddWithValue("@expirationDate", tokenExpirationDate);
            await insertCommand.ExecuteNonQueryAsync();
        }
    }
}
