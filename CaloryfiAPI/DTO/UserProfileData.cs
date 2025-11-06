using CaloryfiAPI.Models;

namespace CaloryfiAPI.DTO
{
    public class UserProfileData
    {
        public int Id { get; set; }

        public string Email { get; set; } = null!;

        public string Username { get; set; } = null!;

        public UserProfileData(User User)
        {
            Id = User.Id;
            Email = User.Email;
            Username = User.Username;
        }
    }
}
