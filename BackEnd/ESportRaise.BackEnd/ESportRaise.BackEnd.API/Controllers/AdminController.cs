using ESportRaise.BackEnd.API.Models.Admin;
using ESportRaise.BackEnd.BLL.Constants;
using ESportRaise.BackEnd.BLL.DTOs.AppUser;
using ESportRaise.BackEnd.BLL.DTOs.ConfigChange;
using ESportRaise.BackEnd.BLL.Exceptions;
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
        private readonly IDatabaseBackupService databaseBackupService;

        private readonly IConfigChangeService configChangeService;

        private readonly IAppUserService userService;

        public AdminController(
            IDatabaseBackupService databaseBackupService,
            IConfigChangeService configChangeService,
            IAppUserService appUserService)
        {
            this.databaseBackupService = databaseBackupService;
            this.configChangeService = configChangeService;
            this.userService = appUserService;
        }

        [HttpPost("backupDb")]
        public async Task<IActionResult> BackupDatabaseAsync([FromBody] BackupDatabaseRequest request)
        {
            if (request.BackupFile == null)
            {
                throw new BadRequestException("Backup file isn't specified!");
            }
            await databaseBackupService.BackupDatabaseAsync(request.BackupFile);

            return Ok();
        }

        [HttpGet("getBackup")]
        public IActionResult SendBackup(string file)
        {
            if (file == null)
            {
                throw new BadRequestException("Backup file isn't specified!");
            }
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

        [HttpGet("users")]
        public async Task<IActionResult> GetUsersAsync(int pageIndex, int pageSize, string name)
        {
            AppUsersPaginatedDTO result = await userService.GetUsersByNamePaginatedAsync(pageIndex, pageSize, name);
            return Ok(result);
        }
    }
}
