using ESportRaise.BackEnd.BLL.DTOs.TeamMember;
using ESportRaise.BackEnd.BLL.Exceptions;
using ESportRaise.BackEnd.DAL.Entities;
using ESportRaise.BackEnd.DAL.Repositories;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.BLL.Services
{
    public sealed class TeamMemberService
    {
        private readonly TeamRepository teams;

        private readonly TeamMemberRepository teamMembers;

        public TeamMemberService(TeamRepository teams, TeamMemberRepository teamMembers)
        {
            this.teams = teams;
            this.teamMembers = teamMembers;
        }

        public async Task<TeamMemberDTO> GetTeamMemberOrNullAsync(int userId)
        {
            TeamMember member = await teamMembers.GetAsync(userId);
            if (member == null)
            {
                return null;
            }
            return new TeamMemberDTO
            {
                Id = userId,
                TeamId = member.TeamId,
                YouTubeId = member.YouTubeId
            };
        }

        public async Task<int> GetTeamIdAsync(int id)
        {
            TeamMember member = await teamMembers.GetAsync(id);
            if (member == null)
            {
                throw new BadRequestException("User doesn't belong to a team!");
            }
            return member.TeamId;
        }

        public async Task ChangeYouTubeChannelIdAsync(int userId, string channelId)
        {
            TeamMember member = await teamMembers.GetAsync(userId);
            member.YouTubeId = string.IsNullOrEmpty(channelId) ? null : channelId;
            await teamMembers.UpdateAsync(member);
        }
    }
}
