using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MVC_CRUD.Data;
using MVC_CRUD.DTOs;
using MVC_CRUD.Interfaces;
using MVC_CRUD.Models;

namespace MVC_CRUD.Repositories
{
    public class EmployeeRepository: IEmployeeRepository
    {
        private readonly AppDbContext _dbcontext;
        private readonly ILogger<EmployeeRepository> _logger;
        
        public  EmployeeRepository(AppDbContext dbcontext, ILogger<EmployeeRepository> logger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
        }
        public async Task<saveEmployeeDTO?> SaveEmployeeAsync(saveEmployeeDTO employee)
        {
            try
            {
                var emp = new Employee
                {
                    empName = employee.empName,
                    empEmail = employee.empEmail,
                    empDepartment = employee.empDepartment,
                    empSalary = employee.empSalary
                };

                await _dbcontext.Employee.AddAsync(emp);
                var result = await _dbcontext.SaveChangesAsync();

                if (result > 0)
                {
                    

                    // Return the newly added employee as DTO including the generated ID
                    return new saveEmployeeDTO
                    {
                        empId = emp.empId,
                        empName = emp.empName,
                        empEmail = emp.empEmail,
                        empDepartment = emp.empDepartment,
                        empSalary = emp.empSalary
                    };
                }

                _logger.LogWarning("Employee not saved.");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving employee {EmpName}", employee.empName);
                throw;
            }
        }


        public async Task<updateEmployeeDTO?> UpdateEmployeeAsync(updateEmployeeDTO updateEmployee)
        {
            try
            {
                // Find the employee by ID
                var emp = await _dbcontext.Employee.FindAsync(updateEmployee.empId);
                if (emp == null)
                {
                    _logger.LogWarning("Employee with ID {EmpId} not found for update.", updateEmployee.empId);
                    return null; // Return null if employee doesn't exist
                }

                // Update properties
                emp.empName = updateEmployee.empName;
                emp.empEmail = updateEmployee.empEmail;
                emp.empDepartment = updateEmployee.empDepartment;
                emp.empSalary = updateEmployee.empSalary;

                // Save changes
                var result = await _dbcontext.SaveChangesAsync();
                if (result > 0)
                {
                    _logger.LogInformation("Employee with ID {EmpId} updated successfully.", updateEmployee.empId);

                    // Fetch the latest updated employee from DB
                    var updatedEmp = await _dbcontext.Employee
                                            .AsNoTracking()
                                            .FirstOrDefaultAsync(e => e.empId == updateEmployee.empId);

                    if (updatedEmp == null) return null;

                    // Map entity to DTO
                    var updatedDTO = new updateEmployeeDTO
                    {
                        empId = updatedEmp.empId,
                        empName = updatedEmp.empName,
                        empEmail = updatedEmp.empEmail,
                        empDepartment = updatedEmp.empDepartment,
                        empSalary = updatedEmp.empSalary
                    };

                    return updatedDTO;
                }

                _logger.LogWarning("Employee with ID {EmpId} not updated.", updateEmployee.empId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating employee {EmpId}", updateEmployee.empId);
                throw;
            }
        }

        public async Task<List<saveEmployeeDTO>> GetEmployeesAsync()
        {
            try
            {
                return await _dbcontext.Employee
                    .AsNoTracking()
                    .Select(emp => new saveEmployeeDTO
                    {
                        empId = emp.empId,
                        empName = emp.empName,
                        empEmail = emp.empEmail,
                        empDepartment = emp.empDepartment,
                        empSalary = emp.empSalary
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching employees.");
                throw;
            }
        }


        public async Task<saveEmployeeDTO?> GetEmployeeByIdAsync(int empId)
        {
            try
            {
                var emp = await _dbcontext.Employee
                                          .AsNoTracking() // efficient for read-only
                                          .Where(e => e.empId == empId)
                                          .Select(e => new saveEmployeeDTO
                                          {
                                              empId = e.empId,
                                              empName = e.empName,
                                              empEmail = e.empEmail,
                                              empDepartment = e.empDepartment,
                                              empSalary = e.empSalary
                                          })
                                          .FirstOrDefaultAsync();

                if (emp == null)
                {
                    _logger.LogWarning("Employee with ID {EmpId} not found.", emp);
                }
                else
                {
                    _logger.LogInformation("Employee with ID {EmpId} fetched successfully.", emp);
                }

                return emp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching employee {EmpId}.", empId);
                throw;
            }
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            try
            {
                var emp = await _dbcontext.Employee.FindAsync(id);
                if (emp == null)
                {
                    _logger.LogWarning("Employee with ID {EmpId} not found for deletion.", id);
                    return false;
                }
                _dbcontext.Employee.Remove(emp);
                var result = await _dbcontext.SaveChangesAsync() > 0;
                if (result)
                    _logger.LogInformation("Employee with ID {EmpId} deleted successfully.", id);
                else
                    _logger.LogWarning("Employee with ID {EmpId} could not be deleted.", id);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting employee {EmpId}.", id);
                throw;
            }
        }


        public async Task<bool> EmailExistsAsync(string email, int empId)
        {
            try
            {
                // check if any other employee has this email
                return await _dbcontext.Employee
                    .AnyAsync(e => e.empEmail == email && e.empId != empId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking email {Email} for EmpId {EmpId}", email, empId);
                throw;
            }
        }





    }
}
