using System.ComponentModel.DataAnnotations;

namespace MVC_CRUD.Models
{
    public class Employee
    {
        [Key]
        public int empId { get; set; }
        public string empName { get; set; }
        public string empEmail { get; set; }
        public string empDepartment { get; set; }
        public decimal empSalary { get; set; }
    }
}
