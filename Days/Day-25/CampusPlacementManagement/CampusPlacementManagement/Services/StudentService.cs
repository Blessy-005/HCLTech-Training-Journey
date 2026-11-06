using CampusPlacementManagement.DTOs.Students;
using CampusPlacementManagement.Interfaces.Repositories;
using CampusPlacementManagement.Interfaces.Services;
using CampusPlacementManagement.Models.Entities;
using CampusPlacementManagement.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace CampusPlacementManagement.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IPlacementOfficerRepository _placementOfficerRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public StudentService(
        IStudentRepository studentRepository,
        IPlacementOfficerRepository placementOfficerRepository,
        UserManager<ApplicationUser> userManager)
    {
        _studentRepository = studentRepository;
        _placementOfficerRepository = placementOfficerRepository;
        _userManager = userManager;
    }

    public async Task<StudentResponseDto> EnrollAsync(
        RegisterStudentDto dto,
        string placementOfficerUserId)
    {
        var officer = await GetApprovedOfficerAsync(placementOfficerUserId);

        var email = dto.Email.Trim();

        if (await _userManager.FindByEmailAsync(email) != null)
            throw new InvalidOperationException(
                "An account with this email already exists.");

        var existingStudent =
            await _studentRepository.GetByRegisterNumberAsync(
                officer.CollegeId, dto.RegisterNumber.Trim());

        if (existingStudent != null)
            throw new InvalidOperationException(
                "This register number already exists in your college.");

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            PhoneNumber = dto.MobileNumber.Trim(),
            FullName = dto.FullName.Trim(),
            AccountStatus = AccountStatus.Pending,
            EmailConfirmed = false
        };

        var identityResult = await _userManager.CreateAsync(
            user, dto.Password);

        if (!identityResult.Succeeded)
        {
            var errors = string.Join(
                "; ",
                identityResult.Errors.Select(e => e.Description));

            throw new InvalidOperationException(
                $"Student account creation failed: {errors}");
        }

        var student = new Student
        {
            UserId = user.Id,
            CollegeId = officer.CollegeId,
            PlacementOfficerId = officer.PlacementOfficerId,
            RegisterNumber = dto.RegisterNumber.Trim(),
            Department = dto.Department.Trim(),
            Degree = dto.Degree.Trim(),
            GraduationYear = dto.GraduationYear,
            CGPA = dto.CGPA,
            ActiveBacklogs = dto.ActiveBacklogs,
            ResumeUrl = dto.ResumeUrl?.Trim(),
            Status = StudentStatus.Pending
        };

        try
        {
            await _studentRepository.AddAsync(student);
        }
        catch
        {
            await _userManager.DeleteAsync(user);
            throw;
        }

        var savedStudent =
            await _studentRepository.GetByIdAsync(student.StudentId);

        return MapToResponse(savedStudent!);
    }

    public async Task<IEnumerable<StudentResponseDto>> GetAllAsync(
        string placementOfficerUserId)
    {
        var officer = await GetApprovedOfficerAsync(placementOfficerUserId);

        var students =
            await _studentRepository.GetByCollegeAsync(officer.CollegeId);

        return students.Select(MapToResponse);
    }

    public async Task<IEnumerable<StudentResponseDto>> GetPendingAsync(
        string placementOfficerUserId)
    {
        var officer = await GetApprovedOfficerAsync(placementOfficerUserId);

        var students =
            await _studentRepository.GetPendingByCollegeAsync(
                officer.CollegeId);

        return students.Select(MapToResponse);
    }

    public async Task<StudentResponseDto> GetByIdAsync(
        int id,
        string placementOfficerUserId)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(id), "Student ID must be greater than zero.");

        var officer = await GetApprovedOfficerAsync(placementOfficerUserId);
        var student = await _studentRepository.GetByIdAsync(id);

        if (student == null || student.CollegeId != officer.CollegeId)
            throw new KeyNotFoundException("Student not found.");

        return MapToResponse(student);
    }

    public async Task<StudentResponseDto> ApproveAsync(
        int id,
        string placementOfficerUserId)
    {
        var officer = await GetApprovedOfficerAsync(placementOfficerUserId);
        var student = await GetStudentForOfficerAsync(id, officer.CollegeId);

        if (student.Status != StudentStatus.Pending)
            throw new InvalidOperationException(
                "Only pending students can be approved.");

        var user = await _userManager.FindByIdAsync(student.UserId);

        if (user == null)
            throw new InvalidOperationException(
                "Student user account not found.");

        var roleResult = await _userManager.AddToRoleAsync(user, "Student");

        if (!roleResult.Succeeded)
        {
            var errors = string.Join(
                "; ",
                roleResult.Errors.Select(e => e.Description));

            throw new InvalidOperationException(
                $"Student role assignment failed: {errors}");
        }

        user.AccountStatus = AccountStatus.Approved;
        user.EmailConfirmed = true;
        user.UpdatedAt = DateTime.UtcNow;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            await _userManager.RemoveFromRoleAsync(user, "Student");

            var errors = string.Join(
                "; ",
                updateResult.Errors.Select(e => e.Description));

            throw new InvalidOperationException(
                $"Student account approval failed: {errors}");
        }

        student.Status = StudentStatus.Approved;
        student.ApprovedAt = DateTime.UtcNow;

        await _studentRepository.UpdateAsync(student);

        return MapToResponse(student);
    }

    public async Task<StudentResponseDto> RejectAsync(
        int id,
        string placementOfficerUserId)
    {
        var officer = await GetApprovedOfficerAsync(placementOfficerUserId);
        var student = await GetStudentForOfficerAsync(id, officer.CollegeId);

        if (student.Status != StudentStatus.Pending)
            throw new InvalidOperationException(
                "Only pending students can be rejected.");

        var user = await _userManager.FindByIdAsync(student.UserId);

        if (user == null)
            throw new InvalidOperationException(
                "Student user account not found.");

        user.AccountStatus = AccountStatus.Rejected;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            throw new InvalidOperationException(
                "Failed to update student account status.");

        student.Status = StudentStatus.Rejected;

        await _studentRepository.UpdateAsync(student);

        return MapToResponse(student);
    }

    public async Task<StudentResponseDto> SuspendAsync(
        int id,
        string placementOfficerUserId)
    {
        var officer = await GetApprovedOfficerAsync(placementOfficerUserId);
        var student = await GetStudentForOfficerAsync(id, officer.CollegeId);

        if (student.Status != StudentStatus.Approved)
            throw new InvalidOperationException(
                "Only approved students can be suspended.");

        var user = await _userManager.FindByIdAsync(student.UserId);

        if (user == null)
            throw new InvalidOperationException(
                "Student user account not found.");

        user.AccountStatus = AccountStatus.Suspended;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            throw new InvalidOperationException(
                "Failed to suspend student account.");

        student.Status = StudentStatus.Suspended;

        await _studentRepository.UpdateAsync(student);

        return MapToResponse(student);
    }

    public async Task<StudentResponseDto> GetCurrentStudentAsync(
        string userId)
    {
        var student = await _studentRepository.GetByUserIdAsync(userId);

        if (student == null)
            throw new KeyNotFoundException("Student profile not found.");

        if (student.Status != StudentStatus.Approved ||
            student.User.AccountStatus != AccountStatus.Approved ||
            student.College.Status != CollegeStatus.Approved)
        {
            throw new UnauthorizedAccessException(
                "Your student account or college is not currently approved.");
        }

        return MapToResponse(student);
    }

    private async Task<PlacementOfficer> GetApprovedOfficerAsync(
        string userId)
    {
        var officer =
            await _placementOfficerRepository.GetByUserIdAsync(userId);

        if (officer == null)
            throw new UnauthorizedAccessException(
                "Placement Officer profile not found.");

        if (officer.Status != PlacementOfficerStatus.Approved ||
            officer.User.AccountStatus != AccountStatus.Approved ||
            officer.College.Status != CollegeStatus.Approved)
        {
            throw new UnauthorizedAccessException(
                "Your account or college is not currently approved.");
        }

        return officer;
    }

    private async Task<Student> GetStudentForOfficerAsync(
        int id,
        int collegeId)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(id), "Student ID must be greater than zero.");

        var student = await _studentRepository.GetByIdAsync(id);

        if (student == null || student.CollegeId != collegeId)
            throw new KeyNotFoundException("Student not found.");

        return student;
    }

    private static StudentResponseDto MapToResponse(Student student)
    {
        return new StudentResponseDto
        {
            StudentId = student.StudentId,
            FullName = student.User.FullName,
            Email = student.User.Email ?? string.Empty,
            MobileNumber = student.User.PhoneNumber ?? string.Empty,
            CollegeId = student.CollegeId,
            CollegeName = student.College.CollegeName,
            PlacementOfficerId = student.PlacementOfficerId,
            RegisterNumber = student.RegisterNumber,
            Department = student.Department,
            Degree = student.Degree,
            GraduationYear = student.GraduationYear,
            CGPA = student.CGPA,
            ActiveBacklogs = student.ActiveBacklogs,
            ResumeUrl = student.ResumeUrl,
            Status = student.Status,
            ApprovedAt = student.ApprovedAt
        };
    }
}
