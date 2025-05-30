using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ClinicApp.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string FileNo { get; set; } 

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}