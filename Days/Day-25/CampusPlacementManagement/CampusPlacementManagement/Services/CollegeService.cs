using CampusPlacementManagement.DTOs.Colleges;
using CampusPlacementManagement.Interfaces.Repositories;
using CampusPlacementManagement.Interfaces.Services;
using CampusPlacementManagement.Models.Entities;
using CampusPlacementManagement.Models.Enums;

namespace CampusPlacementManagement.Services;

public class CollegeService : ICollegeService
{
    private readonly ICollegeRepository _collegeRepository;

    public CollegeService(ICollegeRepository collegeRepository)
    {
        _collegeRepository = collegeRepository;
    }

    public async Task<College?> GetByIdAsync(int collegeId)
    {
        if (collegeId <= 0)
            throw new ArgumentOutOfRangeException(nameof(collegeId));

        return await _collegeRepository.GetByIdAsync(collegeId);
    }

    public async Task<List<College>> GetAllAsync()
    {
        return await _collegeRepository.GetAllAsync();
    }

    public async Task<College> CreateAsync(CreateCollegeDto dto)
    {
        var collegeCode = dto.CollegeCode.Trim();

        var existingCollege =
            await _collegeRepository.GetByCodeAsync(collegeCode);

        if (existingCollege != null)
            throw new InvalidOperationException(
                "A college with this code already exists.");

        var college = new College
        {
            CollegeName = dto.CollegeName.Trim(),
            CollegeCode = collegeCode,
            UniversityName = dto.UniversityName?.Trim(),
            OfficialEmail = dto.OfficialEmail.Trim(),
            PhoneNumber = dto.PhoneNumber.Trim(),
            Address = dto.Address.Trim(),
            Website = dto.Website?.Trim()
        };

        await _collegeRepository.AddAsync(college);

        return college;
    }

    public async Task<bool> UpdateAsync(
        int collegeId,
        UpdateCollegeDto dto)
    {
        if (collegeId <= 0)
            throw new ArgumentOutOfRangeException(nameof(collegeId));

        var college = await _collegeRepository.GetByIdAsync(collegeId);

        if (college == null)
            return false;

        var collegeCode = dto.CollegeCode.Trim();

        var existingCollege =
            await _collegeRepository.GetByCodeAsync(collegeCode);

        if (existingCollege != null &&
            existingCollege.CollegeId != collegeId)
        {
            throw new InvalidOperationException(
                "Another college with this code already exists.");
        }

        college.CollegeName = dto.CollegeName.Trim();
        college.CollegeCode = collegeCode;
        college.UniversityName = dto.UniversityName?.Trim();
        college.OfficialEmail = dto.OfficialEmail.Trim();
        college.PhoneNumber = dto.PhoneNumber.Trim();
        college.Address = dto.Address.Trim();
        college.Website = dto.Website?.Trim();

        await _collegeRepository.UpdateAsync(college);

        return true;
    }

    public async Task<bool> UpdateStatusAsync(
        int collegeId,
        UpdateCollegeStatusDto dto)
    {
        if (collegeId <= 0)
            throw new ArgumentOutOfRangeException(nameof(collegeId));

        if (!Enum.IsDefined(typeof(CollegeStatus), dto.Status))
            throw new ArgumentException("Invalid college status.");

        var college = await _collegeRepository.GetByIdAsync(collegeId);

        if (college == null)
            return false;

        college.Status = dto.Status;

        await _collegeRepository.UpdateAsync(college);

        return true;
    }
}
