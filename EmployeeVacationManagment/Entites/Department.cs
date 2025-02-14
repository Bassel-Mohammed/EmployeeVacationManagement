using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeVacationManagment.Entites
{
    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; } = null!;

       
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }


}
