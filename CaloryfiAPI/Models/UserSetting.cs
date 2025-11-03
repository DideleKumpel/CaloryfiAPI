using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaloryfiAPI.Models;

public class UserSetting
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    public bool Sex { get; set; }
    public int NumberOfMeals { get; set; }
    public int DietGoal { get; set; }
    public int ActivityLevel { get; set; }
    public decimal Kcal { get; set; }
    public decimal Carbs { get; set; }
    public decimal Proteins { get; set; }
    public decimal Fats { get; set; }

    // Navigation property
    public virtual User User { get; set; }
}
