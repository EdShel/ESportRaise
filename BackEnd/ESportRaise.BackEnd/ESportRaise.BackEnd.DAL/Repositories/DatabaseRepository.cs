using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.DAL.Repositories
{
    public class DatabaseRepository : IDisposable
    {
        private SqlConnection db;

        public DatabaseRepository(SqlConnection connection)
        {
            db = connection;
            db.Open();
        }

        public async Task MakeBackupAsync(string backupFile)
        {
            using (var backupCommand = db.CreateCommand())
            {
                backupCommand.CommandText = $"BACKUP DATABASE {db.Database} TO DISK = @file";
                backupCommand.Parameters.AddWithValue("@file", backupFile);
                await backupCommand.ExecuteNonQueryAsync();
            }
        } 

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    db.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
