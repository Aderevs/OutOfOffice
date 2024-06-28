using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace OutOfOffice.DbLogic.Repositories
{
    public class ApprovalRequestsRepository
    {
        private readonly OutOfOfficeDbContext _context;

        public ApprovalRequestsRepository(OutOfOfficeDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(ApprovalRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            _context.ApprovalRequests.Add(request);
            await _context.SaveChangesAsync();
        }
        public async Task AddListAsTransactionAsync(List<ApprovalRequest> requests)
        {
            if (requests == null)
            {
                throw new ArgumentNullException(nameof(requests));
            }
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (ApprovalRequest request in requests)
                {
                    _context.ApprovalRequests.Add(request);
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task DeleteByLeaveRequestId(int leaveRequestId)
        {
            var allRequestsToDelete = await _context.ApprovalRequests
                .Where(approval => approval.LeaveRequestId == leaveRequestId)
                .ToListAsync();

            _context.ApprovalRequests.RemoveRange(allRequestsToDelete);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<ApprovalRequest>> GetAllRequestsOfApproverByIdAsync(int approverId)
        {
            return await _context.ApprovalRequests
                .Include(request => request.LeaveRequest)
                .Where(request => request.ApproverId == approverId)
                .ToListAsync();
        }
        public async Task<ApprovalRequest> GetByIdIncludeLeaveAsync(int approvalId)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _context.ApprovalRequests
                .Include(request => request.LeaveRequest)
                .FirstAsync(approval => approval.ID == approvalId);
#pragma warning restore CS8603 // Possible null reference return.
        }
        public async Task<ApprovalRequest> GetByIdIncludeLeaveOrDefaultAsync(int approvalId)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _context.ApprovalRequests
                .Include(request => request.LeaveRequest)
                .FirstOrDefaultAsync(approval => approval.ID == approvalId);
#pragma warning restore CS8603 // Possible null reference return.
        }
        public async Task<ApprovalRequest> GetByIdOrDefaultAsync(int id)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _context.ApprovalRequests.FindAsync(id);
#pragma warning restore CS8603 // Possible null reference return.
        }
        public async Task UpdateAsync(ApprovalRequest request)
        {
            _context.ApprovalRequests.Update(request);
            await _context.SaveChangesAsync();
        }
    }
}
