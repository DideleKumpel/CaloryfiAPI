using Caloryfi.Model.DTO;
using CaloryfiAPI.Data;
using CaloryfiAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CaloryfiAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MealComponentController: Controller
    {
        private readonly AppDatabaseContext _context;

        public MealComponentController(AppDatabaseContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost("AddComponent")]
        public async Task<IActionResult> AddComponent([FromBody]MealComponentDTO newMealComponent) {
            if (!TryGetUserId(out int userId))
                return BadRequest(new { message = "Error occurred while reading userID" });
            if(newMealComponent == null)
            {
                return BadRequest(new { message = "Error mealComponent is null" });
            }
            try
            {
                MealComponent mealComponentExist = await _context.MealComponents.FirstOrDefaultAsync(m => m.MealId == newMealComponent.MealId && m.IngredientId == newMealComponent.IngredientId);
                if (mealComponentExist != null)
                {
                    return Conflict(new { message = "Meal component already exists" });
                }
                mealComponentExist = new MealComponent
                {
                    MealId = newMealComponent.MealId,
                    IngredientId = newMealComponent.IngredientId,
                    Weight = newMealComponent.Weight
                };
                await _context.MealComponents.AddAsync(mealComponentExist);
                _context.SaveChanges();
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
            return Ok("Component has been added");
        }

        [Authorize]
        [HttpDelete("DeleteComponent/{IngridientId}/{MealId}")]
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
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [Authorize]
        [HttpPost("UpdateWeight")]
        public async Task<IActionResult> UpdateWeight([FromBody] MealComponentDTO UpdatedMeal)
        {
            if (!TryGetUserId(out int userId))
                return BadRequest(new { message = "Error occurred while reading userID" });
            try
            {
                var Meal = await _context.MealComponents.FirstOrDefaultAsync(m => m.MealId == UpdatedMeal.MealId && m.IngredientId == UpdatedMeal.IngredientId);
                if (Meal == null)
                {
                    return NotFound(new { message = "Meal component not found" });
                }
                Meal.Weight = UpdatedMeal.Weight;
                _context.MealComponents.Update(Meal);
                _context.SaveChanges();
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
            return Ok("Component weight has been updated");
        }

        private bool TryGetUserId(out int userId)
        {
            return int.TryParse(User.FindFirst("UserID")?.Value, out userId);
        }
    }
}
