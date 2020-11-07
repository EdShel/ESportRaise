using System.IO;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.BLL.Interfaces
{
    public interface IDatabaseBackupService
    {
        Task BackupDatabaseAsync(string backupFile);
        Stream GetBackupAsStream(string backupFile);
    }
}