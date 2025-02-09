using OutOfOffice.Attributes;

namespace OutOfOffice.DbLogic
{
    public enum ApprovalRequestStatus
    {
        [DisplayJson("Новий")]
        New,

        [DisplayJson("Погоджений")]
        Approved,

        [DisplayJson("Відхилений")]
        Rejected
    }
    public class ApprovalRequest
    {
        public int ID { get; init; }
        public int ApproverId { get; set; }
        public Employee? Approver { get; set; }
        public int LeaveRequestId { get; set; }
        public LeaveRequest? LeaveRequest { get; set; }
        public ApprovalRequestStatus Status { get; set; } = ApprovalRequestStatus.New;
        public string? Comment { get; set; }
    }
}
