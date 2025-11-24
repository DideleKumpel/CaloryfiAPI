using CaloryfiAPI.Data;
using CaloryfiAPI.DTO;
using CaloryfiAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CaloryfiAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeightHistoryController : Controller
{
    private readonly AppDatabaseContext _context;
    private readonly IConfiguration _configuration;

    public WeightHistoryController(AppDatabaseContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpGet("GetWeightHistory")]
    [Authorize]
    public async Task<IActionResult> GetWeightHistory(DateTime dateFrom, DateTime dateTo)
    {
        int userId;
        try
        {
            userId = GetUserId();
        }
        catch (UnauthorizedAccessException ex)
        {
            return BadRequest(new { message = ex.Message });
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


    [HttpPost("AddWeight")]
    [Authorize]
    public async Task<IActionResult> AddWeight([FromBody] WeightHistoryDTO request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        int userId;
        try
        {
            userId = GetUserId();
        }
        catch (UnauthorizedAccessException ex)
        {
            return BadRequest(new { message = ex.Message });
        }

        var newEntry = new WeightHistory
        {
            UserId = userId,
            Date = request.Date,
            Weight = request.Weight
        };

        try
        {
            _context.WeightHistories.Add(newEntry);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while saving weight history.");
        }

        var response = new WeightHistoryDTO
        {
            Date = newEntry.Date,
            Weight = newEntry.Weight
        };

        return Ok(response);
    }

    private int GetUserId()
    {
        var claim = User.FindFirst("UserID")?.Value;
        if (int.TryParse(claim, out int userId) && userId > 0)
            return userId;

        throw new UnauthorizedAccessException("Invalid userId.");
    }
}
