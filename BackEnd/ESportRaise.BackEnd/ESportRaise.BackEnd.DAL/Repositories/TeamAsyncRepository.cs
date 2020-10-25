using ESportRaise.BackEnd.DAL.Entities;
using ESportRaise.BackEnd.DAL.Interfaces;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.DAL.Repositories
{
    //public sealed class TeamAsyncRepository : IAsyncRepository<Team>
    //{
    //    private readonly SqlConnection dbConnection;

    //    public TeamAsyncRepository(SqlConnection databaseConnection)
    //    {
    //        this.dbConnection = databaseConnection;
    //    }

    //    public async Task CreateAsync(Team item)
    //    {
    //        var insertCommand = new SqlCommand(
    //            "INSERT INTO Team(Name, CoachId) VALUES(@name, @coachId)", dbConnection);
    //        insertCommand.Parameters.AddWithValue("@name", item.Name);
    //        insertCommand.Parameters.AddWithValue("@coachId", item.CoachId);
    //        await insertCommand.ExecuteNonQueryAsync();
    //    }

    //    public async Task DeleteAsync(Team item)
    //    {
    //        var deleteCommand = new SqlCommand("DELETE FROM Team WHERE Id = @id", dbConnection);
    //        deleteCommand.Parameters.AddWithValue("@id", item.Id);
    //        await deleteCommand.ExecuteNonQueryAsync();
    //    }

    //    public async Task<IEnumerable<Team>> GetAllAsync()
    //    {
    //        var selectCommand = new SqlCommand("SELECT Id, Name, CoachId FROM Team", dbConnection);
    //        var resultReader = await selectCommand.ExecuteReaderAsync();
    //        var teams = new List<Team>();
    //        while (await resultReader.ReadAsync())
    //        {
    //            var team = new Team()
    //            {
    //                Id = resultReader.GetInt32(0),
    //                Name = resultReader.GetString(1),
    //                CoachId = resultReader.GetInt32(2)
    //            };
    //            teams.Add(team);
    //        }

    //        return teams;
    //    }

    //    public async Task UpdateAsync(Team item)
    //    {
    //        var updateCommand = new SqlCommand(
    //            "UPDATE Team SET Name = @name, CoachId = @coachId WHERE Id = @id", dbConnection);
    //        updateCommand.Parameters.AddWithValue("@name", item.Name);
    //        updateCommand.Parameters.AddWithValue("@coachId", item.CoachId);
    //        updateCommand.Parameters.AddWithValue("@id", item.Id);
    //        await updateCommand.ExecuteNonQueryAsync();
    //    }
    //}
}
