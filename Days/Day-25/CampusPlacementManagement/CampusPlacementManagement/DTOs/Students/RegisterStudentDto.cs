using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.DTOs.Students;

public class RegisterStudentDto
{
    [Required]
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Phone]
    [MaxLength(20)]
    public string MobileNumber { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string RegisterNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Department { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Degree { get; set; } = string.Empty;

    [Range(2000, 2100)]
    public int GraduationYear { get; set; }

    [Range(typeof(decimal), "0", "10")]
    public decimal CGPA { get; set; }

    [Range(0, 100)]
    public int ActiveBacklogs { get; set; }

    [MaxLength(500)]
    public string? ResumeUrl { get; set; }
}
