using System.ComponentModel.DataAnnotations;

namespace GymFitnessTracker.Models.Domain
{
    public class RegisterUserRequest
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Username { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string InAppName { get; set; }
        public string[] Roles { get; set; }
        public string Gender { get; set; }
        public IFormFile? ProfilePicture { get; set; }
    }
}
