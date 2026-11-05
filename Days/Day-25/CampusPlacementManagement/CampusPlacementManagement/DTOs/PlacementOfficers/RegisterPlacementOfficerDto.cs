using System.ComponentModel.DataAnnotations;

namespace CampusPlacementManagement.DTOs.PlacementOfficers;

public class RegisterPlacementOfficerDto
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
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int CollegeId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Designation { get; set; } = string.Empty;
}
