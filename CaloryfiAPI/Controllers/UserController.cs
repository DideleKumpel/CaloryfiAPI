using CaloryfiAPI.Data;
using CaloryfiAPI.DTO;
using CaloryfiAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CaloryfiAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly AppDatabaseContext _context;
        private readonly IConfiguration _configuration;

        public UserController(AppDatabaseContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("GetProfile")]
        [Authorize]
        public async Task<IActionResult> GetUserProfile()
        {
            int userId = -1;
            bool succes = int.TryParse(User.FindFirst("UserID")?.Value, out userId);
            if (userId <= 0)
            {
                return BadRequest(new { message = "Invalid userId." });
            }
            User UserData = null;

            try
            {
                UserData = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }

            if (UserData == null)
            {
                return NotFound(new { message = "This user dont exist" });
            }

            UserProfileData UserProfile = new UserProfileData(UserData);

            return Ok(UserData);
        }


        [HttpPost("CreateAccount")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAccount([FromBody] RegisterAccountDTO registerAccount)
        {
            if (string.IsNullOrEmpty(registerAccount.Email) || string.IsNullOrEmpty(registerAccount.Username) || string.IsNullOrEmpty(registerAccount.Password))
            {
                return BadRequest("Email, password, nickname are empty.");
            }
            if (registerAccount.Weight < 1)
            {
                return BadRequest("Invalid weight");
            }
            if (!IsValidEmail(registerAccount.Email))
            {
                return BadRequest("Email is invalid");
            }
            try
            {
                //Check if user with this email already exist
                var accoutn = await _context.Users.FirstOrDefaultAsync(u => u.Email == registerAccount.Email);
                if (accoutn != null)
                {
                    return Conflict("User with this email exist.");
                }
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
            if (!IsValidPassword(registerAccount.Password))
            {
                return BadRequest("Password must have number and apitalized letter");
            }

            //Making user rekord
            User User = new User
            {
                Email = registerAccount.Email,
                Username = registerAccount.Username,
                Password = HashPassword(registerAccount.Password),
            };

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.Users.AddAsync(User);
                _context.SaveChanges();

                UserSetting Settings = new UserSetting { 
                    UserId = User.Id,
                    Sex = registerAccount.Sex,
                    NumberOfMeals= 4,
                    DietGoal = 1,  //weight 0-lose 2-mentain 3-gain
                    ActivityLevel = 2, // none 0 to 4 high
                    Kcal = CalculateCalory(registerAccount.Weight, registerAccount.Sex, 1, 2),
                    Proteins = 0.3M,
                    Fats = 0.3M,
                    Carbs = 0.4M
                };

                await _context.UserSettings.AddAsync(Settings);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
            return Ok("Account has benn created");
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            User User = null;
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Email and password are required." });
            }
            try
            {
                User = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == HashPassword(request.Password));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
            if (User == null)
            {
                return Unauthorized();
            }
            var token = GenerateJwtToken(User.Id);
            return Ok(new { Token = token });
        }

        [Authorize]
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            int userId = -1;
            bool succes = int.TryParse(User.FindFirst("UserID")?.Value, out userId);
            if (succes && userId > 0)
            {
                var token = GenerateJwtToken(userId);
                return Ok(new { Token = token });
            }
            return BadRequest(new { message = "Error occured while reading userID" });
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            return System.Text.RegularExpressions.Regex.IsMatch(
                email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }

        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 12)
            {
                return false;
            }

            bool hasUpperCase = password.Any(char.IsUpper);
            bool hasDigit = password.Any(char.IsDigit);

            return hasUpperCase && hasDigit;
        }
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private int CalculateCalory(int Weight, bool Sex, int Goal, int Acitvity)
        {
            double CaloricDemand = Weight * 10;
            if (Sex) // for female
            {
                CaloricDemand += 700;
            }
            else //for male
            {
                CaloricDemand += 900;
            }
            switch (Acitvity)
            {
                case 0:
                    CaloricDemand *= 1.2;
                    break;
                case 1:
                    CaloricDemand *= 1.35;
                    break;
                case 2:
                    CaloricDemand *= 1.5;
                    break;
                case 3:
                    CaloricDemand *= 1.65;
                    break;
                case 4:
                    CaloricDemand *= 1.8;
                    break;
            }

            return (int)CaloricDemand;
        }

        private string GenerateJwtToken(int userid)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtConfig:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtConfig:Issuer"],
                audience: _configuration["JwtConfig:Audience"],
                claims: new[] { new Claim("UserID", userid.ToString()) },
                expires: DateTime.Now.AddHours(3),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
