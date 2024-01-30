using DemoSecurityApp.Context;
using DemoSecurityApp.EntityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoSecurityApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly SecurityDBContext _context;
        public UserController(SecurityDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel user)
        {
            if (user == null)
            {
                return BadRequest("user can't be null");
            }
            if (string.IsNullOrEmpty(user.FirstName))
            {
                return NotFound("first name can't  be null");
            }
            if (string.IsNullOrEmpty(user.Password))
            {
                return NotFound("password can't be null");
            }
            var userEntity = await _context.Users.FirstOrDefaultAsync(x=>x.FirstName == user.FirstName && x.Password == user.Password);

            if(userEntity == null)
            {
                return NotFound("user can't be null");
            }

            return Ok(new { Message = "Login Success" });
        }


        [HttpPost]
        [Route("signup")]
        public async Task<IActionResult> Signup([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            if (string.IsNullOrEmpty(user.FirstName))
            {
                return NotFound("First Name can't be null");
            }
            if (string.IsNullOrWhiteSpace(user.LastName))
            {
                return NotFound("Last Name can't be null");
            }
            if (string.IsNullOrWhiteSpace(user.Password))
            {
                return NotFound("Password can't be null");
            }

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Signup successfull" });
        }
    }
}
