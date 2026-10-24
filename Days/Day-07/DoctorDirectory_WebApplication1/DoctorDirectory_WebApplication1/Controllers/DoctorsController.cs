
using DoctorDirectory.Data;
using DoctorDirectory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoctorDirectory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly DoctorContext _context;

        public DoctorsController(DoctorContext context)
        {
            _context = context;
        }

        // GET: api/doctors?specialization=Cardiology
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctors(
            [FromQuery] string? specialization)
        {
            var query = _context.Doctors.AsQueryable();

            if (!string.IsNullOrWhiteSpace(specialization))
            {
                query = query.Where(d =>
                    d.Specialization.ToLower() ==
                    specialization.Trim().ToLower());
            }

            return Ok(await query.ToListAsync());
        }

        // GET: api/doctors/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Doctor>> GetDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);

            if (doctor == null)
                return NotFound();

            return Ok(doctor);
        }

        // POST: api/doctors
        [HttpPost]
        public async Task<ActionResult<Doctor>> CreateDoctor(Doctor doctor)
        {
            bool emailExists = await _context.Doctors
                .AnyAsync(d => d.Email.ToLower() == doctor.Email.ToLower());

            if (emailExists)
                return Conflict(new { message = "Email already exists." });

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetDoctor),
                new { id = doctor.Id },
                doctor);
        }

        // PUT: api/doctors/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctor(int id, Doctor doctor)
        {
            var existingDoctor = await _context.Doctors.FindAsync(id);

            if (existingDoctor == null)
                return NotFound();

            bool emailExists = await _context.Doctors.AnyAsync(d =>
                d.Email.ToLower() == doctor.Email.ToLower() &&
                d.Id != id);

            if (emailExists)
                return Conflict(new { message = "Email already exists." });

            existingDoctor.FullName = doctor.FullName;
            existingDoctor.Specialization = doctor.Specialization;
            existingDoctor.Email = doctor.Email;
            existingDoctor.YearsOfExperience = doctor.YearsOfExperience;
            existingDoctor.ConsultationFee = doctor.ConsultationFee;

            await _context.SaveChangesAsync();

            return Ok(existingDoctor);
        }

        // DELETE: api/doctors/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);

            if (doctor == null)
                return NotFound();

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
