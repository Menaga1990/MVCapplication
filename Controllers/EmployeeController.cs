using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_CRUD.DTOs;
using MVC_CRUD.Interfaces;
using MVC_CRUD.Models;

namespace MVC_CRUD.Controllers
{
   
    [Route("/[controller]")]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        public  EmployeeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository= employeeRepository;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("GetAllEmployees")]

        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _employeeRepository.GetEmployeesAsync();

            if (employees == null || employees.Count == 0)
            {
                return Ok(new List<saveEmployeeDTO>()); // return empty array instead of NotFound
            }

            var employeeDtos = employees.Select(e => new saveEmployeeDTO
            {
                empId = e.empId,
                empName = e.empName,
                empEmail = e.empEmail,
                empDepartment = e.empDepartment,
                empSalary = e.empSalary
            }).ToList();

            return Ok(employeeDtos);
        }

        [HttpGet("GetEmployeeById/{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound($"Employee with ID {id} not found.");
            }
            return Ok(employee);
        }
        [HttpPost("SaveEmployee")]
        public async Task<IActionResult> SaveEmployee([FromBody] saveEmployeeDTO employee)
        {
            if (employee == null)
            {
                return BadRequest("Employee data is null.");
            }
            var savedEmployee = await _employeeRepository.SaveEmployeeAsync(employee);
            if (savedEmployee == null)
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }
            return Ok(new { success = true, data = savedEmployee });
        }

        [HttpPost("UpdateEmployee")]
        public async Task<IActionResult> UpdateEmployee([FromBody] updateEmployeeDTO employee)
        {
            var updated = await _employeeRepository.UpdateEmployeeAsync(employee);
            if (updated == null)
                return Json(new { success = false, message = $"Employee with ID {employee.empId} not found." });

            return Json(new { success = true, data = updated });
        }



        [HttpPost("DeleteEmployee/{id}")]

        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var deleted = await _employeeRepository.DeleteEmployeeAsync(id);
            if (deleted == false) // check if 0 rows were affected
                return NotFound(new { success = false, message = $"Employee with ID {id} not found." });

            return Ok(new { success = true, message = "Employee deleted successfully." });
        }



        [HttpPost("CheckEmail")]
        public async Task<IActionResult> CheckEmail(string email, int empId)
        {
            var exists = await _employeeRepository.EmailExistsAsync(email, empId);

            // jQuery validate expects JSON true/false
            return Json(!exists);
        }




    }
}
