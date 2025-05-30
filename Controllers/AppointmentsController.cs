using Microsoft.AspNetCore.Mvc;
using ClinicApp.Services;
using System;
using System.Threading.Tasks;
using ClinicApp.Models;
using System.ComponentModel.DataAnnotations;

namespace ClinicApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpPost]
        public async Task<IActionResult> BookAppointment([FromBody] AppointmentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var appointment = await _appointmentService.BookAppointmentAsync(request.DoctorId, request.PatientId, request.AppointmentDateTime, request.DurationInMinutes);
                return CreatedAtAction(nameof(GetAppointment), new { id = appointment.AppointmentId }, appointment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); 
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message); 
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAppointments()
        {
            var appointments = await _appointmentService.GetAppointmentsAsync();
            return Ok(appointments);
        }

        [HttpGet("{id}", Name = "GetAppointment")] 
        public async Task<IActionResult> GetAppointment(int id)
        {
            var appointment = (await _appointmentService.GetAppointmentsAsync()).FirstOrDefault(a => a.AppointmentId == id); 
            if (appointment == null)
            {
                return NotFound();
            }
            return Ok(appointment);
        }

        [HttpDelete("{appointmentId}")]
        public async Task<IActionResult> CancelAppointment(int appointmentId)
        {
            try
            {
                await _appointmentService.CancelAppointmentAsync(appointmentId);
                return NoContent(); 
            }
            catch (ArgumentException)
            {
                return NotFound("Appointment not found.");
            }
        }
    }

    public class AppointmentRequest
    {
        [Required]
        public int DoctorId { get; set; }
        [Required]
        public int PatientId { get; set; }
        [Required]
        public DateTime AppointmentDateTime { get; set; }
        [Required]
        public int DurationInMinutes { get; set; }
    }
}