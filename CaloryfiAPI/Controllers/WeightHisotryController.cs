using CaloryfiAPI.Data;
using CaloryfiAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaloryfiAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeightHisotryController : Controller
    {
        private readonly AppDatabaseContext _context;

        public WeightHisotryController(AppDatabaseContext context)
        {
            _context = context;
        }

        [HttpGet("GetCurrentWeight")]
        [Authorize]
        private async Task<IActionResult> GetCurrentWeight()
        {
            int userId = -1;
            bool succes = int.TryParse(User.FindFirst("UserID")?.Value, out userId);
            if(succes == false || userId<0){
                return BadRequest(new { message = "Error occured while reading userID" });
            }
            try
            {
                var Weight = _context.WeightHistories.LastOrDefault(w => w.UserId == userId);
                if(Weight == null)
                {
                    return NotFound("Weight history not found add your weight");
                }
                return Ok(Weight);
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost("UpdateWeight/{newWeight}")]
        [Authorize]
        private async Task<IActionResult> UpdateWeight(int newWeight)
        {
            int userId = -1;
            bool succes = int.TryParse(User.FindFirst("UserID")?.Value, out userId);
            if (succes == false || userId < 0)
            {
                return BadRequest(new { message = "Error occured while reading userID" });
            }
            try
            {
                var Weight = _context.WeightHistories.LastOrDefault(w => w.UserId == userId && w.Date == DateTime.UtcNow);
                if (Weight == null)
                {
                    WeightHistory weightHistory = new WeightHistory
                    {
                        UserId = userId,
                        Weight = newWeight,
                        Date = DateTime.UtcNow
                    };
                    await _context.WeightHistories.AddAsync(Weight);
                }
                else
                {
                    Weight.Weight = newWeight;
                    _context.WeightHistories.Update(Weight);
                }
                await _context.SaveChangesAsync();
                return Ok(Weight);
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        }
    }
}
