using ESportRaise.BackEnd.DAL.Entities;
using System;
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

        public async Task SaveStateRecordAsync(StateRecord stateRecord)
        {
            var saveCommand = db.CreateCommand();
            saveCommand.CommandText = 
                "EXEC SaveStateRecord @teamMember, @trainingId, @heartRate, @temperature";
            saveCommand.Parameters.AddWithValue("@teamMember", stateRecord.TeamMemberId);
            saveCommand.Parameters.AddWithValue("@trainingId", stateRecord.TrainingId);
            saveCommand.Parameters.AddWithValue("@trainingId", stateRecord.HeartRate);
            saveCommand.Parameters.AddWithValue("@trainingId", stateRecord.Temperature);
            await saveCommand.ExecuteNonQueryAsync();
        }

        #region CRUD

        protected override Func<SqlDataReader, Training> SelectMapper
        {
            get => r => new Training
            {
                Id = r.GetInt32(0),
                BeginTime = r.GetDateTime(1)
            };
        }

        protected override Func<Training, object[]> InsertValues
        {
            get => t => new object[0];
        }

        protected override Func<Training, TablePropertyValuePair[]> UpdatePropertiesAndValuesExtractor
        {
            get => t => new TablePropertyValuePair[0];
        }

        protected override TablePropertyExtractor UpdatePredicatePropertyEqualsValue
        {
            get => new TablePropertyExtractor("Id", t => t.Id);
        }

        #endregion
    }
}
