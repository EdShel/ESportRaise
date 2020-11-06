using ESportRaise.BackEnd.API.Models.Admin;
using ESportRaise.BackEnd.BLL.Constants;
using ESportRaise.BackEnd.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.API.Controllers
{
    [Route("[controller]"), ApiController, Authorize(Roles = AuthConstants.ADMIN_ROLE)]
    public class AdminController : ControllerBase
    {
        private DatabaseBackupService databaseBackupService;

        private ConfigChangeService configChangeService;

        public AdminController(DatabaseBackupService databaseBackupService, ConfigChangeService configChangeService)
        {
            this.databaseBackupService = databaseBackupService;
            this.configChangeService = configChangeService;
        }

        [HttpPost("backupDb")]
        public async Task<IActionResult> BackupDatabase([FromBody] BackupDatabaseRequest request)
        {
            await databaseBackupService.BackupDatabase(request.BackupFile);

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
    }
}
