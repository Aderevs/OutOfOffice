using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace OutOfOffice.DbLogic
{
    public class OutOfOfficeDbContext : DbContext
    {
        private readonly string _connectionString;
        public DbSet<Employee> Employees { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<ApprovalRequest> ApprovalRequests { get; set; }
        public DbSet<Project> Projects { get; set; }
        public OutOfOfficeDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .HasOne(employee => employee.PeoplePartner)
                .WithMany(employee => employee.SubordinateEmployees)
                .HasForeignKey(employee => employee.PeoplePartnerId);

            modelBuilder.Entity<LeaveRequest>()
                .HasOne(request => request.Employee)
                .WithMany(employee => employee.LeaveRequests)
                .HasForeignKey(request => request.EmployeeId);

            modelBuilder.Entity<ApprovalRequest>()
                .HasOne(approvalRequest => approvalRequest.LeaveRequest)
                .WithOne(leaveRequest => leaveRequest.ApprovalRequest)
                .HasForeignKey<ApprovalRequest>(approvalRequest => approvalRequest.LeaveRequestId);

            modelBuilder.Entity<ApprovalRequest>()
                .HasOne(approvalRequest => approvalRequest.Approver)
                .WithMany(employee => employee.Approvals)
                .HasForeignKey(approvalRequest => approvalRequest.ApproverId);

            modelBuilder.Entity<Project>()
                .HasOne(project => project.ProjectManager)
                .WithMany(employee => employee.SubordinateProjects)
                .HasForeignKey(project => project.ProjectManagerId);

            modelBuilder.Entity<Employee>()
                .HasMany(employee => employee.Projects)
                .WithMany(project => project.Employees)
                .UsingEntity<Dictionary<string, object>>(
                    "EmployeesProjects",
                    j => j
                        .HasOne<Project>()
                        .WithMany()
                        .HasForeignKey("ProjectId"),
                    j => j
                        .HasOne<Employee>()
                        .WithMany()
                        .HasForeignKey("EmployeeId"),
                    j =>
                    {
                        j.HasKey("ProjectId", "EmployeeId");
                        j.ToTable("EmployeesProjects");
                    });

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(_connectionString)
                .LogTo(e => Debug.WriteLine(e));
        }
    }
}
