using System.ComponentModel.DataAnnotations;

namespace GymFitnessTracker.Models.Domain
{
    public class ResetPasswordRequest
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }       
        public string Pin { get; set; }
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}
