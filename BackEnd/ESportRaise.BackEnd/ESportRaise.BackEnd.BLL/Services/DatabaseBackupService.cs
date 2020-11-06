using ESportRaise.BackEnd.DAL.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.BLL.Services
{
    public class DatabaseBackupService
    {
        private readonly BackupSettings settings;

        private readonly DatabaseRepository database;

        public DatabaseBackupService(DatabaseRepository database, IConfiguration configuration)
        {
            this.database = database;

            settings = new BackupSettings();
            configuration.Bind("BackupSettings", settings);
        }

        public async Task BackupDatabase(string backupFile)
        {
            await database.MakeBackupAsync(backupFile);
        }

        private class BackupSettings
        {
            public string BackupPath { get; set; }
        }
    }
}
