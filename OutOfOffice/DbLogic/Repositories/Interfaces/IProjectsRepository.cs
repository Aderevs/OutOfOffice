namespace OutOfOffice.DbLogic.Repositories.Interfaces
{
    public interface IProjectsRepository
    {
        Task AddAsync(Project project);
        Task ChangeStatusForCertainProjectAsync(Project project);
        Task<IEnumerable<Project>> GetAllAsync();
        Task<IEnumerable<Project>> GetAllByEmployeeIdIncludePMAsync(int employeeId);
        Task<IEnumerable<Project>> GetAllByPMIdAsync(int pmId);
        Task<IEnumerable<Project>> GetAllByRageOfIdsAsync(IEnumerable<int> ids);
        Task<IEnumerable<Project>> GetAllProjectsOfSubordinateEmployeesByHRIdIncludePMAsync(int hrId);
        Task<Project> GetByIdIncludeEmployeesAndPMOrDefaultAsync(int id);
        Task<Project> GetByIdIncludeEmployeesOrDefaultAsync(int id);
        Task<Project> GetByIdOrDefaultAsync(int id);
        Task UpdateAsync(Project project);
    }
}