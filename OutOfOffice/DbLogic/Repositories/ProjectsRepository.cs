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
                foreach (var project in employee.Projects)
                {
                    result.Add(project);
                }
            }
            return result;

        }
        public async Task<IEnumerable<Project>> GetAllByPMIdAsync(int pmId)
        {
            return await _context.Projects
                .Where(project => project.ProjectManagerId == pmId)
                .ToListAsync();
        }
        public async Task<Project> GetByIdIncludeEmployeesAndPMOrDefaultAsync(int id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _context.Projects
                .Include(project => project.Employees)
                .Include(project => project.ProjectManager)
                .FirstOrDefaultAsync(project => project.ID == id);
#pragma warning restore CS8603 // Possible null reference return.
        }
        public async Task<Project> GetByIdOrDefaultAsync(int id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _context.Projects.FindAsync(id);
#pragma warning restore CS8603 // Possible null reference return.
        }
        public async Task AddAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Project project)
        {
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
        }
        public async Task ChangeStatusForCertainProjectAsync(Project project)
        {
            project.IsActive = !project.IsActive;
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> CheckIfExistsByIdAsync(int id)
        {
            return await _context.Projects.AnyAsync(project => project.ID == id);
        }

    }
}
