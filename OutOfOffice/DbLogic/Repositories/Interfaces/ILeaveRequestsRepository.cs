namespace OutOfOffice.DbLogic.Repositories.Interfaces
{
    public interface ILeaveRequestsRepository
    {
        Task AddAsync(LeaveRequest request);
        Task<IEnumerable<LeaveRequest>> GetAllAsync();
        Task<IEnumerable<LeaveRequest>> GetAllByEmployeeId(int EmployeeId);
        Task<LeaveRequest> GetByIdOrDefaultAsync(int requestId);
        Task UpdateAsync(LeaveRequest request);
    }
}