using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeVacationManagment.Entites
{
    public class VacationType
    {
        [Key, MaxLength(10)]
        public string Code { get; set; } = null!;

        [Required, MaxLength(20)]
        public string Name { get; set; } = null!;
    }

}
