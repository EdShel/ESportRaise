using ESportRaise.BackEnd.DAL.Entities;
using ESportRaise.BackEnd.DAL.Interfaces;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.DAL.Repositories
{
    public sealed class CoachAsyncRepository : IAsyncRepository<Coach>
    {
        private readonly SqlConnection dbConnection;

        public CoachAsyncRepository(SqlConnection databaseConnection)
        {
            this.dbConnection = databaseConnection;
        }

        public async Task CreateAsync(Coach item)
        {
            var insertCommand = new SqlCommand("INSERT INTO Coach(Name) VALUES(@name)", dbConnection);
            insertCommand.Parameters.AddWithValue("@name", item.Name);
            await insertCommand.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(Coach item)
        {
            var deleteCommand = new SqlCommand("DELETE FROM Coach WHERE Id = @id", dbConnection);
            deleteCommand.Parameters.AddWithValue("@id", item.Id);
            await deleteCommand.ExecuteNonQueryAsync();
        }

        public async Task<IEnumerable<Coach>> GetAllAsync()
        {
            var selectCommand = new SqlCommand("SELECT Id, Name FROM Coach", dbConnection);
            var resultReader = await selectCommand.ExecuteReaderAsync();
            var coaches = new List<Coach>();
            while(await resultReader.ReadAsync())
            {
                var coach = new Coach()
                {
                    Id = resultReader.GetInt32(0),
                    Name = resultReader.GetString(1)
                };
                coaches.Add(coach);
            }

            return coaches;
        }

        public async Task UpdateAsync(Coach item)
        {
            var updateCommand = new SqlCommand("UPDATE Coach SET Name = @name WHERE Id = @id", dbConnection);
            updateCommand.Parameters.AddWithValue("@name", item.Name);
            updateCommand.Parameters.AddWithValue("@id", item.Id);
            await updateCommand.ExecuteNonQueryAsync();
        }
    }
}
