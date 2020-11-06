using System.ComponentModel.DataAnnotations;

namespace ESportRaise.BackEnd.API.Models.Admin
{
    public class BackupDatabaseRequest
    {
        [Required]
        public string BackupFile { get; set; }
    }
}
