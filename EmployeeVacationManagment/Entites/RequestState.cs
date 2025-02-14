using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeVacationManagment.Entites
{
    public class RequestState
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(10)]
        public string Name { get; set; } = null!;
    }

}
