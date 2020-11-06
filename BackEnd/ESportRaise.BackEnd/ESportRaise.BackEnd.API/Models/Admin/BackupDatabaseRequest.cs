using System.ComponentModel.DataAnnotations;

namespace ESportRaise.BackEnd.API.Models.Admin
{
    public class BackupDatabaseRequest
    {
        [Required, MinLength(5)]
        public string BackupFile { get; set; }
    }
}
