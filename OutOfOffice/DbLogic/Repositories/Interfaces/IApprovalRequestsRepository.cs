namespace OutOfOffice.DbLogic.Repositories.Interfaces
{
    public interface IApprovalRequestsRepository
    {
        Task AddAsync(ApprovalRequest request);
        Task AddListAsTransactionAsync(List<ApprovalRequest> requests);
        Task DeleteByLeaveRequestId(int leaveRequestId);
        Task<IEnumerable<ApprovalRequest>> GetAllOfApproverByIdAsync(int approverId);
        Task<IEnumerable<ApprovalRequest>> GetAllForLeaveRequestsOfEmployeeByIdIncludeApproverAsync(int employeeId);
        Task<ApprovalRequest> GetByIdIncludeLeaveAndEmployeeAsync(int approvalId);
        Task<ApprovalRequest> GetByIdIncludeLeaveAndApproverAsync(int approvalId);
        Task<ApprovalRequest> GetByIdIncludeLeaveOrDefaultAsync(int approvalId);
        Task<ApprovalRequest> GetByIdOrDefaultAsync(int id);
        Task UpdateAsync(ApprovalRequest request);
    }
}