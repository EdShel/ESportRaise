using ESportRaise.BackEnd.DAL.Entities;
using System.Data.SqlClient;

namespace ESportRaise.BackEnd.DAL.Repositories
{
    public sealed class TeamRepository : BasicAsyncRepository<Team>
    {
        public TeamRepository(SqlConnection sqlConnection) : base(sqlConnection)
        {
        }

        #region CRUD

        protected override TablePropertyValuePair[] ExtractUpdateProperties(Team item)
        {
            return new TablePropertyValuePair[]
            {
                new TablePropertyValuePair(nameof(Team.Name), item.Name)
            };
        }

        protected override object[] ExtractInsertValues(Team item)
        {
            return new object[]
            {
                item.Name
            };
        }

        protected override int GetPrimaryKeyValue(Team item)
        {
            return item.Id;
        }

        protected override Team MapFromReader(SqlDataReader r)
        {
            return new Team
            {
                Id = r.GetInt32(0),
                Name = r.GetString(1)
            };
        }

        #endregion
    }
}
