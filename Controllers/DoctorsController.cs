using Microsoft.AspNetCore.Mvc;
using ClinicApp.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public DoctorsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDoctors()
        {
            var doctors = await _appointmentService.GetDoctorsAsync();
            return Ok(doctors);
        }

        [HttpGet("{doctorId}/availabletimes")]
        public async Task<IActionResult> GetAvailableTimes(int doctorId, [FromQuery] DateTime date)
        {
            try
            {
                var availableTimes = await _appointmentService.GetAvailableTimesAsync(doctorId, date);
                return Ok(availableTimes);
            }
            catch (ArgumentException)
            {
                return NotFound("Doctor not found.");
            }
        }
    }
}