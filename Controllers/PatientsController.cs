using Microsoft.AspNetCore.Mvc;
using ClinicApp.Services;  // Replace with your actual namespace
using System.Threading.Tasks;
using ClinicApp.Models;

namespace ClinicApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public PatientsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPatients()
        {
            var patients = await _appointmentService.GetPatientsAsync();
            return Ok(patients);
        }

        [HttpGet("{fileNo}")]
        public async Task<IActionResult> GetPatientByFileNo(string fileNo)
        {
            var patient = await _appointmentService.GetPatientByFileNoAsync(fileNo);
            if (patient == null)
            {
                return NotFound("Patient not found.");
            }
            return Ok(patient);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePatient([FromBody] Patient newPatient) // [FromBody] attribute is important
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return 400 if the model is invalid
            }

            try
            {
                var createdPatient = await _appointmentService.CreatePatientAsync(newPatient);
                //  Return 201 Created with the new patient in the response
                return CreatedAtAction(nameof(GetPatientByFileNo), new { fileNo = createdPatient.FileNo }, createdPatient);
            }
            catch (Exception ex)
            {
                //  Log the error!
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
