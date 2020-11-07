using System.Threading.Tasks;
using ESportRaise.BackEnd.BLL.DTOs.AppUser;

namespace ESportRaise.BackEnd.BLL.Interfaces
{
    public interface IAppUserService
    {
        Task<bool> DoesAnyAdminExistAsync();

        Task<AppUserDTO> GetUserAsync(int id);

        Task<AppUserDTO> GetUserAsync(string userNameOrEmail);
    }
}