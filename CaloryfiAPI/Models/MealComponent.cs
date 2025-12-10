using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaloryfiAPI.Models;

public class MealComponent
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Meal))]
    public int MealId { get; set; }

    [ForeignKey(nameof(Ingredient))]
    public int IngredientId { get; set; }

    public double Quantity { get; set; } // opcjonalnie — gramatura, porcja itp.

    // Navigation
    public virtual Meal Meal { get; set; }
    public virtual Ingredient Ingredient { get; set; }
}
