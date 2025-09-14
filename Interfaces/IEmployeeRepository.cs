using MVC_CRUD.DTOs;

namespace MVC_CRUD.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<saveEmployeeDTO?> SaveEmployeeAsync(saveEmployeeDTO employee);
        Task<updateEmployeeDTO?> UpdateEmployeeAsync(updateEmployeeDTO updateEmployee);
        Task<List<saveEmployeeDTO>> GetEmployeesAsync();
        Task<saveEmployeeDTO?> GetEmployeeByIdAsync(int empId);
        Task<bool> DeleteEmployeeAsync(int id);
        Task<bool> EmailExistsAsync(string email, int empId);
    }
}
