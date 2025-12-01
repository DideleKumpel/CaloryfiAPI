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

    }
}
