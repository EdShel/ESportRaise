using ESportRaise.BackEnd.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.DAL.Repositories
{
    public class StateRecordRepository : BasicAsyncRepository<StateRecord>
    {
        public StateRecordRepository(SqlConnection sqlConnection) : base(sqlConnection)
        {

        }

        public async override Task<int> CreateAsync(StateRecord stateRecord)
        {
            var saveCommand = db.CreateCommand();
            saveCommand.CommandText =
                "EXEC SaveStateRecord @teamMember, @trainingId, @heartRate, @temperature";
            saveCommand.Parameters.AddWithValue("@teamMember", stateRecord.TeamMemberId);
            saveCommand.Parameters.AddWithValue("@trainingId", stateRecord.TrainingId);
            saveCommand.Parameters.AddWithValue("@trainingId", stateRecord.HeartRate);
            saveCommand.Parameters.AddWithValue("@trainingId", stateRecord.Temperature);
            await saveCommand.ExecuteNonQueryAsync();
            return default;
        }

        public async Task<IEnumerable<StateRecord>> GetMostRecentForUserAsync(
            int trainingId,
            int seconds,
            int userId)
        {
            var selectCommand = db.CreateCommand();
            selectCommand.CommandText =
                "SELECT * FROM StateRecord WHERE TrainingId = @trainingId AND " +
                "TeamMemberId = @userId AND DATEDIFF(SECOND, CreateTime, GETDATE()) < @secs " +
                "ORDER BY CreateTime DESC";
            selectCommand.Parameters.AddWithValue("@trainingId", trainingId);
            selectCommand.Parameters.AddWithValue("@userId", userId);
            selectCommand.Parameters.AddWithValue("@secs", seconds);
            using (var r = await selectCommand.ExecuteReaderAsync())
            {
                var states = new List<StateRecord>();
                while (await r.ReadAsync())
                {
                    states.Add(MapFromReader(r));
                }
                return states;
            }
        }

        public async Task<IEnumerable<StateRecord>> GetMostRecentAsync(
            int trainingId,
            int seconds)
        {
            var selectCommand = db.CreateCommand();
            selectCommand.CommandText =
                "SELECT * FROM StateRecord WHERE TrainingId = @trainingId AND " +
                "DATEDIFF(SECOND, CreateTime, GETDATE()) < @secs " +
                "ORDER BY CreateTime DESC";
            selectCommand.Parameters.AddWithValue("@trainingId", trainingId);
            selectCommand.Parameters.AddWithValue("@secs", seconds);
            using (var r = await selectCommand.ExecuteReaderAsync())
            {
                var states = new List<StateRecord>();
                while (await r.ReadAsync())
                {
                    states.Add(MapFromReader(r));
                }
                return states;
            }
        }

        #region CRUD

        protected override StateRecord MapFromReader(SqlDataReader r)
        {
            return new StateRecord
            {
                Id = r.GetInt32(0),
                TeamMemberId = r.GetInt32(1),
                TrainingId = r.GetInt32(2),
                CreateTime = r.GetDateTime(3),
                HeartRate = r.GetInt32(4),
                Temperature = r.GetFloat(5)
            };
        }

        protected override object[] ExtractInsertValues(StateRecord item)
        {
            throw new NotImplementedException();
        }

        protected override TablePropertyValuePair[] ExtractUpdateProperties(StateRecord item)
        {
            throw new NotImplementedException();
        }

        protected override int GetPrimaryKeyValue(StateRecord item)
        {
            return item.Id;
        }

        #endregion
    }
}
