using System.Threading.Tasks;
using ESportRaise.BackEnd.BLL.DTOs.TeamMember;

namespace ESportRaise.BackEnd.BLL.Interfaces
{
    public interface ITeamMemberService
    {
        Task ChangeYouTubeChannelIdAsync(int userId, string channelId);

        Task<int> GetTeamIdAsync(int id);

        Task<TeamMemberDTO> GetTeamMemberOrNullAsync(int userId);
    }
}