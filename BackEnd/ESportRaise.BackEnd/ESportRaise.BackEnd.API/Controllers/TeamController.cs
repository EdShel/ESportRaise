using ESportRaise.BackEnd.DAL.Entities;
using ESportRaise.BackEnd.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.API.Controllers
{
    //[Route("[controller]"), ApiController]
    //public class TeamController : ControllerBase
    //{
    //    private SqlConnection db;

    //    private TeamAsyncRepository teams;

    //    public TeamController(SqlConnection dbConnection)
    //    {
    //        db = dbConnection;
    //        teams = new TeamAsyncRepository(db);
    //    }

    //    [HttpGet]
    //    public async Task<ActionResult<IEnumerable<string>>> Get()
    //    {
    //        await db.OpenAsync();
    //        var allTeams = await teams.GetAllAsync();
    //        db.Close();
    //        return new ObjectResult(allTeams);
    //    }

    //    [HttpPost]
    //    public async Task Post(string name, int coachId)
    //    {
    //        await db.OpenAsync();
    //        await teams.CreateAsync(new Team
    //        {
    //            Name = name,
    //            CoachId = coachId
    //        });
    //        db.Close();
    //    }

    //    [HttpPut]
    //    public async Task Put(Team team)
    //    {
    //        await db.OpenAsync();
    //        await teams.UpdateAsync(team);
    //        db.Close();
    //        return;
    //    }

    //    [HttpDelete]
    //    public async Task Delete(int id)
    //    {
    //        await db.OpenAsync();
    //        await teams.DeleteAsync(new Team { Id = id });
    //        db.Close();
    //    }
    //}
}
