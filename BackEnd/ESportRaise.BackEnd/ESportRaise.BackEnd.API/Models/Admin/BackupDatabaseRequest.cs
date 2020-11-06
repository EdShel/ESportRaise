using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ESportRaise.BackEnd.API.Models.Admin
{
    public class BackupDatabaseRequest
    {
        [Required]
        public string BackupFile { get; set; }
    }

    public class ConfigurationChangeRequest : List<KeyValuePair<string, string>>
    {
    }
}
