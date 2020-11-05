using ESportRaise.BackEnd.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.DAL.Repositories
{
    public class TrainingAsyncRepository : BasicAsyncRepository<Training>
    {
        public TrainingAsyncRepository(SqlConnection sqlConnection) : base(sqlConnection)
        {
        }

        public async Task<int> GetTrainingIdAsync(int userId, int idlenessMinutesForNewTraining)
        {
            var getTrainingCommand = db.CreateCommand();
            getTrainingCommand.CommandText = "EXEC GiveTrainingId @userId, @idleMins";
            getTrainingCommand.Parameters.AddWithValue("@userId", userId);
            getTrainingCommand.Parameters.AddWithValue("@idleMins", idlenessMinutesForNewTraining);

            return (int)await getTrainingCommand.ExecuteScalarAsync();
        }

        public async Task<Training> GetLastForTeamAsync(int teamId)
        {
            var selectCommand = db.CreateCommand();
            selectCommand.CommandText = "SELECT t.Id, t.TeamId, t.BeginTime " +
                                        "FROM LastTraining lt " +
                                        "JOIN Training t ON t.Id = lt.TrainingId " +
                                        "WHERE lt.TeamId = @teamId";
            selectCommand.Parameters.AddWithValue("@teamId", teamId);
            using (var r = await selectCommand.ExecuteReaderAsync())
            {
                if (await r.ReadAsync())
                {
                    return MapFromReader(r);
                }
                return null;
            }
        }

        public async Task<IEnumerable<Training>> GetBeforeDateTimeAsync(int teamId, DateTime dateTime, int hours)
        {
            var selectCommand = db.CreateCommand();
            selectCommand.CommandText = "SELECT * FROM Training " +
                                        "WHERE TeamId = @teamId AND " +
                                        "DATEDIFF(HOUR, BeginTime, @time) <= @hours";
            selectCommand.Parameters.AddWithValue("@teamId", teamId);
            selectCommand.Parameters.AddWithValue("@time", dateTime);
            selectCommand.Parameters.AddWithValue("@hours", hours);

            using (var r = await selectCommand.ExecuteReaderAsync())
            {
                List<Training> trainings = new List<Training>();
                if (await r.ReadAsync())
                {
                    trainings.Add(MapFromReader(r));
                }
                return trainings;
            }
        }

        #region CRUD

        protected override Training MapFromReader(SqlDataReader r)
        {
            return new Training
            {
                Id = r.GetInt32(0),
                BeginTime = r.GetDateTime(1)
            };
        }

        protected override object[] ExtractInsertValues(Training item)
        {
            return new object[0];
        }

        protected override TablePropertyValuePair[] ExtractUpdateProperties(Training item)
        {
            return new TablePropertyValuePair[0];
        }

        protected override int GetPrimaryKeyValue(Training t)
        {
            return t.Id;
        }

        #endregion
    }
}
