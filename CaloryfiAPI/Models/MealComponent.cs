using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaloryfiAPI.Models;

public class MealComponent
{
    [Key, Column(Order = 0)]
    [ForeignKey(nameof(Meal))]
    public int MealId { get; set; }

    [Key, Column(Order = 1)]
    [ForeignKey(nameof(Ingredient))]
    public int IngredientId { get; set; }

    public double Weight { get; set; }

    // Navigation properties
    public virtual Meal Meal { get; set; }
    public virtual Ingredient Ingredient { get; set; }
}
