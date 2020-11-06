using ESportRaise.BackEnd.BLL.Exceptions;
using ESportRaise.BackEnd.DAL.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
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

        public async Task BackupDatabaseAsync(string backupFile)
        {
            string pathToBackup = Path.Combine(settings.BackupPath, backupFile);
            string backupDirectory = Path.GetDirectoryName(pathToBackup);
            Directory.CreateDirectory(backupDirectory);
            await database.MakeBackupAsync(pathToBackup);
        }

        public Stream GetBackupAsStream(string backupFile)
        {
            string pathToBackup = Path.Combine(settings.BackupPath, backupFile);
            if (!File.Exists(pathToBackup))
            {
                throw new NotFoundException("Backup file doesn't exist!");
            }
            return new FileStream(pathToBackup, FileMode.Open, FileAccess.Read);
        }

        private class BackupSettings
        {
            public string BackupPath { get; set; }
        }
    }
}
