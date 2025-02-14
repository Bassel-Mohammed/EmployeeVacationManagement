using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeVacationManagment.Entites
{
    public class VacationRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime SubmissionDate { get; set; }

        [Required, MaxLength(100)]
        public string Description { get; set; } = null!;

        [Required, MaxLength(6)]
        public string EmployeeNumber { get; set; } = null!;

        [Required, MaxLength(10)]
        public string VacationTypeCode { get; set; } = null!;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int TotalDays { get; set; }

        [Required]
        public int RequestStateId { get; set; }

        public string? ApprovedBy { get; set; }
        public string? DeclinedBy { get; set; }

        public Employee Employee { get; set; } = null!;
        public VacationType VacationType { get; set; } = null!;
        public RequestState RequestState { get; set; } = null!;

        public VacationRequest()
        {
            SubmissionDate = DateTime.Now;
        }
    }
}
