namespace CaloryfiAPI.DTO
{
    public class UserSettingsDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public bool Sex { get; set; }
        public int NumberOfMeals { get; set; }
        public int DietGoal { get; set; }
        public int ActivityLevel { get; set; }
        public decimal Kcal { get; set; }
        public decimal Carbs { get; set; }
        public decimal Proteins { get; set; }
        public decimal Fats { get; set; }
    }
}
