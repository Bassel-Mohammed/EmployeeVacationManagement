using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeVacationManagment.Entites
{
    public class EFCoreDBContext : DbContext
    {
        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<VacationType> VacationTypes { get; set; }
        public DbSet<RequestState> RequestStates { get; set; }
        public DbSet<VacationRequest> VacationRequests { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-4PANH4P\\SQLEXPRESS;Database=EmployeeVacationDataBase;Integrated Security=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           // Employee and Department RelationShip (One to Many)
    modelBuilder.Entity<Employee>()
        .HasOne(e => e.Department)
        .WithMany(d => d.Employees)  
        .HasForeignKey(e => e.DepartmentId);



    // Employee and Position RelationShip (One to Many)
    modelBuilder.Entity<Employee>()
        .HasOne(e => e.Position)
        .WithMany(p => p.Employees)  
        .HasForeignKey(e => e.PositionId);



    // Employee and Manager RelatioShip (One to Many, if ReportsTo is not null)
    modelBuilder.Entity<Employee>()
        .HasOne(e => e.Manager)
        .WithMany()  
        .HasForeignKey(e => e.ReportsTo)
        .OnDelete(DeleteBehavior.Restrict);  



    // VacationRequest and Employee RelationShip (Many to One relationship, via EmployeeNumber)
    modelBuilder.Entity<VacationRequest>()
        .HasOne(v => v.Employee)
        .WithMany()  
        .HasForeignKey(v => v.EmployeeNumber)
        .HasPrincipalKey(e => e.EmployeeNumber);  



    // VacationRequest and VacationType RelationShip (Many to One)
    modelBuilder.Entity<VacationRequest>()
        .HasOne(v => v.VacationType)
        .WithMany()  
        .HasForeignKey(v => v.VacationTypeCode);



    // VacationRequest and RequestState RelationShip (Many to One)
    modelBuilder.Entity<VacationRequest>()
        .HasOne(v => v.RequestState)
        .WithMany()  
        .HasForeignKey(v => v.RequestStateId);
        }
    }
}
