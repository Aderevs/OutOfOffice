﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace OutOfOffice.DbLogic.Repositories
{
    public class EmployeesRepository
    {
        private readonly OutOfOfficeDbContext _context;

        public EmployeesRepository(OutOfOfficeDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _context.Employees.ToListAsync();
        }
        public async Task<Employee> GetEmployeeByFullNameOrDefaultAsync(string fullName)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _context.Employees.FirstOrDefaultAsync(employee => employee.FullName == fullName);
#pragma warning restore CS8603 // Possible null reference return.
        }
        public async Task<Employee> GetByIdOrDefaultAsync(int id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _context.Employees.FindAsync(id);
#pragma warning restore CS8603 // Possible null reference return.
        }
        public async Task<Employee> GetByIdIncludeHRAndProjectsWithManagersAsync(int id)
        {
            return await _context.Employees
                .Include(employee => employee.PeoplePartner)
                .Include(employee => employee.Projects)
                .ThenInclude(project => project.ProjectManager)
                .FirstAsync(employee => employee.ID == id);
        }
        public async Task<IEnumerable<Employee>> GetProjectManagersForAllEmployeeProjectsAsync(int id)
        {
            return (IEnumerable<Employee>)await _context.Employees
                .Where(employee => employee.ID == id)
                .Include(employee => employee.Projects)
                .ThenInclude(project => project.ProjectManager)
                .Select(employee => employee.Projects
                    .Select(project => project.ProjectManager))
                .ToListAsync();
        }
        public async Task<IEnumerable<Employee>> GetAllHRsAsync()
        {
            return await _context.Employees
                .Where(employee => employee.Position == Position.HRManager)
                .ToListAsync();
        }
        public async Task<IEnumerable<Employee>> GetAllSubordinateEmployeesByHRIdAsync(int HRId)
        {
            return await _context.Employees
                .Where(employee => employee.PeoplePartnerId == HRId)
                .ToListAsync();
        }
        public async Task<Employee> GetByLeaveRequestIdAsync(int requestId)
        {
            var request = await _context.LeaveRequests
                .Include(request => request.Employee)
                .FirstOrDefaultAsync(request => request.ID == requestId);
            return request.Employee;
        }
        public async Task AddAsync(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> CheckIfAdminAlreadyExistsAsync()
        {
            return await _context.Employees.AnyAsync(employee => employee.Position == Position.Administrator);
        }
        public async Task<bool> CheckIfAnyHrExistsAsync()
        {
            return await _context.Employees.AnyAsync(employee => employee.Position == Position.HRManager);
        }
        public async Task ChangeStatusForCertainEmployeeAsync(Employee employee)
        {
            employee.IsActive = !employee.IsActive;
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> CheckIfExistsEmployeeWithSuchIdAsync(int id)
        {
            return await _context.Employees.AnyAsync(employee => employee.ID == id);
        }
    }
}
