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

        public AdminController(DatabaseBackupService databaseBackupService)
        {
            this.databaseBackupService = databaseBackupService;
        }

        [HttpPost("backupDb")]
        public async Task<IActionResult> BackupDatabase([FromBody] BackupDatabaseRequest request)
        {
            await databaseBackupService.BackupDatabase(request.BackupFile);

            return Ok();
        }
    }
}
