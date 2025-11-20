namespace CaloryfiAPI.DTO;

public class IngredientDTO
{
    public int Id { get; set; }
    public string Name { get; set; }

    public int Kcal { get; set; }
    public int Carbs { get; set; }
    public int Proteins { get; set; }
    public int Fats { get; set; }

    public double Weight { get; set; }
}
