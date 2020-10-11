using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ESportRaise.BackEnd.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ESportRaise.BackEnd.API.Controllers
{
    [Route("coach/[controller]")]
    [ApiController]
    public class CoachesController : ControllerBase
    {
        SqlConnection db;

        CoachAsyncRepository coaches;

        public CoachesController()
        {
            db = new SqlConnection("Data Source=DESKTOP-KNVFLPP;Initial Catalog=ESR;Integrated Security=True");
            coaches = new CoachAsyncRepository(db);
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            await db.OpenAsync();
            var allCoaches =  (await coaches.GetAllAsync())
                .Select(coach => coach.Name);
            db.Close();
            return new JsonResult(allCoaches);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        public async Task Put(string value)
        {
            await db.OpenAsync();
            await coaches.CreateAsync(new DAL.Entities.Coach
            {
                Name = value
            });
            db.Close();
        }

        // DELETE api/values/5
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
