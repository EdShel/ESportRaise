using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using ESportRaise.BackEnd.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ESportRaise.BackEnd.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CoachController : ControllerBase
    {
        private SqlConnection db;

        private CoachAsyncRepository coaches;

        public CoachController(SqlConnection dbConnection)
        {
            db = dbConnection;
            coaches = new CoachAsyncRepository(db);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            await db.OpenAsync();
            var allCoaches = await coaches.GetAllAsync();
            db.Close();
            return new JsonResult(allCoaches);
        }

        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        public async Task Put(string value)
        {
            await db.OpenAsync();
            await coaches.CreateAsync(new DAL.Entities.Coach
            {
                Name = value
            });
            db.Close();
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await db.OpenAsync();
            await coaches.DeleteAsync(new DAL.Entities.Coach
            {
                Id = id,
            });
            db.Close();
        }
    }
}
