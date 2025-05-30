using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClinicApp.Models;
using ClinicApp.Repositories; 
namespace ClinicApp.Services
{
    public interface IAppointmentService
    {
        Task<IEnumerable<Doctor>> GetDoctorsAsync();
        Task<IEnumerable<DateTime>> GetAvailableTimesAsync(int doctorId, DateTime date);
        Task<Appointment> BookAppointmentAsync(int doctorId, int patientId, DateTime appointmentDateTime, int durationInMinutes);
        Task<IEnumerable<Appointment>> GetAppointmentsAsync();
        Task CancelAppointmentAsync(int appointmentId);
        Task<IEnumerable<Patient>> GetPatientsAsync();
        Task<Patient> GetPatientByFileNoAsync(string fileNo);
        Task<Patient> CreatePatientAsync(Patient newPatient); // Add this line
    }
}