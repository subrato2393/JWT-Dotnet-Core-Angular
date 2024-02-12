using DemoSecurityApp.Context;
using DemoSecurityApp.EntityModel;
using DemoSecurityApp.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.RegularExpressions;

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
            if (ModelState.IsValid)
            {
                //if (user == null)
                //{
                //    return BadRequest("user can't be null");
                //}
                //if (string.IsNullOrEmpty(user.FirstName))
                //{
                //    return NotFound("first name can't  be null");
                //}
                //if (string.IsNullOrEmpty(user.Password))
                //{
                //    return NotFound("password can't be null");
                //}

                var userEntity = await _context.Users.FirstOrDefaultAsync(x => x.FirstName == user.FirstName);

                if (!PasswordHasher.VerifyPassword(user.Password, userEntity.Password))
                {
                    return BadRequest("Password not match");
                }

                if (userEntity == null)
                {
                    return NotFound("user can't be null");
                }
            }
            return Ok(new { Message = "Login Success" });
        }


        [HttpPost]
        [Route("signup")]
        public async Task<IActionResult> Signup([FromBody] User user)
        {
            if (ModelState.IsValid)
            {
                // validate phone no
                if (await ValidatePhoneNo(user.PhoneNo))
                {
                    return BadRequest("Phone No Already Exist");
                }

                // validate email
                if (await ValidateEmail(user.Email))
                {
                    return BadRequest("Email Already Exist");
                }

                //password strength check
                var password = PassWordStrengthCheck(user.Password);
                if (!string.IsNullOrEmpty(password))
                {
                    return BadRequest(password);
                }

                user.Password = PasswordHasher.HashPassword(user.Password);
                user.Role = "User";
                user.Token = "";

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
            }
            //if (user == null)
            //{
            //    return BadRequest();
            //}
            //if (string.IsNullOrEmpty(user.FirstName))
            //{
            //    return NotFound("First Name can't be null");
            //}
            //if (string.IsNullOrWhiteSpace(user.LastName))
            //{
            //    return NotFound("Last Name can't be null");
            //}
            //if (string.IsNullOrWhiteSpace(user.Password))
            //{
            //    return NotFound("Password can't be null");
            //}
            return Ok(new { Message = "Signup successfull" });
        }

        private async Task<bool> ValidatePhoneNo(string phoneNo)
        {
           return await _context.Users.AnyAsync(x => x.PhoneNo == phoneNo);
        }

        private async Task<bool> ValidateEmail(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email == email);
        }

        private string PassWordStrengthCheck(string password)
        {
            StringBuilder sb = new StringBuilder();

            if(password.Length < 8)
            {
               sb.Append("Password must be 8 character long"+Environment.NewLine);
            }

            if(!Regex.IsMatch(password,"[a-z]") && Regex.IsMatch(password,"[A-Z]")  && Regex.IsMatch(password, "[0-10]"))
            {
                sb.Append("Password must contain alphanumeric"+Environment.NewLine);
            }

            if (!Regex.IsMatch(password,"[@,!,$,%,^,&,*,(,),*,<,?,:,|]"))
            {
                sb.Append("Password Must contain special character"+Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}
