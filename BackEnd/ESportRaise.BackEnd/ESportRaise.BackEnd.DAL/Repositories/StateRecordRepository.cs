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
            using (var saveCommand = db.CreateCommand())
            {
                saveCommand.CommandText =
                    "EXEC SaveStateRecord @teamMember, @trainingId, @heartRate, @temperature";
                saveCommand.Parameters.AddWithValue("@teamMember", stateRecord.TeamMemberId);
                saveCommand.Parameters.AddWithValue("@trainingId", stateRecord.TrainingId);
                saveCommand.Parameters.AddWithValue("@heartRate", stateRecord.HeartRate);
                saveCommand.Parameters.AddWithValue("@temperature", stateRecord.Temperature);
                await saveCommand.ExecuteNonQueryAsync();
                return default;
            }
        }

        public async Task<IEnumerable<StateRecord>> GetForTrainingAndUserMostRecentAsync(
            int trainingId,
            int seconds,
            int userId)
        {
            using (var selectCommand = db.CreateCommand())
            {
                selectCommand.CommandText =
                    "SELECT * FROM StateRecord WHERE TrainingId = @trainingId AND " +
                    "TeamMemberId = @userId AND DATEDIFF(SECOND, CreateTime, GETUTCDATE()) < @secs " +
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
        }

        public async Task<IEnumerable<StateRecord>> GetForTrainingMostRecentAsync(
            int trainingId,
            int seconds)
        {
            using (var selectCommand = db.CreateCommand())
            {
                selectCommand.CommandText =
                    "SELECT * FROM StateRecord WHERE TrainingId = @trainingId AND " +
                    "DATEDIFF(SECOND, CreateTime, GETUTCDATE()) < @secs " +
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
        }

        public async Task<IEnumerable<StateRecord>> GetForTrainingAsync(int trainingId)
        {
            using (var selectCommand = db.CreateCommand())
            {
                selectCommand.CommandText =
                    "SELECT * FROM StateRecord WHERE TrainingId = @trainingId ORDER BY CreateTime";
                selectCommand.Parameters.AddWithValue("@trainingId", trainingId);
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
        }

        public async Task<IEnumerable<StateRecord>> GetForTrainingAsync(int trainingId, int teamMemberId)
        {
            using (var selectCommand = db.CreateCommand())
            {
                selectCommand.CommandText =
                    "SELECT * FROM StateRecord WHERE TrainingId = @trainingId " +
                    "AND TeamMemberId = @teamMemberId ORDER BY CreateTime";
                selectCommand.Parameters.AddWithValue("@trainingId", trainingId);
                selectCommand.Parameters.AddWithValue("@teamMemberId", teamMemberId);
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
        }

        #region CRUD

        protected override StateRecord MapFromReader(SqlDataReader r)
        {
            return new StateRecord
            {
                Id = r.GetInt64(0),
                TeamMemberId = r.GetInt32(1),
                TrainingId = r.GetInt32(2),
                CreateTime = DateTime.SpecifyKind(r.GetDateTime(3), DateTimeKind.Utc),
                HeartRate = r.GetInt32(4),
                Temperature = (float)r.GetDouble(5)
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
            throw new NotImplementedException();
        }

        #endregion
    }
}
