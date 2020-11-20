using ESportRaise.BackEnd.DAL.Entities;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.DAL.Repositories
{
    public sealed class TeamMemberRepository : BasicAsyncRepository<TeamMember>
    {
        public TeamMemberRepository(SqlConnection sqlConnection) : base(sqlConnection)
        {
            HasIdentityId = false;
        }

        public async Task<IEnumerable<TeamMember>> GetAllFromTeamAsync(int teamId)
        {
            using (var selectCommand = db.CreateCommand())
            {
                selectCommand.CommandText = $"SELECT * FROM {nameof(TeamMember)} WHERE TeamId = @teamId";
                selectCommand.Parameters.AddWithValue("@teamId", teamId);
                using (var r = await selectCommand.ExecuteReaderAsync())
                {
                    var list = new List<TeamMember>();
                    while (await r.ReadAsync())
                    {
                        list.Add(MapFromReader(r));
                    }
                    return list;
                }
            }
        }

        public async override Task DeleteAsync(int id)
        {
            using (var deleteRecordsCommand = db.CreateCommand())
            {
                deleteRecordsCommand.CommandText = $"DELETE FROM {nameof(StateRecord)} " +
                                                   $"WHERE TeamMemberId = @id";
                deleteRecordsCommand.Parameters.AddWithValue("@id", id);
                await deleteRecordsCommand.ExecuteNonQueryAsync();
            }

            using (var setNullForStreams = db.CreateCommand())
            {
                setNullForStreams.CommandText = $"UPDATE {nameof(VideoStream)} SET TeamMemberId = NULL " +
                                                $"WHERE TeamMemberId = @id";
                setNullForStreams.Parameters.AddWithValue("@id", id);
                await setNullForStreams.ExecuteNonQueryAsync();
            }

            await base.DeleteAsync(id);
        }

        #region CRUD

        protected override object[] ExtractInsertValues(TeamMember item)
        {
            return new object[] { item.Id, item.TeamId, item.YouTubeId };
        }

        protected override TablePropertyValuePair[] ExtractUpdateProperties(TeamMember item)
        {
            return new TablePropertyValuePair[]
            {
                new TablePropertyValuePair(nameof(TeamMember.YouTubeId), item.YouTubeId)
            };
        }

        protected override int GetPrimaryKeyValue(TeamMember item)
        {
            return item.Id;
        }

        protected override TeamMember MapFromReader(SqlDataReader r)
        {
            return new TeamMember
            {
                Id = r.GetInt32(0),
                TeamId = r.GetInt32(1),
                YouTubeId = r.IsDBNull(2) ? null : r.GetString(2)
            };
        }

        #endregion
    }
}
