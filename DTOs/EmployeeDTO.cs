namespace MVC_CRUD.DTOs
{
    public class saveEmployeeDTO
    {
        
        public int empId { get; set; }
        public string empName { get; set; }
        public string empEmail { get; set; }
        public string empDepartment { get; set; }
        public decimal empSalary { get; set; }
    }
    public class updateEmployeeDTO
    {
        public int empId { get; set; }
        public string empName { get; set; }
        public string empEmail { get; set; }
        public string empDepartment { get; set; }
        public decimal empSalary { get; set; }
    }
}
