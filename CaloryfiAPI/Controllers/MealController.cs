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

            var meals = await _context.Meals
                .Where(m => m.UserId == userId
                            && m.Date_Added >= targetDate
                            && m.Date_Added < nextDate)
                .Include(m => m.MealComponents)
                    .ThenInclude(mc => mc.Ingredient)
                .ToListAsync();

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

        [HttpGet("GetMeal/{mealId}")]
        [Authorize]
        public async Task<IActionResult> GetMeal(int mealId)
        {
            if (!TryGetUserId(out int userId))
                return BadRequest(new { message = "Error occurred while reading userID" });

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
        private bool TryGetUserId(out int userId)
        {
            return int.TryParse(User.FindFirst("UserID")?.Value, out userId);
        }
    }
}
