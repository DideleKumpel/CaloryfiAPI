using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaloryfiAPI.Models;

public class Meal
{
    [Key]
    public int Id { get; set; }

    public DateTime Date_Added { get; set; }

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    // Navigation properties
    public virtual User User { get; set; }
    public virtual ICollection<MealComponent> MealComponents { get; set; }
}
