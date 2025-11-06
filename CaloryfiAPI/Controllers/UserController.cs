using CaloryfiAPI.Data;
using CaloryfiAPI.DTO;
using CaloryfiAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
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

        public UserController(AppDatabaseContext context)
        {
            _context = context;
        }

        [HttpGet("GetProfile")]
        [Authorize]
        public async Task<IActionResult> GetUserProfile(int userId)
        {
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

            return Ok(UserProfile);
        }


        [HttpPost("CreateAccount")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAccount([FromBody] RegisterAccountDTO registerAccount)
        {
            if (string.IsNullOrEmpty(registerAccount.Email) || string.IsNullOrEmpty(registerAccount.Username) || string.IsNullOrEmpty(registerAccount.Password))
            {
                return BadRequest("Email, password, nickname are empty.");
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
            try
            {
                await _context.Users.AddAsync(User);
                _context.SaveChanges();
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
            return Ok("Account has benn created");
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

    }
}
