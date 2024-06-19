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
        public async Task<IEnumerable<Employee>> GetAllEmployees()
        {
            return await _context.Employees.ToListAsync();
        }
        public async Task<Employee> GetEmployeeByFullNameOrDefaultAsync(string fullName)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _context.Employees.FirstOrDefaultAsync(employee => employee.FullName == fullName);
#pragma warning restore CS8603 // Possible null reference return.
        }

    }
}
