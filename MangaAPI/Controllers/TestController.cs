using MangaAPI.Models.Input;
using MangaAPI.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace MangaAPI.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly MangasContext _context;

        public TestController(MangasContext context)
        {
            _context = context;
        }

        [HttpPut("{id}")]
        public IActionResult Upgrade(Guid id)
        {
            var user = _context.Users.SingleOrDefault(user =>  user.Id == id && !user.isDeleted);

            if (user == null) { return NotFound(); }

            user.Upgrade(Entities.UserType.Admin);
            _context.SaveChanges();

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] TesteInputModel files)
        {

            return Ok();
        }
    }
}
