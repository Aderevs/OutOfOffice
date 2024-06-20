using OutOfOffice.DbLogic;

namespace OutOfOffice.Models
{
    public class LeaveRequestView
    {
        public int ID { get; init; }
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public AbsenceReason AbsenceReason { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string? Comment { get; set; }
        public Status Status { get; init; }
        public ApprovalRequestView? ApprovalRequest { get; set; }
    }
}
