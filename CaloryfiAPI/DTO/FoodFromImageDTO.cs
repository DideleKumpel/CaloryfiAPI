using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CaloryfiAPI.DTO
{
    public class FoodFormImageDTO
    {
        public string Name { get; set; }
        public double Weight { get; set; }
        public ImageModel Image { get; set; }
    }
}
