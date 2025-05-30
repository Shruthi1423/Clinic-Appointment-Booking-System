using ClinicApp.Models;
using System.Threading.Tasks;
namespace ClinicApp.Repositories
{
    public interface IDoctorRepository : IRepository<Doctor>
    {
    }
    public interface IPatientRepository : IRepository<Patient>
    {
        Task<Patient> GetPatientByFileNoAsync(string fileNo);
    }

    public interface IAppointmentRepository : IRepository<Appointment>
    {
    }
}