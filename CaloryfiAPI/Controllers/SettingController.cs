using CaloryfiAPI.Data;
using CaloryfiAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CaloryfiAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SettingController : Controller
    {
        private readonly AppDatabaseContext _context;
        private readonly IConfiguration _configuration;

        public SettingController(AppDatabaseContext context)
        {
            _context = context;
        }

        [HttpGet("GetProfile")]
        [Authorize]
        public async Task<IActionResult> GetSettings(int UserId)
        {
            int userId = -1;
            bool succes = int.TryParse(User.FindFirst("UserID")?.Value, out userId);
            if (!succes)
            {
                return BadRequest(new { message = "Error occured while reading userID" });
            }
            else
            {
                try
                {
                    UserSetting Settings = await _context.UserSettings.FirstOrDefaultAsync(s => s.UserId == userId);
                    if (Settings == null)
                    {
                        return NotFound(new { message = "User settings not found." });
                    }
                    return Ok(Settings);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "An error occurred while processing your request.");
                }
            }
        }

        [HttpPost("UpdateProfile")]
        [Authorize]
        public async Task<IActionResult> UpdateSettings([FromBody] UserSetting updatedSettings)
        {
            int userId = -1;
            bool succes = int.TryParse(User.FindFirst("UserID")?.Value, out userId);
            if (!succes)
            {
                return BadRequest(new { message = "Error occured while reading userID" });
            }
            try
            {
                UserSetting CurrentUserSettings = await _context.UserSettings.FirstOrDefaultAsync(u => u.UserId == userId);
                if(CurrentUserSettings == null) // no settings found
                {
                    return NotFound(new { message = "User settings not found." });
                }
                
                if (!IsValidSetting(updatedSettings))
                {
                    return BadRequest(new { message = "Invalid settings data." });
                }

                CurrentUserSettings.Sex = updatedSettings.Sex;
                CurrentUserSettings.NumberOfMeals = updatedSettings.NumberOfMeals;
                CurrentUserSettings.DietGoal = updatedSettings.DietGoal;
                CurrentUserSettings.ActivityLevel = updatedSettings.ActivityLevel;
                CurrentUserSettings.Kcal = updatedSettings.Kcal;
                CurrentUserSettings.Carbs = updatedSettings.Carbs;
                CurrentUserSettings.Proteins = updatedSettings.Proteins;
                CurrentUserSettings.Fats = updatedSettings.Fats;

                _context.UserSettings.Update(CurrentUserSettings);
                await _context.SaveChangesAsync();
                return Ok(CurrentUserSettings);
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        private bool IsValidSetting(UserSetting setting)
        {
            if (setting.NumberOfMeals < 1 || setting.NumberOfMeals > 10)
                return false;
            if (setting.DietGoal < 0 || setting.DietGoal > 3)
                return false;
            if (setting.ActivityLevel < 0 || setting.ActivityLevel > 4)
                return false;
            if (setting.Kcal < 0 || setting.Carbs < 0 || setting.Proteins < 0 || setting.Fats < 0)
                return false;
            if (setting.Carbs + setting.Proteins + setting.Fats != 1)
                return false;
            return true;
        }
    }
}
