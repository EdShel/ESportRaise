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
