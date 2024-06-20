using OutOfOffice.DbLogic;

namespace OutOfOffice.Models
{
    public class ApprovalRequestView
    {
        public int ID { get; init; }
        public Employee? Approver { get; set; }
        public LeaveRequestView? LeaveRequest { get; set; }
        public Status Status { get; init; }
        public string? Comment { get; set; }
    }
}
