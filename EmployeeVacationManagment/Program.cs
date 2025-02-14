using EmployeeVacationManagment.Entites;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EmployeeVacationManagment
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var context = new EFCoreDBContext())
            {
                AddVacationTypes(context);
                AddRequestStates(context);
                AddDepartments(context);
                AddPositions(context);
                AddEmployees(context);

                
               UpdateEmployeeInfo(context, "E1009", "updated name testing", 2, 3, 60000);

                // Submit a vacation request for employee E1001
                SubmitVacationRequest(context, "E1001", new DateTime(2025, 02, 14), new DateTime(2025, 02, 18));

                SubmitVacationRequest(context, "E1003", new DateTime(2025, 02, 14), new DateTime(2025, 02, 18));

                // Update vacation days after approving a vacation request
                UpdateVacationDays(context, "E1001", 5); // Example: Deduct 5 days for vacation


                // Display all pending vacation requests
                ShowPendingVacationRequests(context);

                // make the user approve or decline specifc vacation request 
                Console.WriteLine("Enter the Vacation Request ID to approve or decline:");
                int requestId = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("Enter 'approve' to approve or 'decline' to decline:");
                string action = Console.ReadLine().ToLower();

                if (action == "approve")
                {
                    
                    ApproveVacationRequest(context, requestId, "E1001");
                }
                else if (action == "decline")
                {
                    

                    DeclineVacationRequest(context, requestId, "E1001");
                }
                else
                {

                    Console.WriteLine("Invalid action. Please enter 'approve' or 'decline'.");
                }



                Console.WriteLine("all employees");

                GetAllEmployees(context);

                Console.WriteLine("  ");
                Console.WriteLine("employees by number");
                GetEmployeeByNumber(context, "E1001");

                Console.WriteLine("  ");
                Console.WriteLine("employees by number 2");

                GetEmployeeByNumber(context, "E1003");

                Console.WriteLine("  ");
                Console.WriteLine("Vacation History request");

                GetApprovedVacationRequests(context, "E1001");

                Console.WriteLine("  ");
                Console.WriteLine("Vacation History request2");

                GetApprovedVacationRequests(context, "E1003");

                Console.WriteLine("  ");
                Console.WriteLine("pending vacation request employee1");

                GetPendingVacationRequestsForReview(context, "E1001");

                Console.WriteLine("  ");
                Console.WriteLine("pending vacation request employee2");

                GetPendingVacationRequestsForReview(context, "E1003");



            }
        }

        // retrive data about all the pending Requests history of a specfic employee
        public static void GetPendingVacationRequestsForReview(EFCoreDBContext context, string reviewerEmployeeNumber)
        {
            var pendingRequests = context.VacationRequests
     .AsNoTracking()
     .Where(vr => vr.RequestStateId == 1) 
     .Select(vr => new
     {
         vr.Description,
         vr.EmployeeNumber,
         EmployeeName = vr.Employee.Name,
         vr.SubmissionDate,
         VacationDuration = vr.TotalDays >= 14 ? $"{vr.TotalDays / 7} weeks" : $"{vr.TotalDays} days",
         StartDate = vr.StartDate.ToString("yyyy-MM-dd"),
         EndDate = vr.EndDate.ToString("yyyy-MM-dd"),
         vr.Employee.Salary
     })
     .ToList();

            if (pendingRequests.Any())
            {
                Console.WriteLine($"Pending Vacation Requests for Review by Employee: {reviewerEmployeeNumber}");
                foreach (var request in pendingRequests)
                {
                    Console.WriteLine($"- Description: {request.Description}");
                    Console.WriteLine($"  Employee Number: {request.EmployeeNumber}");
                    Console.WriteLine($"  Employee Name: {request.EmployeeName}");
                    Console.WriteLine($"  Submitted On: {request.SubmissionDate:yyyy-MM-dd}");
                    Console.WriteLine($"  Duration: {request.VacationDuration}");
                    Console.WriteLine($"  Start Date: {request.StartDate}");
                    Console.WriteLine($"  End Date: {request.EndDate}");
                    Console.WriteLine($"  Employee Salary: {request.Salary:C}");
                    Console.WriteLine("--------------------------------------------------");
                }
            }
            else
            {
                Console.WriteLine($"No pending vacation requests found for reviewer: {reviewerEmployeeNumber}");
            }
        }




        // retrive data about all the approvded vacations request of a specifc employee
        public static void GetApprovedVacationRequests(EFCoreDBContext context, string employeeNumber)
        {
            var approvedRequests = context.VacationRequests
                .AsNoTracking() 
                .Where(vr => vr.EmployeeNumber == employeeNumber && vr.RequestState.Name == "Approved") 
                .Select(vr => new
                {
                    VacationType = vr.VacationType.Name, 
                    vr.Description, 
                    RequestDuration = $"{vr.StartDate:yyyy-MM-dd} to {vr.EndDate:yyyy-MM-dd}", 
                    vr.TotalDays, 
                    ApprovedBy = context.Employees
                        .Where(e => e.EmployeeNumber == vr.ApprovedBy) 
                        .Select(e => e.Name)
                        .FirstOrDefault() ?? "Unknown" 
                })
                .ToList(); 

            if (approvedRequests.Any())
            {
                Console.WriteLine($"Approved Vacation Requests for Employee: {employeeNumber}");
                foreach (var request in approvedRequests)
                {
                    Console.WriteLine($"- Vacation Type: {request.VacationType}");
                    Console.WriteLine($"  Description: {request.Description}");
                    Console.WriteLine($"  Duration: {request.RequestDuration} ({request.TotalDays} days)");
                    Console.WriteLine($"  Approved By: {request.ApprovedBy}");
                    Console.WriteLine("--------------------------------------------------");
                }
            }
            else
            {
                Console.WriteLine($"No approved vacation requests found for employee: {employeeNumber}");
            }
        }



        // retrive general data about a specific employee
        public static void GetEmployeeByNumber(EFCoreDBContext context, string employeeNumber)
        {
            var employee = context.Employees
                .AsNoTracking()
                .Where(e => e.EmployeeNumber == employeeNumber) 
                .Select(e => new
                {
                    e.EmployeeNumber,
                    e.Name,
                    DepartmentName = e.Department.Name, 
                    PositionName = e.Position.Name,     
                    ReportsTo = context.Employees
                        .Where(m => m.EmployeeNumber == e.ReportsTo) 
                        .Select(m => m.Name)
                        .FirstOrDefault(), 
                    e.VacationDaysLeft
                })
                .FirstOrDefault(); 

            if (employee != null)
            {
                Console.WriteLine($"Employee Number: {employee.EmployeeNumber}");
                Console.WriteLine($"Name: {employee.Name}");
                Console.WriteLine($"Department: {employee.DepartmentName}");
                Console.WriteLine($"Position: {employee.PositionName}");
                Console.WriteLine($"Reports To: {employee.ReportsTo ?? "None"}");
                Console.WriteLine($"Total Vacation Days Left: {employee.VacationDaysLeft}");
            }
            else
            {
                Console.WriteLine($"Employee with Employee Number '{employeeNumber}' not found.");
            }
        }


        // retrive all the employees in the database
        public static void GetAllEmployees(EFCoreDBContext context)
        {
            var employees = context.Employees
                .AsNoTracking() 
                .Select(e => new
                {
                    e.EmployeeNumber,
                    e.Name,
                    DepartmentName = e.Department.Name, 
                    e.Salary
                })
                .ToList(); 

            // Display employees
            foreach (var emp in employees)
            {
                Console.WriteLine($"Employee Number: {emp.EmployeeNumber}, Name: {emp.Name}, " +
                                  $"Department: {emp.DepartmentName}, Salary: {emp.Salary:C}");
            }
        }




        // add 20 departments in the database
        public static void AddDepartments(EFCoreDBContext context)
        {
            for (int i = 1; i <= 20; i++)
            {
                context.Departments.Add(new Department
                {
                    Name = "Department " + i
                });
            }
            context.SaveChanges();
        }

        // add 20 posistions in the database
        public static void AddPositions(EFCoreDBContext context)
        {
            for (int i = 1; i <= 20; i++)
            {
                context.Positions.Add(new Position
                {
                    Name = "Position " + i
                });
            }
            context.SaveChanges();
        }

        // add 20 Employees in the database
        public static void AddEmployees(EFCoreDBContext context)
        {
            var employees = new List<Employee>
            {
                new Employee { EmployeeNumber = "E1001", Name = "Basil Mohammed", DepartmentId = 1, PositionId = 1, GenderCode = "M", VacationDaysLeft = 24, Salary = 50000 },

                new Employee { EmployeeNumber = "E1002", Name = "Mohammed Ahmad", DepartmentId = 2, PositionId = 2, GenderCode = "F", ReportsTo = "E1001", VacationDaysLeft = 24, Salary = 55000 },

                
                new Employee { EmployeeNumber = "E1003", Name = "Feras Mohammed", DepartmentId = 3, PositionId = 3, GenderCode = "M", VacationDaysLeft = 24, Salary = 60000 },
                
                new Employee { EmployeeNumber = "E1004", Name = "Mohammed Madhoon", DepartmentId = 4, PositionId = 4, GenderCode = "F", ReportsTo = "E1001", VacationDaysLeft = 24, Salary = 65000 },
                
                new Employee { EmployeeNumber = "E1005", Name = "Mohammed Matar", DepartmentId = 5, PositionId = 5, GenderCode = "M", VacationDaysLeft = 24, Salary = 70000 },
                
                new Employee { EmployeeNumber = "E1006", Name = "Mosa khalid", DepartmentId = 6, PositionId = 6, GenderCode = "F", ReportsTo = "E1002", VacationDaysLeft = 24, Salary = 75000 },
                
                new Employee { EmployeeNumber = "E1007", Name = "Khalid abdalziz", DepartmentId = 7, PositionId = 7, GenderCode = "M", VacationDaysLeft = 24, Salary = 80000 },
                
                new Employee { EmployeeNumber = "E1008", Name = "salm yousef", DepartmentId = 8, PositionId = 8, GenderCode = "F", ReportsTo = "E1005", VacationDaysLeft = 24, Salary = 85000 },
                
                new Employee { EmployeeNumber = "E1009", Name = "abdulrhman alasmer", DepartmentId = 9, PositionId = 9, GenderCode = "M", VacationDaysLeft = 24, Salary = 90000 },
                
                new Employee { EmployeeNumber = "E1010", Name = "sara salman", DepartmentId = 10, PositionId = 10, GenderCode = "F", ReportsTo = "E1009", VacationDaysLeft = 24, Salary = 95000 },

                new Employee { EmployeeNumber = "E1011", Name = "jana abdalrhman", DepartmentId = 11, PositionId = 11, GenderCode = "M", VacationDaysLeft = 24, Salary = 52000 },

                new Employee { EmployeeNumber = "E1012", Name = "abdalhadi alfayz", DepartmentId = 12, PositionId = 12, GenderCode = "F", ReportsTo = "E1011", VacationDaysLeft = 24, Salary = 57000 },

                new Employee { EmployeeNumber = "E1013", Name = "rashid muslim", DepartmentId = 13, PositionId = 13, GenderCode = "M", VacationDaysLeft = 24, Salary = 63000 },

                new Employee { EmployeeNumber = "E1014", Name = "salman alsalm", DepartmentId = 14, PositionId = 14, GenderCode = "F", ReportsTo = "E1013", VacationDaysLeft = 24, Salary = 68000 },

                new Employee { EmployeeNumber = "E1015", Name = "abdlhadi", DepartmentId = 15, PositionId = 15, GenderCode = "M", VacationDaysLeft = 24, Salary = 73000 },

                new Employee { EmployeeNumber = "E1016", Name = "khaled alrmuny", DepartmentId = 16, PositionId = 16, GenderCode = "F", ReportsTo = "E1015", VacationDaysLeft = 24, Salary = 78000 },

                new Employee { EmployeeNumber = "E1017", Name = "islam alrashid", DepartmentId = 17, PositionId = 17, GenderCode = "M", VacationDaysLeft = 24, Salary = 83000 },

                new Employee { EmployeeNumber = "E1018", Name = "fahid almuld", DepartmentId = 18, PositionId = 18, GenderCode = "F", ReportsTo = "E1017", VacationDaysLeft = 24, Salary = 88000 },

                new Employee { EmployeeNumber = "E1019", Name = "ahmed mosa", DepartmentId = 19, PositionId = 19, GenderCode = "M", VacationDaysLeft = 24, Salary = 93000 },

                new Employee { EmployeeNumber = "E1020", Name = "yousef jamal", DepartmentId = 20, PositionId = 20, GenderCode = "F", ReportsTo = "E1019", VacationDaysLeft = 24, Salary = 98000 }
            };

            context.Employees.AddRange(employees);
            context.SaveChanges();
        }

        // update the employee information in the database
        public static void UpdateEmployeeInfo(EFCoreDBContext context, string employeeNumber, string name, int departmentId, int positionId, decimal salary)
        {
            var employee = context.Employees.FirstOrDefault(e => e.EmployeeNumber == employeeNumber);
            if (employee != null)
            {
                employee.Name = name;
                employee.DepartmentId = departmentId;
                employee.PositionId = positionId;
                employee.Salary = salary;

                context.SaveChanges();
            }
        }

        // create new vacation request
        public static void SubmitVacationRequest(EFCoreDBContext context, string employeeNumber, DateTime startDate, DateTime endDate)
        {
            
            var employee = context.Employees.FirstOrDefault(e => e.EmployeeNumber == employeeNumber);
            if (employee == null)
            {
                Console.WriteLine("Employee not found.");
                return;
            }

           
            var vacationType = context.VacationTypes.FirstOrDefault(vt => vt.Code == "VAC001"); 
            if (vacationType == null)
            {
                Console.WriteLine("Vacation type not found.");
                return;
            }

            
            var existingRequest = context.VacationRequests
                .FirstOrDefault(vr => vr.EmployeeNumber == employeeNumber && vr.StartDate == startDate && vr.EndDate == endDate);

            if (existingRequest != null)
            {
                Console.WriteLine("Vacation request already exists for the given date range.");
                return;
            }

           
            // the defult vacation request state is Pending , so it has request id 1 
            var vacationRequest = new VacationRequest
            {
                EmployeeNumber = employeeNumber,
                StartDate = startDate,
                EndDate = endDate,
                VacationTypeCode = vacationType.Code,  
                SubmissionDate = DateTime.Now,
                RequestStateId = 1, 
                Description = "Requested vacation from " + startDate.ToShortDateString() + " to " + endDate.ToShortDateString() // Ensure Description is set
            };

            context.VacationRequests.Add(vacationRequest);
            context.SaveChanges(); 

            Console.WriteLine("Vacation request submitted successfully.");
        }

        // update the vacations left days for employee after approving the vacation
        public static void UpdateVacationDays(EFCoreDBContext context, string employeeNumber, int daysTaken)
        {
            var employee = context.Employees.FirstOrDefault(e => e.EmployeeNumber == employeeNumber);
            if (employee != null)
            {
                employee.VacationDaysLeft -= daysTaken;

                context.SaveChanges();
            }
        }


        // add request states to the database
        public static void AddRequestStates(EFCoreDBContext context)
        {
            if (!context.RequestStates.Any()) 
            {
                context.RequestStates.AddRange(
                    new RequestState { Name = "Pending" },
                    new RequestState { Name = "Approved" },
                    new RequestState { Name = "Declined" }
                );
                context.SaveChanges();
            }
        }

        // approve the vacation request
        public static void ApproveVacationRequest(EFCoreDBContext context, int vacationRequestId, string approvedBy)
        {
            var vacationRequest = context.VacationRequests.FirstOrDefault(vr => vr.Id == vacationRequestId);

            if (vacationRequest == null)
            {
                Console.WriteLine("Vacation request not found.");
                return;
            }

           
            if (vacationRequest.RequestStateId != 1) 
            {
                Console.WriteLine("Vacation request is not in a pending state.");
                return;
            }

            
            vacationRequest.RequestStateId = 2; 
            vacationRequest.ApprovedBy = approvedBy;

            context.SaveChanges();

            Console.WriteLine("Vacation request approved.");
        }

        // decline the vacation request for a specfic employee
        public static void DeclineVacationRequest(EFCoreDBContext context, int vacationRequestId, string declinedBy)
        {
            var vacationRequest = context.VacationRequests.FirstOrDefault(vr => vr.Id == vacationRequestId);

            if (vacationRequest == null)
            {
                Console.WriteLine("Vacation request not found.");
                return;
            }

            
            if (vacationRequest.RequestStateId != 1) 
            {
                Console.WriteLine("Vacation request is not in a pending state.");
                return;
            }

            
            vacationRequest.RequestStateId = 3; 
            vacationRequest.DeclinedBy = declinedBy; 

            context.SaveChanges();

            Console.WriteLine("Vacation request declined.");
        }



        // retrive all the the pending vacation requests 
        public static void ShowPendingVacationRequests(EFCoreDBContext context)
        {
            var pendingRequests = context.VacationRequests
                .Where(vr => vr.RequestStateId == 1) 
                .ToList();

            if (pendingRequests.Any())
            {
                foreach (var request in pendingRequests)
                {
                    Console.WriteLine($"Request ID: {request.Id}, Employee: {request.EmployeeNumber}, Start: {request.StartDate.ToShortDateString()}, End: {request.EndDate.ToShortDateString()}, Description: {request.Description}");
                }
            }
            else
            {
                Console.WriteLine("No pending vacation requests.");
            }
        }


        // add new vacation type 
        public static void AddVacationTypes(EFCoreDBContext context)
        {
            if (!context.VacationTypes.Any()) 
            {
                context.VacationTypes.AddRange(
                    new VacationType { Code = "VAC001", Name = "Annual Leave" },
                    new VacationType { Code = "VAC002", Name = "Sick Leave" },
                    new VacationType { Code = "VAC003", Name = "Personal Leave" }
                );
                context.SaveChanges();
            }
        }



    }
}
