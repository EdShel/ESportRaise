using System.Collections.Generic;

namespace ESportRaise.BackEnd.BLL.DTOs.AppUser
{
    public sealed class AppUsersPaginatedDTO
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int TotalPagesCount { get; set; }

        public IEnumerable<AppUserInfoDTO> Users { get; set; }
    }
}
