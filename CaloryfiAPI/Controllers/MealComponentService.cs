using CaloryfiAPI.Data;
using CaloryfiAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CaloryfiAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MealComponentService: Controller
    {
        private readonly AppDatabaseContext _context;

        public MealComponentService(AppDatabaseContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost("AddComponent")]
        public async Task<IActionResult> AddComponent(MealComponent mealComponent) {
            if (!TryGetUserId(out int userId))
                return BadRequest(new { message = "Error occurred while reading userID" });
            if(mealComponent == null)
            {
                return BadRequest(new { message = "Error mealComponent is null" });
            }
            try
            {
                await _context.MealComponents.AddAsync(mealComponent);
                _context.SaveChanges();
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
            return Ok("Component has been added");
        }

        [Authorize]
        [HttpPost("DeleteComponent")]
        public async Task<IActionResult> DeleteComponent(int IngridientId, int MealId)
        {
            if (!TryGetUserId(out int userId))
                return BadRequest(new { message = "Error occurred while reading userID" });
            try
            {
                var Meal = await _context.MealComponents.FirstOrDefaultAsync(m => m.MealId == MealId && m.IngredientId == IngridientId);
                if (Meal == null)
                {
                    return NotFound(new { message = "Meal component not found" });
                }
                _context.MealComponents.Remove(Meal);
                _context.SaveChanges();
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
            return Ok("Component has been added");
        }

        [Authorize]
        [HttpGet("GetComponent")]
        public async Task<IActionResult> GetComponent(int IngridientId, int MealId) {
            if (!TryGetUserId(out int userId))
                return BadRequest(new { message = "Error occurred while reading userID" });
            try
            {
                var Meal = await _context.MealComponents.FirstOrDefaultAsync(m => m.MealId == MealId && m.IngredientId == IngridientId);
                if(Meal == null)
                {
                    return NotFound(new { message = "Meal component not found" });
                }
                return Ok(Meal);
            }
            catch
            {
                return NotFound(new { message = "Meal component not found" });
            }
        }

        private bool TryGetUserId(out int userId)
        {
            return int.TryParse(User.FindFirst("UserID")?.Value, out userId);
        }
    }
}
