using System.ComponentModel.DataAnnotations;

namespace CaloryfiAPI.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(255)]
    public string Email { get; set; }

    [Required, MaxLength(255)]
    public string Password { get; set; }

    [Required, MaxLength(100)]
    public string Username { get; set; }

    // Navigation properties
    public virtual UserSetting UserSetting { get; set; }
    public virtual ICollection<WeightHistory> WeightHistory { get; set; }
    public virtual ICollection<Meal> Meals { get; set; }
    public virtual ICollection<Ingredient> Ingredients { get; set; }
}
