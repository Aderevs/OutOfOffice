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
        public async Task<IEnumerable<Project>> GetAllByEmployeeIdIncludePMAsync(int employeeId)
        {
            var employee = await _context.Employees
                .Include(employee => employee.Projects)
                .ThenInclude(project => project.ProjectManager)
                .FirstAsync(employee => employee.ID == employeeId);
            return employee.Projects;

        }
        public async Task<IEnumerable<Project>> GetAllProjectsOfSubordinateEmployeesByHRIdIncludePMAsync(int hrId)
        {
            var employees = await _context.Employees
                            .Include(employee => employee.Projects)
                            .ThenInclude(project => project.ProjectManager)
                            .Where(employee => employee.PeoplePartnerId == hrId)
                            .ToListAsync();
            HashSet<Project> result = new HashSet<Project>();
            foreach (var employee in employees)
            {
                foreach(var project in employee.Projects)
                {
                    result.Add(project);
                } 
            }
            return result;

        }
        public async Task<Project> GetByIdIncludeEmployeesAndPMOrDefaultAsync(int id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _context.Projects
                .Include(project => project.Employees)
                .Include(project=> project.ProjectManager)
                .FirstOrDefaultAsync(project=>project.ID == id);
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
