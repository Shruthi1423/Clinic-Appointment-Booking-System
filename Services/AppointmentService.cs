using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinicApp.Models;
using ClinicApp.Repositories;
using Microsoft.EntityFrameworkCore;
using ClinicApp.Data;

namespace ClinicApp.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly AppDbContext _dbContext; // Add this, correct namespace!
        private readonly int _appointmentDuration = 60;

        public AppointmentService(IDoctorRepository doctorRepository, IPatientRepository patientRepository, IAppointmentRepository appointmentRepository, AppDbContext dbContext) //modify this
        {
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
            _appointmentRepository = appointmentRepository;
            _dbContext = dbContext; // Initialize
        }

        public async Task<IEnumerable<Doctor>> GetDoctorsAsync()
        {
            return await _doctorRepository.GetAllAsync();
        }

        public async Task<IEnumerable<DateTime>> GetAvailableTimesAsync(int doctorId, DateTime date)
        {
            var doctor = await _doctorRepository.GetByIdAsync(doctorId);
            if (doctor == null)
            {
                return new List<DateTime>();
            }

            DateTime startTime = date.Date.AddHours(9);
            DateTime endTime = date.Date.AddHours(17);
            List<DateTime> availableTimes = new List<DateTime>();

            var bookedAppointments = await _appointmentRepository.GetQueryable()
              .Where(a => a.DoctorId == doctorId && a.AppointmentDateTime.Date == date.Date)
              .OrderBy(a => a.AppointmentDateTime)
              .ToListAsync();

            DateTime currentTime = startTime;
            while (currentTime <= endTime.AddMinutes(-_appointmentDuration))
            {
                bool isOverlapping = bookedAppointments.Any(existingAppointment =>
                  currentTime < existingAppointment.AppointmentDateTime.AddMinutes(existingAppointment.DurationInMinutes) &&
                  currentTime.AddMinutes(_appointmentDuration) > existingAppointment.AppointmentDateTime
                );

                if (!isOverlapping)
                {
                    availableTimes.Add(currentTime);
                }
                currentTime = currentTime.AddMinutes(30);
            }
            return availableTimes;
        }

        public async Task<Appointment> BookAppointmentAsync(int doctorId, int patientId, DateTime appointmentDateTime, int durationInMinutes)
        {
            if (appointmentDateTime < DateTime.Now)
            {
                throw new ArgumentException("Appointment time cannot be in the past.");
            }

            var doctor = await _doctorRepository.GetByIdAsync(doctorId);
            var patient = await _patientRepository.GetByIdAsync(patientId);

            if (doctor == null || patient == null)
            {
                throw new ArgumentException("Invalid doctor or patient ID.");
            }
            bool isOverlapping = await _appointmentRepository.GetQueryable().AnyAsync(a =>
              a.DoctorId == doctorId &&
              a.AppointmentDateTime.Date == appointmentDateTime.Date &&
              a.AppointmentDateTime < appointmentDateTime.AddMinutes(durationInMinutes) &&
              a.AppointmentDateTime.AddMinutes(a.DurationInMinutes) > appointmentDateTime
            );

            if (isOverlapping)
            {
                throw new Exception("Time slot is already booked.");
            }

            var appointment = new Appointment
            {
                DoctorId = doctorId,
                PatientId = patientId,
                AppointmentDateTime = appointmentDateTime,
                DurationInMinutes = durationInMinutes
            };

            await _appointmentRepository.AddAsync(appointment);
            await _appointmentRepository.SaveChangesAsync();
            return appointment;
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsAsync()
        {
            return await _appointmentRepository.GetQueryable()
              .Include(a => a.Doctor)
              .Include(a => a.Patient)
              .ToListAsync();
        }

        public async Task CancelAppointmentAsync(int appointmentId)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
            if (appointment != null)
            {
                _appointmentRepository.Delete(appointment);
                await _appointmentRepository.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("Appointment not found.");
            }
        }

        public async Task<IEnumerable<Patient>> GetPatientsAsync()
        {
            return await _patientRepository.GetAllAsync();
        }

        public async Task<Patient> GetPatientByFileNoAsync(string fileNo)
        {
            return await _patientRepository.GetPatientByFileNoAsync(fileNo);
        }

        public async Task<Patient> CreatePatientAsync(Patient newPatient)
        {
            //  Implement the logic to create a new patient in the database
            _dbContext.Patients.Add(newPatient);
            await _dbContext.SaveChangesAsync();
            return newPatient; // Or fetch from database if PatientId is generated by DB
        }
    }
}
