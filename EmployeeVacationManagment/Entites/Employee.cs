using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeVacationManagment.Entites
{
    public class Employee
    {
        [Key, MaxLength(6)]
        public string EmployeeNumber { get; set; } = null!;

        [Required, MaxLength(20)]
        public string Name { get; set; } = null!;

        public int DepartmentId { get; set; }
        public int PositionId { get; set; }

        [Required, MaxLength(1)]
        public string GenderCode { get; set; } = null!;

        [MaxLength(6)]
        public string? ReportsTo { get; set; }

        [Range(0, 24)]
        public int VacationDaysLeft { get; set; } = 24;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Salary { get; set; }

        public Department Department { get; set; } = null!;
        public Position Position { get; set; } = null!;
        public Employee? Manager { get; set; }
        public ICollection<Employee> Subordinates { get; set; } = new List<Employee>();

        public Employee()
        {
            Subordinates = new List<Employee>();
        }

        public void AssignSubordinate(Employee employee)
        {
            Subordinates.Add(employee);
        }
    }
}
