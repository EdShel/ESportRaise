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

        public async Task CreateAsync(User user, string password)
        {
            var hashedPassword = passwordHasher.HashPassword(password);
            user.HashedPassword = hashedPassword;
            await base.CreateAsync(user);
        }

        public async Task<User> GetUserOrDefaultByUserNameAsync(string userName)
        {
            var selectCommand = db.CreateCommand();
            selectCommand.CommandText = "SELECT * FROM User WHERE UserName = @userName";
            selectCommand.Parameters.AddWithValue("@userName", userName);
            var reader = await selectCommand.ExecuteReaderAsync();

            if (reader.HasRows)
            {
                return SelectMapper(reader);
            }
            return null;
        }

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

        public async Task<bool> IsAnyUserWithEmailAsync(string email)
        {
            var selectCommand = db.CreateCommand();
            selectCommand.CommandText = "SELECT 1 FROM User WHERE Email = @email";
            selectCommand.Parameters.AddWithValue("@email", email);
            var existsResult = await selectCommand.ExecuteScalarAsync();

            return 1.Equals(existsResult);
        }

        public async Task<bool> IsAnyUserWithUserNameAsync(string userName)
        {
            var selectCommand = db.CreateCommand();
            selectCommand.CommandText = "SELECT 1 FROM User WHERE Email = @userName";
            selectCommand.Parameters.AddWithValue("@userName", userName);
            var existsResult = await selectCommand.ExecuteScalarAsync();

            return 1.Equals(existsResult);
        }

        public bool IsUserPasswordCorrect(User user, string password)
        {
            return passwordHasher.VerifyPassword(user.HashedPassword, password);
        }

        public async Task CreateRefreshTokenAsync(User user, string refreshToken)
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

        public async Task<bool> HasRefreshToken(User user, string refreshToken)
        {
            var selectCommand = db.CreateCommand();
            selectCommand.CommandText = "SELECT 1 FROM RefreshToken WHERE UserId = @userId AND Token = @token";
            selectCommand.Parameters.AddWithValue("@userId", user.Id);
            selectCommand.Parameters.AddWithValue("@token", refreshToken);
            var searchResult = await selectCommand.ExecuteScalarAsync();

            return 1.Equals(searchResult);
        }

        public async Task DeleteRefreshTokenAsync(User user, string refreshToken)
        {
            var deleteCommand = db.CreateCommand();
            deleteCommand.CommandText = "DELETE FROM RefreshToken WHERE UserId = @userId AND Token = @token";
            deleteCommand.Parameters.AddWithValue("@userId", user.Id);
            deleteCommand.Parameters.AddWithValue("@token", refreshToken);
            await deleteCommand.ExecuteNonQueryAsync();
        }
    }
}
