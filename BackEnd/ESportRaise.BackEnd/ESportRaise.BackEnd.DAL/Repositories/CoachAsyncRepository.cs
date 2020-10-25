using ESportRaise.BackEnd.DAL.Entities;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.DAL.Repositories
{
    // TOOD: create table for users
    public class UserAsyncRepository : BasicAsyncRepository<User>
    {
        public UserAsyncRepository(SqlConnection databaseConnection)
            : base(databaseConnection)
        {
        }

        protected override Func<SqlDataReader, User> SelectMapper {
            get => r => new User();
        }

        protected override Func<User, object[]> InsertValues { get; }

        protected override Func<User, TablePropertyValuePair[]> UpdatePropertiesAndValuesExtractor { get; }

        protected override TablePropertyExtractor UpdatePredicatePropertyEqualsValue { get; }

        public async Task<User> GetUserOrDefaultByEmailAsync(string email)
        {
            var selectCommand = db.CreateCommand();
            selectCommand.CommandText = "SELECT * FROM User WHERE Email = @email";
            selectCommand.Parameters.AddWithValue("@email", email);
            var reader = await selectCommand.ExecuteReaderAsync();

            if (reader.HasRows)
            {
                return SelectMapper(reader);
            }
            return null;
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

        public bool IsPasswordCorrect(string hashedPassword, string password)
        {
            return false;
        }

        private static string HashPassword(string password)
        {
            return password;
        }
    }
}
