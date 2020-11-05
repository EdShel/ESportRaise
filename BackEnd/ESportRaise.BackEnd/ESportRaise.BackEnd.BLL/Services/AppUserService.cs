using ESportRaise.BackEnd.BLL.DTOs.AppUser;
using ESportRaise.BackEnd.BLL.Exceptions;
using ESportRaise.BackEnd.DAL.Entities;
using ESportRaise.BackEnd.DAL.Repositories;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.BLL.Services
{
    public sealed class AppUserService
    {
        private readonly AppUserRepository users;

        public AppUserService(AppUserRepository users)
        {
            this.users = users;
        }

        public async Task<AppUserDTO> GetUserAsync(string userNameOrEmail)
        {
            AppUser user = await users.GetUserOrDefaultByEmailOrUserNameAsync(userNameOrEmail);
            if (user == null)
            {
                throw new NotFoundException("User doesn't exist!");
            }

            return new AppUserDTO
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName
            };
        }
    }
}
