using System.ComponentModel.DataAnnotations;

namespace GymFitnessTracker.Models.Domain
{
    public class VerifyPinRequest
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Pin { get; set; }
    }
}
