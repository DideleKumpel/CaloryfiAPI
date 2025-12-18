using CaloryfiAPI.DTO;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Management;
using static System.Net.Mime.MediaTypeNames;

namespace CaloryfiAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeminiApiController : Controller
    {
        private readonly string apiKey;

        private static readonly HashSet<string> allowedExtensions = new HashSet<string>
        {
        ".jpg", ".jpeg", ".png"
        };

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

        [Authorize]
        [HttpPost("GetFoodFromImage")]
        public async Task<IActionResult> GetFoodFromImage([FromBody] FoodFormImageDTO foodFormImageDTO)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return StatusCode(500, "Gemini API key is not configured.");
            }
            if (foodFormImageDTO == null || string.IsNullOrWhiteSpace(foodFormImageDTO.Name) || foodFormImageDTO.Weight <= 0)
            {
                return BadRequest("Invalid food data.");
            }
            
            try
            {
                var client = new Client(apiKey: apiKey);
                var response = new Google.GenAI.Types.GenerateContentResponse();
                if (foodFormImageDTO.Image == null || foodFormImageDTO.Image.Data == null || foodFormImageDTO.Image.Data.Length == 0)
                {
                    string prompt = $"Provide the nutritional information for 100grams (kcal, carbs, proteins, fats) for the meal: {foodFormImageDTO.Name}. Respond in JSON format nubers write as int.{{ \"Kcal\": , \"Carbs\": ,\"Proteins\": ,\"Fats\": }}";
                    response = await client.Models.GenerateContentAsync(
                            model: "gemini-2.5-flash-lite", contents: prompt
                    );
                }
                else
                {
                    if (!allowedExtensions.Contains(foodFormImageDTO.Image.Extension))
                    {
                        return BadRequest("Wrong image extension.");
                    }
                    string extension = foodFormImageDTO.Image.Extension.TrimStart('.').ToLower();
                    string mimeType = extension switch
                    {
                        "png" => "image/png",
                        "jpeg" => "image/jpeg",
                        "jpg" => "image/jpeg",
                        _ => "image/jpeg"
                    };

                    string prompt = $"Provide the nutritional information for 100grams (kcal, carbs, proteins, fats) for the meal: {foodFormImageDTO.Name} use photo atached to message. Respond in JSON format nubers write as int.{{ \"Kcal\": , \"Carbs\": ,\"Proteins\": ,\"Fats\": }}";
                    response = await client.Models.GenerateContentAsync(
                        model: "gemini-2.5-flash-lite",
                        contents: new List<Content> {
                        new Content {
                            Parts = new List<Part> {
                                new Part { InlineData = new Blob { MimeType = mimeType, Data = foodFormImageDTO.Image.Data } },
                                new Part { Text = prompt }
                            }
                        }
                        }
                    );
                }
                Console.WriteLine(response.Candidates[0].Content.Parts[0].Text);
                string objectString = response.Candidates[0].Content.Parts[0].Text;
                objectString = objectString.Replace("```json", "").Replace("```", "").Trim();
                var result = JsonConvert.DeserializeObject<IngredientDTO>(objectString);
                result.Name = foodFormImageDTO.Name;
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
            }
        }
    }
}
