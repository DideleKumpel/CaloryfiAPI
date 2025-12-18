using CaloryfiAPI.Data;
using CaloryfiAPI.Models;
using CaloryfiAPI.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CaloryfiAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientsController : Controller
    {
        private readonly AppDatabaseContext _context;

        public IngredientsController(AppDatabaseContext context)
        {
            _context = context;
        }

        [HttpGet("GetIngrediets")]
        [Authorize]
        public async Task<IActionResult> GetIngrediets()
        {
            int userId = -1;
            bool succes = int.TryParse(User.FindFirst("UserID")?.Value, out userId);
            if (!succes)
            {
                return BadRequest(new { message = "Error occured while reading userID" });
            }
            try
            {
                var IngedeintsMeal = await _context.Ingredients.Where(m => m.UserId == null || m.UserId == userId).ToArrayAsync();
                return Ok(IngedeintsMeal);
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost("AddCustomIngredient")]
        [Authorize]
        public async Task<IActionResult> AddCustomIngredient([FromBody]IngredientDTO newIngredient)
        {
            int userId = -1;
            bool succes = int.TryParse(User.FindFirst("UserID")?.Value, out userId);
            if (!succes)
            {
                return BadRequest(new { message = "Error occured while reading userID" });
            }
            if(newIngredient.Name == null || newIngredient.Fats < 0 || newIngredient.Proteins < 0 || newIngredient.Carbs < 0 || newIngredient.Kcal < 0)
            {
                return BadRequest("Invalid ingredient data");
            }
            try
            {
                var ingredient = new Ingredient
                {
                    Name = newIngredient.Name,
                    Kcal = newIngredient.Kcal,
                    Proteins = newIngredient.Proteins,
                    Carbs = newIngredient.Carbs,
                    Fats = newIngredient.Fats,
                    UserId = userId
                };
                _context.Ingredients.Add(ingredient);
                await _context.SaveChangesAsync();
                return Ok(ingredient.Id);
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

    }
}
