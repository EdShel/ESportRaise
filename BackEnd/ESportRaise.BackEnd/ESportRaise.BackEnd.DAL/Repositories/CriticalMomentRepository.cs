using ESportRaise.BackEnd.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.DAL.Repositories
{
    public sealed class CriticalMomentRepository : BasicAsyncRepository<CriticalMoment>
    {
        public CriticalMomentRepository(SqlConnection sqlConnection) : base(sqlConnection)
        {
        }

        public async Task<bool> IsCachedForTrainingAsync(int trainingId)
        {
            using (var selectCommand = db.CreateCommand())
            {
                selectCommand.CommandText =
                    "SELECT HasCachedMoments FROM CriticalMomentsCache WHERE TrainingId = @id";
                selectCommand.Parameters.AddWithValue("@id", trainingId);
                var obj = await selectCommand.ExecuteScalarAsync();
                return Convert.ToBoolean(obj);
            }
        }

        public async Task SetCachedForTrainingAsync(int trainingId)
        {
            using (var insertCommand = db.CreateCommand())
            {
                insertCommand.CommandText =
                    "INSERT INTO CriticalMomentsCache(TrainingId) VALUES(@trainingId)";
                insertCommand.Parameters.AddWithValue("@trainingId", trainingId);
                await insertCommand.ExecuteNonQueryAsync();
            }
        }

        public async Task<IEnumerable<CriticalMoment>> GetForTrainingAsync(int trainingId)
        {
            using (var selectCommand = db.CreateCommand())
            {
                selectCommand.CommandText = $"SELECT * FROM {nameof(CriticalMoment)} WHERE TrainingId = @trainingId";
                selectCommand.Parameters.AddWithValue("@trainingId", trainingId);
                using (var r = await selectCommand.ExecuteReaderAsync())
                {
                    var moments = new List<CriticalMoment>();
                    while (await r.ReadAsync())
                    {
                        moments.Add(MapFromReader(r));
                    }
                    return moments;
                }
            }
        }

        public async Task CreateManyAsync(IEnumerable<CriticalMoment> moments)
        {
            string insertSql = $"INSERT INTO {nameof(CriticalMoment)} VALUES(@trainingId, @beginTime, @endTime)";
            using (var insertCommand = db.CreateCommand())
            {
                insertCommand.CommandText = insertSql;
                insertCommand.Parameters.Add("@trainingId", SqlDbType.Int);
                insertCommand.Parameters.Add("@beginTime", SqlDbType.DateTime);
                insertCommand.Parameters.Add("@endTime", SqlDbType.DateTime);
                using (var transaction = db.BeginTransaction())
                {
                    insertCommand.Transaction = transaction;
                    try
                    {
                        foreach (var moment in moments)
                        {
                            insertCommand.Parameters["@trainingId"].Value = moment.TrainingId;
                            insertCommand.Parameters["@beginTime"].Value = moment.BeginTime;
                            insertCommand.Parameters["@endTime"].Value = moment.EndTime;

                            await insertCommand.ExecuteNonQueryAsync();
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                    transaction.Commit();
                }
            }
        }

        #region CRUD

        protected override object[] ExtractInsertValues(CriticalMoment item)
        {
            return new object[]
            {
                item.Id, item.TrainingId, item.BeginTime, item.EndTime
            };
        }

        protected override TablePropertyValuePair[] ExtractUpdateProperties(CriticalMoment item)
        {
            throw new InvalidOperationException("Entity is immutable!");
        }

        protected override int GetPrimaryKeyValue(CriticalMoment item)
        {
            throw new InvalidOperationException("Entity is immutable!");
        }

        protected override CriticalMoment MapFromReader(SqlDataReader r)
        {
            return new CriticalMoment
            {
                Id = r.GetInt64(0),
                TrainingId = r.GetInt32(1),
                BeginTime = DateTime.SpecifyKind(r.GetDateTime(2), DateTimeKind.Utc),
                EndTime = DateTime.SpecifyKind(r.GetDateTime(3), DateTimeKind.Utc)
            };
        }

        #endregion
    }
}
