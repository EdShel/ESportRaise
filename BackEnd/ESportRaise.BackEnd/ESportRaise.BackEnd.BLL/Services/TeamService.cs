using ESportRaise.BackEnd.BLL.DTOs.Team;
using ESportRaise.BackEnd.BLL.DTOs.TeamMember;
using ESportRaise.BackEnd.BLL.Exceptions;
using ESportRaise.BackEnd.DAL.Entities;
using ESportRaise.BackEnd.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.BLL.Services
{
    public sealed class TeamService
    {
        private readonly TeamRepository teams;

        private readonly TeamMemberRepository members;

        private readonly AppUserRepository users;

        public TeamService(TeamRepository teams, TeamMemberRepository members, AppUserRepository users)
        {
            this.teams = teams;
            this.members = members;
            this.users = users;
        }

        public async Task<int> CreateTeamAsync(CreateTeamDTO request)
        {
            TeamMember userAsTeamMember = await members.GetAsync(request.CreatorId);
            if (userAsTeamMember != null)
            {
                throw new BadRequestException("User is already in a team!");
            }

            var team = new Team
            {
                Name = request.Name
            };
            int teamId = await teams.CreateAsync(team);
            await AddTeamMemberAsync(teamId, request.CreatorId);

            return teamId;
        }

        public async Task AddTeamMemberAsync(int teamId, int userId)
        {
            Team team = await teams.GetAsync(teamId);
            if (team == null)
            {
                throw new BadRequestException("Team does not exist!");
            }
            var teamMember = new TeamMember
            {
                Id = userId,
                TeamId = teamId
            };
            await members.CreateAsync(teamMember);
        }

        public async Task RemoveTeamMemberAsync(int teamId, int userId)
        {
            await members.DeleteAsync(userId);

            int membersLeft = (await members.GetAllFromTeamAsync(teamId)).Count();
            if (membersLeft == 0)
            {
                await teams.DeleteAsync(teamId);
            }
        }

        public async Task<TeamDTO> GetTeamAsync(int id)
        {
            Team team = await teams.GetAsync(id);
            if (team == null)
            {
                throw new NotFoundException("Team doesn't exist");
            }

            IEnumerable<TeamMember> currentTeamMembers = await members.GetAllFromTeamAsync(team.Id);
            var teamMembersDTO = new List<TeamDTO.TeamMember>();
            foreach(var member in currentTeamMembers)
            {
                AppUser user = await users.GetAsync(member.Id);
                teamMembersDTO.Add(new TeamDTO.TeamMember
                {
                    Id = user.Id,
                    UserName = user.UserName
                });
            }
            return new TeamDTO
            {
                Id = team.Id,
                Name = team.Name,
                Members = teamMembersDTO
            };
        }
    }
}
