using Microsoft.AspNetCore.Mvc;
using NagpServiceApi.Data;
using NagpServiceApi.Models;
namespace NagpServiceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _context.Users.ToList();
            return Ok(users);
        }
        [HttpPost("add")]
        public IActionResult Add(User user)
        {
            if (user == null)
            {
                return BadRequest("User cannot be null");
            }

            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok(user);
        }
    }
}
