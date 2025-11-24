namespace CaloryfiAPI.DTO;

public class MealDTO
{
    public int Id { get; set; }
    public DateTime DateAdded { get; set; }

    public List<IngredientDTO> Ingredients { get; set; }
}
