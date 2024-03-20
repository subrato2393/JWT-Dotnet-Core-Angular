using DemoSecurityApp.Context;
using DemoSecurityApp.EntityModel;
using DemoSecurityApp.Helpers;
using DemoSecurityApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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

                userEntity.Token = CreateJwt(userEntity);
                var newAccessToken = userEntity.Token;
                var newRefreshToken = CreateRefreshToken();
                userEntity.RefreshToken = newRefreshToken;
                await _context.SaveChangesAsync();
                return Ok(new
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                });
            }
            return BadRequest("User Not Valid");
        }

        [HttpPost("refresh")]
        public async Task<ActionResult> Refresh(TokenDto tokenDto)
        {
            if (tokenDto == null)
                return BadRequest();
            string accessToken = tokenDto.AccessToken;
            string refreshToken = tokenDto.RefreshToken;

            var principle = GetPrincipleFromExpiryToken(accessToken);
            var firstName = principle.Identity.Name;
            var user =await _context.Users.FirstOrDefaultAsync(x => x.FirstName == firstName);
            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return BadRequest("Invalid request");

            var newAccessToken = CreateJwt(user);
            var newRefreshToken = CreateRefreshToken();
            user.RefreshToken = newRefreshToken;
            await _context.SaveChangesAsync();
            return Ok(new TokenDto()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            });
        }
        private string CreateRefreshToken()
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var refreshToken = Convert.ToBase64String(tokenBytes);

            var tokenInUser = _context.Users.Any(x => x.RefreshToken == refreshToken);
            if (tokenInUser)
            {
                return CreateRefreshToken();
            }
            return refreshToken;
        }

        private ClaimsPrincipal GetPrincipleFromExpiryToken(string token)
        {
            var key = Encoding.ASCII.GetBytes("thisismysecretkey.....");
            var tokenValidationParameter = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey =new SymmetricSecurityKey(key),
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime =false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principle = tokenHandler.ValidateToken(token, tokenValidationParameter, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("This is invalid token");
            return principle;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await _context.Users.ToListAsync());
        }

        private string CreateJwt(User userEntity)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("thisismysecretkey.....");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name,userEntity.FirstName),
                new Claim(ClaimTypes.Role,userEntity.Role) 
            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = identity,
                Expires = DateTime.Now.AddMinutes(5),
                SigningCredentials = credentials
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            return jwtTokenHandler.WriteToken(token);
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
