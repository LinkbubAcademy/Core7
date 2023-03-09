using Common.Lib.Authentication;
using Common.Lib.Core.Context;
using Common.Lib.Core.Expressions;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Test.Blazor.Server.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        IRepository<User> Repo { get; set; }

        public UsersController(IContextFactory contextFactory)
        {
            Repo = contextFactory.GetRepository<User>();
        }

        // GET: api/<UsersController>
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            var output = new List<string>();

            var qr1 = await Repo.Where(UserEmail.EqualsTo("lolo@lolo.com"),
                                        UserByPassword.Create("1234"))
                                .ToListAsync();            

            var qr2 = await Repo.Where(UserEmail.EqualsTo("lolo@lolo.com"),
                                        UserByPassword.Create("12345"))
                                .ToListAsync();

            var qr3 = await Repo.SelectAsync(UserEmail.Property);

            var qr4 = await Repo.DistinctAsync(UserEmail.Property);

            output.Add(qr1.Value.Count == 0 ? "test1 (ok) failed" : "test1 (ok) success");
            output.Add(qr2.Value.Count == 0 ? "test1 (error) success" : "test1 (error) failed");
            output.Add(qr3.Value.Count == 2 && qr3.Value[0] == "lolo@lolo.com" ? "test2 Select (ok) success" : "test2 Select (ok) failed");
            output.Add(qr4.Value.Count == 1 && qr3.Value[0] == "lolo@lolo.com" ? "test3 Distinct (ok) success" : "test3 Distinct (ok) failed");

            return output;
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UsersController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
