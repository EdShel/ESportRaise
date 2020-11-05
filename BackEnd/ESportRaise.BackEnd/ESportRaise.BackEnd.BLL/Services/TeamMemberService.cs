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

        public async Task<int> GetTeamIdAsync(int id)
        {
            TeamMember member = await teamMembers.GetAsync(id);
            return member.TeamId;
        }

        public async Task ChangeYouTubeChannelId(int userId, string channelId)
        {
            TeamMember member = await teamMembers.GetAsync(userId);
            member.YouTubeId = string.IsNullOrEmpty(channelId) ? null : channelId;
            await teamMembers.UpdateAsync(member);
        }
    }
}
