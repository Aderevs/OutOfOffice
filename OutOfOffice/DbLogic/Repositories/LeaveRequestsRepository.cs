using Microsoft.EntityFrameworkCore;

namespace OutOfOffice.DbLogic.Repositories
{
    public class LeaveRequestsRepository
    {
        private readonly OutOfOfficeDbContext _context;

        public LeaveRequestsRepository(OutOfOfficeDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<LeaveRequest>> GetAllAsync()
        {
            return await _context.LeaveRequests.ToListAsync();
        }
        public async Task<IEnumerable<LeaveRequest>> GetAllByEmployeeId(int EmployeeId)
        {
            return await _context.LeaveRequests
                .Where(x => x.EmployeeId == EmployeeId)
                .ToListAsync();
        }
        public async Task<LeaveRequest> GetByIdOrDefaultAsync(int requestId)
        {
#pragma warning disable CS8603 // Possible null reference return.
            return await _context.LeaveRequests.FindAsync(requestId);
#pragma warning restore CS8603 // Possible null reference return.
        }
        public async Task UpdateAsync(LeaveRequest request)
        {
            _context.LeaveRequests.Update(request);
            await _context.SaveChangesAsync();
        }
    }
}
