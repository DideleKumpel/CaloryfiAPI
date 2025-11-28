using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaloryfiAPI.Models;

public class WeightHistory
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    public DateTime Date { get; set; }
    public int Weight { get; set; }

    // Navigation property
    public virtual User User { get; set; }


}
