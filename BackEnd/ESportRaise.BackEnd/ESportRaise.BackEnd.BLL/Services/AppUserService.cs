using ESportRaise.BackEnd.BLL.DTOs.AppUser;
using ESportRaise.BackEnd.BLL.Exceptions;
using ESportRaise.BackEnd.BLL.Interfaces;
using ESportRaise.BackEnd.DAL.Entities;
using ESportRaise.BackEnd.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.BLL.Services
{
    public sealed class AppUserService : IAppUserService
    {
        private readonly AppUserRepository users;

        public AppUserService(AppUserRepository users)
        {
            this.users = users;
        }

        public async Task<bool> DoesAnyAdminExistAsync()
        {
            return await users.GetRegisteredAdminsCount() != 0;
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

        public async Task<AppUserDTO> GetUserAsync(int id)
        {
            AppUser user = await users.GetAsync(id);
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

        public async Task<AppUsersPaginatedDTO> GetUsersByNamePaginatedAsync(int pageIndex, int pageSize, string name)
        {
            int count = await users.GetUsersByNameCountAsync(name);
            pageIndex = Math.Min(count / pageSize, Math.Max(pageIndex, 0));
            pageSize = Math.Max(1, pageSize);
            IEnumerable<AppUserInfo> foundUsers = await users.GetUsersByNamePaginatedAsync(pageIndex, pageSize, name);
            return new AppUsersPaginatedDTO
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPagesCount = (int)Math.Ceiling((double)count / pageSize),
                Users = foundUsers.Select(u => new AppUserInfoDTO
                {
                    Id = u.Id,
                    Email = u.Email,
                    UserName = u.UserName,
                    UserRole = u.UserRole,
                    TeamId = u.TeamId,
                    TeamName = u.TeamName
                })
            };
        }
    }
}
