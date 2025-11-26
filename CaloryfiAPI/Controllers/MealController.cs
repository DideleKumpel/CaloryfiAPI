using CaloryfiAPI.Data;
using CaloryfiAPI.DTO;
using CaloryfiAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CaloryfiAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MealListController : Controller
    {
        private readonly AppDatabaseContext _context;

        public MealListController(AppDatabaseContext context)
        {
            _context = context;
        }

        [HttpGet("GetMeals")]
        [Authorize]
        public async Task<IActionResult> GetMeals(DateTime? date = null)
        {
            if (!TryGetUserId(out int userId))
                return BadRequest(new { message = "Error occurred while reading userID" });

            var targetDate = (date ?? DateTime.UtcNow).Date;
            var nextDate = targetDate.AddDays(1);
            try
            {
                var meals = await _context.Meals
                    .Where(m => m.UserId == userId
                                && m.Date_Added >= targetDate
                                && m.Date_Added < nextDate)
                    .Include(m => m.MealComponents)
                        .ThenInclude(mc => mc.Ingredient)
                    .ToListAsync();

                if (meals == null)
                {
                    return NotFound(new { message = "Meal component not found" });
                }
                var result = meals.Select(meal => new MealDTO
                {
                    Id = meal.Id,
                    DateAdded = meal.Date_Added,
                    Ingredients = meal.MealComponents.Select(mc => new IngredientDTO
                    {
                        Id = mc.Ingredient.Id,
                        Name = mc.Ingredient.Name,
                        Kcal = mc.Ingredient.Kcal,
                        Carbs = mc.Ingredient.Carbs,
                        Proteins = mc.Ingredient.Proteins,
                        Fats = mc.Ingredient.Fats,
                        Weight = mc.Weight
                    }).ToList()
                });

                return Ok(result);
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("GetMeal/{mealId}")]
        [Authorize]
        public async Task<IActionResult> GetMeal(int mealId)
        {
            if (!TryGetUserId(out int userId))
                return BadRequest(new { message = "Error occurred while reading userID" });

            try
            {
                var meal = await _context.Meals
                    .Where(m => m.Id == mealId && m.UserId == userId)
                    .Include(m => m.MealComponents)
                        .ThenInclude(mc => mc.Ingredient)
                    .FirstOrDefaultAsync();

                if (meal == null)
                    return NotFound(new { message = "Meal not found." });

                var result = new MealDTO
                {
                    Id = meal.Id,
                    DateAdded = meal.Date_Added,
                    Ingredients = meal.MealComponents.Select(mc => new IngredientDTO
                    {
                        Id = mc.Ingredient.Id,
                        Name = mc.Ingredient.Name,
                        Kcal = mc.Ingredient.Kcal,
                        Carbs = mc.Ingredient.Carbs,
                        Proteins = mc.Ingredient.Proteins,
                        Fats = mc.Ingredient.Fats,
                        Weight = mc.Weight
                    }).ToList()
                };

                return Ok(result);
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("DeleteMeal/{mealId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteMeal(int mealId)
        {
            if (!TryGetUserId(out int userId))
                return BadRequest(new { message = "Error occurred while reading userID" });
            try
            {
                var meal = await _context.Meals
                    .Where(m => m.Id == mealId && m.UserId == userId)
                    .FirstOrDefaultAsync();

                if (meal == null)
                    return NotFound(new { message = "Meal not found." });

                _context.Remove(meal);
                _context.SaveChanges();

                return Ok();
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        private bool TryGetUserId(out int userId)
        {
            return int.TryParse(User.FindFirst("UserID")?.Value, out userId);
        }
    }
}
