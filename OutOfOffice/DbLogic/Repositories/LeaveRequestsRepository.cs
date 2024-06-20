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
        public async Task<IEnumerable<LeaveRequest>> GetAllByEmployeeId(int id)
        {
            return await _context.LeaveRequests
                .Where(x => x.EmployeeId == id)
                .ToListAsync();
        }
    }
}
