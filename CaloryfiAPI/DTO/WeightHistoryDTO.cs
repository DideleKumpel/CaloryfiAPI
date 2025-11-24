using System.ComponentModel.DataAnnotations;

namespace CaloryfiAPI.DTO;

public class WeightHistoryDTO
{
    [Required]
    public DateTime Date { get; set; }

    [Required]
    [Range(1, 500)]
    public int Weight { get; set; }
}
