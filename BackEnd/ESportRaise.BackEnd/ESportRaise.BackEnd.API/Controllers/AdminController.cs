using ESportRaise.BackEnd.API.Models.Admin;
using ESportRaise.BackEnd.BLL.Constants;
using ESportRaise.BackEnd.BLL.DTOs.ConfigChange;
using ESportRaise.BackEnd.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.API.Controllers
{
    [Route("[controller]"), ApiController, Authorize(Roles = AuthConstants.ADMIN_ROLE)]
    public class AdminController : ControllerBase
    {
        private IDatabaseBackupService databaseBackupService;

        private IConfigChangeService configChangeService;

        public AdminController(IDatabaseBackupService databaseBackupService, IConfigChangeService configChangeService)
        {
            this.databaseBackupService = databaseBackupService;
            this.configChangeService = configChangeService;
        }

        [HttpPost("backupDb")]
        public async Task<IActionResult> BackupDatabaseAsync([FromBody] BackupDatabaseRequest request)
        {
            await databaseBackupService.BackupDatabaseAsync(request.BackupFile);

            return Ok();
        }

        [HttpGet("getBackup")]
        public IActionResult SendBackup(string file)
        {
            System.IO.Stream backupFileStream = databaseBackupService.GetBackupAsStream(file);
            return File(backupFileStream, "application/octet-stream");
        }

        [HttpGet("config")]
        public IActionResult GetConfiguration()
        {
            return Ok(configChangeService.GetConfigurations());
        }

        [HttpPut("config")]
        public IActionResult SetConfiguration([FromBody] ConfigurationChangeRequest request)
        {
            configChangeService.ChangeConfiguration(request.Select(opt => new ConfigurationOption
            {
                Key = opt.Key,
                Value = opt.Value
            }));
            return Ok();
        }
    }
}
