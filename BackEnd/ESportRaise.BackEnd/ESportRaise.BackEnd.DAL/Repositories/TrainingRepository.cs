﻿using ESportRaise.BackEnd.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.DAL.Repositories
{
    public class TrainingRepository : BasicAsyncRepository<Training>
    {
        public TrainingRepository(SqlConnection sqlConnection) : base(sqlConnection)
        {
        }

        public async Task StopCurrentTrainingForTeamAsync(int teamId)
        {
            var stopCommand = db.CreateCommand();
            stopCommand.CommandText = "UPDATE LastTraining SET TrainingId = NULL WHERE TeamId = @teamId";
            stopCommand.Parameters.AddWithValue("@teamId", teamId);
            await stopCommand.ExecuteNonQueryAsync();
        }


        public async Task<DateTime> GetTrainingEndTimeAsync(int trainingId)
        {
            var checkCommand = db.CreateCommand();
            checkCommand.CommandText = 
                "SELECT MAX(CreateTime) FROM StateRecord " +
                "WHERE TrainingId = @trainingId";
            checkCommand.Parameters.AddWithValue("@trainingId", trainingId);

            DateTime lastRecordTime = (DateTime)await checkCommand.ExecuteScalarAsync();
            if (lastRecordTime != default)
            {
                return lastRecordTime;
            }

            var getBeginCommand = db.CreateCommand();
            getBeginCommand.CommandText = "SELECT BeginTime FROM Training WHERE Id = @id";
            getBeginCommand.Parameters.AddWithValue("@id", trainingId);
            return (DateTime)await getBeginCommand.ExecuteScalarAsync();
        }

        public async Task<int> GiveNewTrainingIdAsync(int userId, int idlenessMinutesForNewTraining)
        {
            var getTrainingCommand = db.CreateCommand();
            getTrainingCommand.CommandText = "EXEC GiveTrainingId @userId, @idleMins";
            getTrainingCommand.Parameters.AddWithValue("@userId", userId);
            getTrainingCommand.Parameters.AddWithValue("@idleMins", idlenessMinutesForNewTraining);

            return (int)await getTrainingCommand.ExecuteScalarAsync();
        }

        public async Task<Training> GetCurrentTrainingForTeamAsync(int teamId)
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