using CaloryfiAPI.DTO;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace CaloryfiAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeminiApiController : Controller
    {
        private readonly string apiKey;

        public GeminiApiController(IConfiguration configuration)
        {
            apiKey = configuration["AiApiKey:Key"];
        }

        [Authorize]
        [HttpGet("AutoCalculateIngredientmMakro/{ingredientName}")]
        public async Task<IActionResult> AutoCalculateIngredientmMakro(string ingredientName)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return StatusCode(500, "Gemini API key is not configured.");
            }
            if (string.IsNullOrWhiteSpace(ingredientName))
            {
                return BadRequest("Ingredient name is required.");
            }
            try
            {
                var client = new Client(apiKey: apiKey);
                string prompt = $"Provide the nutritional information (kcal, carbs, proteins, fats) for the ingredient: {ingredientName}. Respond in JSON format nubers write as int.{{ \"Kcal\": , \"Carbs\": ,\"Proteins\": ,\"Fats\": }}";
                var response = await client.Models.GenerateContentAsync(
                  model: "gemini-2.5-flash-lite", contents: prompt
                );
                Console.WriteLine(response.Candidates[0].Content.Parts[0].Text);

                string objectString = response.Candidates[0].Content.Parts[0].Text;
                objectString = objectString.Replace("```json", "").Replace("```", "").Trim();

                IngredientDTO newIngredeint = JsonConvert.DeserializeObject<IngredientDTO>(objectString);
                newIngredeint.Name = ingredientName;

                return Ok(newIngredeint);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
            }
        }
    }
}
