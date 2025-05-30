using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using ClinicApp.Data;
using ClinicApp.Models;
using ClinicApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Emit;

namespace ClinicApp.Models
{
    public class Doctor
    {
        [Key]
        public int DoctorId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Specialization { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}