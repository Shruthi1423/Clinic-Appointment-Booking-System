namespace ClinicAppointmentAPI.DTOs
{
    public class AppointmentRequestDto
    {
        public string PatientName { get; set; }
        public string PhoneNumber { get; set; }
        public string FileNo { get; set; }

        public int DoctorId { get; set; }
        public DateTime AppointmentDateTime { get; set; }
        public int DurationInMinutes { get; set; }
    }
}
