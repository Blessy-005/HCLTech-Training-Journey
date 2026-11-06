namespace CampusPlacementManagement.DTOs.CompanyHRs;

public class CompanyHRResponseDto
{
    public int CompanyHRId { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string MobileNumber { get; set; } = string.Empty;

    public int CompanyId { get; set; }

    public string CompanyName { get; set; } = string.Empty;

    public string Designation { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }
}