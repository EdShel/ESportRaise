using ESportRaise.BackEnd.DAL.Entities;
using ESportRaise.BackEnd.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.DAL.Repositories
{
    public class AppUserRepository : BasicAsyncRepository<AppUser>
    {
        private IPasswordHasher passwordHasher;

        public AppUserRepository(SqlConnection databaseConnection, IPasswordHasher passwordHasher)
            : base(databaseConnection)
        {
            this.passwordHasher = passwordHasher;
        }

        public async Task CreateAsync(AppUser user, string password)
        {
            var hashedPassword = passwordHasher.HashPassword(password);
            user.HashedPassword = hashedPassword;
            await base.CreateAsync(user);
        }

        public async Task<AppUser> GetUserOrDefaultByUserNameAsync(string userName)
        {
            using (var selectCommand = db.CreateCommand())
            {
                selectCommand.CommandText = "SELECT * FROM AppUser WHERE UserName = @userName";
                selectCommand.Parameters.AddWithValue("@userName", userName);

                using (var reader = await selectCommand.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return MapFromReader(reader);
                    }
                    return null;
                }
            }
        }

        public async Task<AppUser> GetUserOrDefaultByEmailOrUserNameAsync(string emailOrUserName)
        {
            using (var selectCommand = db.CreateCommand())
            {
                selectCommand.CommandText = "SELECT * FROM AppUser WHERE Email = @emailOrUserName OR UserName = @emailOrUserName";
                selectCommand.Parameters.AddWithValue("@emailOrUserName", emailOrUserName);

                using (var reader = await selectCommand.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return MapFromReader(reader);
                    }
                    return null;
                }
            }
        }

        public async Task<bool> IsAnyUserWithEmailAsync(string email)
        {
            using (var selectCommand = db.CreateCommand())
            {
                selectCommand.CommandText = "SELECT 1 FROM AppUser WHERE Email = @email";
                selectCommand.Parameters.AddWithValue("@email", email);
                var existsResult = await selectCommand.ExecuteScalarAsync();

                return 1.Equals(existsResult);
            }
        }

        public async Task<bool> IsAnyUserWithUserNameAsync(string userName)
        {
            using (var selectCommand = db.CreateCommand())
            {
                selectCommand.CommandText = "SELECT 1 FROM AppUser WHERE Email = @userName";
                selectCommand.Parameters.AddWithValue("@userName", userName);
                var existsResult = await selectCommand.ExecuteScalarAsync();

                return 1.Equals(existsResult);
            }
        }

        public bool IsUserPasswordCorrect(AppUser user, string password)
        {
            return passwordHasher.VerifyPassword(user.HashedPassword, password);
        }

        public async Task CreateRefreshTokenAsync(AppUser user, string refreshToken)
        {
            var tokenExpirationDate = DateTime.UtcNow.AddDays(7);
            using (var insertCommand = db.CreateCommand())
            {
                insertCommand.CommandText =
                    "INSERT INTO RefreshToken(UserId, Token, ExpirationDate) " +
                    "VALUES(@userId, @token, @expirationDate)";
                insertCommand.Parameters.AddWithValue("@userId", user.Id);
                insertCommand.Parameters.AddWithValue("@token", refreshToken);
                insertCommand.Parameters.AddWithValue("@expirationDate", tokenExpirationDate);
                await insertCommand.ExecuteNonQueryAsync();
            }
        }

        public async Task<bool> HasRefreshTokenAsync(AppUser user, string refreshToken)
        {
            using (var selectCommand = db.CreateCommand())
            {
                selectCommand.CommandText = "SELECT 1 FROM RefreshToken WHERE UserId = @userId AND Token = @token";
                selectCommand.Parameters.AddWithValue("@userId", user.Id);
                selectCommand.Parameters.AddWithValue("@token", refreshToken);
                var searchResult = await selectCommand.ExecuteScalarAsync();

                return 1.Equals(searchResult);
            }
        }

        public async Task DeleteRefreshTokenAsync(AppUser user, string refreshToken)
        {
            using (var deleteCommand = db.CreateCommand())
            {
                deleteCommand.CommandText = "DELETE FROM RefreshToken WHERE UserId = @userId AND Token = @token";
                deleteCommand.Parameters.AddWithValue("@userId", user.Id);
                deleteCommand.Parameters.AddWithValue("@token", refreshToken);
                await deleteCommand.ExecuteNonQueryAsync();
            }
        }

        public async Task<int> GetRegisteredAdminsCount()
        {
            using (var selectCommand = db.CreateCommand())
            {
                selectCommand.CommandText = "SELECT COUNT(*) FROM AppUser WHERE UserRole = 'Admin'";
                return (int)await selectCommand.ExecuteScalarAsync();
            }
        }

        public async Task<IEnumerable<AppUserInfo>> GetUsersByNamePaginatedAsync(int pageIndex, int pageSize, string name)
        {
            using (var selectCommand = db.CreateCommand())
            {
                selectCommand.CommandText = "SELECT * FROM GetUsersByNamePaginated(@pageIndex, @pageSize, @name)";
                selectCommand.Parameters.AddWithValue("@pageIndex", pageIndex);
                selectCommand.Parameters.AddWithValue("@pageSize", pageSize);
                selectCommand.Parameters.AddWithValue("@name", name ?? string.Empty);
                using (var r = await selectCommand.ExecuteReaderAsync())
                {
                    IList<AppUserInfo> users = new List<AppUserInfo>();
                    while (await r.ReadAsync())
                    {
                        users.Add(new AppUserInfo
                        {
                            Id = r.GetInt32(0),
                            UserName = r.GetString(1),
                            Email = r.GetString(2),
                            UserRole = r.GetString(3),
                            TeamId = r.IsDBNull(4) ? -1 : r.GetInt32(4),
                            TeamName = r.IsDBNull(5) ? null : r.GetString(5)
                        });
                    }
                    return users;
                }
            }
        }

        public async Task<int> GetUsersByNameCountAsync(string name)
        {
            using (var selectCommand = db.CreateCommand())
            {
                selectCommand.CommandText = "SELECT dbo.GetUsersByNameCount(@name)";
                selectCommand.Parameters.AddWithValue("@name", name ?? string.Empty);
                return (int)await selectCommand.ExecuteScalarAsync();
            }
        }

        #region Default mapping

        protected override AppUser MapFromReader(SqlDataReader r)
        {
            return new AppUser
            {
                Id = r.GetInt32(0),
                UserName = r.GetString(1),
                Email = r.GetString(2),
                HashedPassword = r.GetString(3),
                UserRole = r.GetString(4)
            };
        }

        protected override object[] ExtractInsertValues(AppUser user)
        {
            return new object[]
            {
                user.UserName, user.Email, user.HashedPassword, user.UserRole
            };
        }

        protected override TablePropertyValuePair[] ExtractUpdateProperties(AppUser item)
        {
            return new TablePropertyValuePair[] { };
        }

        protected override int GetPrimaryKeyValue(AppUser user)
        {
            return user.Id;
        }

        #endregion

    }
}
