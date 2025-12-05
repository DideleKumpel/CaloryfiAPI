using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaloryfiAPI.Models;

public class Ingredient
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(255)]
    public string Name { get; set; }

    public int Kcal { get; set; }
    public int Carbs { get; set; }
    public int Proteins { get; set; }
    public int Fats { get; set; }

    [ForeignKey(nameof(User))]
    public int? UserId { get; set; }

    // Navigation properties
    public virtual User? User { get; set; }
    public virtual ICollection<MealComponent> MealComponents { get; set; }
}
