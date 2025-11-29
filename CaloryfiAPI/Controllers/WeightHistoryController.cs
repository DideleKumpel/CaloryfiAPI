using CaloryfiAPI.Data;
using CaloryfiAPI.DTO;
using CaloryfiAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CaloryfiAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeightHistoryController : Controller
    {
        private readonly AppDatabaseContext _context;

        public WeightHistoryController(AppDatabaseContext context)
        {
            _context = context;
        }

        [HttpGet("GetCurrentWeight")]
        [Authorize]
        public async Task<IActionResult> GetCurrentWeight()
        {
            int userId = -1;
            bool succes = int.TryParse(User.FindFirst("UserID")?.Value, out userId);
            if (succes == false || userId < 0)
            {
                return BadRequest(new { message = "Error occured while reading userID" });
            }
            try
            {
                var Weight  = await _context.WeightHistories
                            .Where(w => w.UserId == userId)
                            .OrderByDescending(w => w.Date)
                            .FirstOrDefaultAsync();
                if (Weight == null)
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
        public async Task<IActionResult> UpdateWeight(int newWeight)
        {
            int userId = -1;
            bool succes = int.TryParse(User.FindFirst("UserID")?.Value, out userId);
            if (succes == false || userId < 0)
            {
                return BadRequest(new { message = "Error occured while reading userID" });
            }
            try
            {
                var today = DateTime.UtcNow.Date;   
                var tomorrow = today.AddDays(1);
                var Weight = _context.WeightHistories.FirstOrDefault(w => w.UserId == userId && w.Date >= today && w.Date < tomorrow);
                if (Weight == null)
                {
                    Weight = new WeightHistory
                    {
                        UserId = userId,
                        Weight = newWeight,
                        Date = DateTime.UtcNow.Date
                    };
                    await _context.WeightHistories.AddAsync(Weight);
                }
                else
                {
                    Weight.Weight = newWeight;
                    _context.WeightHistories.Update(Weight);
                }
                await _context.SaveChangesAsync();
                return Ok("Weight updated");
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("GetWeightHistory/{dateFrom}/{dateTo}")]
        [Authorize]
        public async Task<IActionResult> GetWeightHistory(DateTime dateFrom, DateTime dateTo)
        {
            int userId = -1;
            bool succes = int.TryParse(User.FindFirst("UserID")?.Value, out userId);
            if (succes == false || userId < 0)
            {
                return BadRequest(new { message = "Error occured while reading userID" });
            }
            WeightHistory? data = null;
            try
            {
                data = await _context.WeightHistories
                    .Where(wh => wh.UserId == userId && wh.Date >= dateFrom && wh.Date <= dateTo)
                    .OrderByDescending(wh => wh.Date)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }

            if (data == null)
            {
                return NotFound(new { message = "Can't find weight history in this range" });
            }

            // Mapowanie na DTO
            var response = new WeightHistoryDTO
            {
                Date = data.Date,
                Weight = data.Weight
            };

            return Ok(response);
        }
    }
}

