namespace OutOfOffice.DbLogic.Repositories.Interfaces
{
    public interface IEmployeesRepository
    {
        Task AddAsync(Employee employee);
        Task AddHRAndSetThemIdToAllEmployeesWithoutPeoplePartnerAsync(Employee hrManager);
        Task ChangeStatusForCertainEmployeeAsync(Employee employee);
        Task<bool> CheckIfAdminAlreadyExistsAsync();
        Task<bool> CheckIfAnyHrExistsAsync();
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<IEnumerable<Employee>> GetAllByRageOfIdsAsync(IEnumerable<int> ids);
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<IEnumerable<Employee>> GetAllHRsAsync();
        Task<IEnumerable<Employee>> GetAllSubordinateEmployeesByHRIdAsync(int hrId);
        Task<IEnumerable<Employee>> GetAllSubordinateEmployeesByPMIdAsync(int pmId);
        Task<Employee> GetByIdIncludeHRAndProjectsWithManagersAsync(int id);
        Task<Employee> GetByIdIncludeProjectsOrDefaultAsync(int id);
        Task<Employee> GetByIdOrDefaultAsync(int id);
        Task<Employee> GetByLeaveRequestIdAsync(int requestId);
        Task<Employee> GetEmployeeByFullNameOrDefaultAsync(string fullName);
        Task UpdateAsync(Employee employee);
    }
}