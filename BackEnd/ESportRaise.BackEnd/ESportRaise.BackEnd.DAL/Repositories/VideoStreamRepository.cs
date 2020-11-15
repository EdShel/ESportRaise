using ESportRaise.BackEnd.DAL.Entities;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.DAL.Repositories
{
    public class VideoStreamRepository : BasicAsyncRepository<VideoStream>
    {
        public VideoStreamRepository(SqlConnection sqlConnection) : base(sqlConnection)
        {
        }

        public async Task<IEnumerable<VideoStream>> GetForTrainingAsync(int trainingId)
        {
            var selectCommand = db.CreateCommand();
            selectCommand.CommandText = "SELECT * FROM VideoStream WHERE TrainingId = @id";
            selectCommand.Parameters.AddWithValue("@id", trainingId);
            using(var r = await selectCommand.ExecuteReaderAsync())
            {
                var streams = new List<VideoStream>();
                while(await r.ReadAsync())
                {
                    streams.Add(MapFromReader(r));
                }
                return streams;
            }
        }

        #region CRUD

        protected override object[] ExtractInsertValues(VideoStream item)
        {
            return new object[]
            {
                item.TrainingId, item.TeamMemberId, item.YouTubeId
            };
        }

        protected override TablePropertyValuePair[] ExtractUpdateProperties(VideoStream item)
        {
            return new TablePropertyValuePair[]
            {
                new TablePropertyValuePair(nameof(VideoStream.YouTubeId), item.YouTubeId)
            };
        }

        protected override int GetPrimaryKeyValue(VideoStream item)
        {
            return item.Id;
        }

        protected override VideoStream MapFromReader(SqlDataReader r)
        {
            return new VideoStream
            {
                Id = r.GetInt32(0),
                TrainingId = r.GetInt32(1),
                TeamMemberId = r.IsDBNull(2) ? -1 : r.GetInt32(2),
                YouTubeId = r.GetString(3)
            };
        }

        #endregion
    }
}
