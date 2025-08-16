using System.ComponentModel.DataAnnotations;

namespace GymFitnessTracker.Models.Domain
{
    public class EmailRequest
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
