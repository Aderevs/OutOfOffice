using Microsoft.EntityFrameworkCore;

namespace OutOfOffice.DbLogic.Repositories
{
    public class ProjectsRepository
    {
        private readonly OutOfOfficeDbContext _context;

        public ProjectsRepository(OutOfOfficeDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Project>> GetAllAsync()
        {
            return await _context.Projects.ToListAsync();
        }
        public async Task<IEnumerable<Project>> GetAllByEmployeeIdIncludePMAsync(int employeeId)
        {
            var employee = await _context.Employees
                .Include(employee=> employee.Projects)
                .ThenInclude(project=>project.ProjectManager)
                .FirstAsync(employee=> employee.ID == employeeId);
            return employee.Projects;
                
        }
    }
}
