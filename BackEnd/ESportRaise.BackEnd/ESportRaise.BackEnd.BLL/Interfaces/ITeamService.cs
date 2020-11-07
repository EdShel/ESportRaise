using System.Threading.Tasks;
using ESportRaise.BackEnd.BLL.DTOs.Team;

namespace ESportRaise.BackEnd.BLL.Interfaces
{
    public interface ITeamService
    {
        Task AddTeamMemberAsync(int teamId, int userId);

        Task<int> CreateTeamAsync(CreateTeamDTO request);

        Task<TeamDTO> GetTeamAsync(int id);

        Task RemoveTeamMemberAsync(int teamId, int userId);
    }
}