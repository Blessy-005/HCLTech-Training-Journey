
using System.ComponentModel.DataAnnotations;

namespace DoctorDirectory.Models
{
    public class Doctor
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Specialization { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        public int YearsOfExperience { get; set; }

        [Range(typeof(decimal), "0", "999999999")]
        public decimal ConsultationFee { get; set; }
    }
}
