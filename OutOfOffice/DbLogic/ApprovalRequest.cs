namespace OutOfOffice.DbLogic
{
    public class ApprovalRequest
    {
        public int ID { get; init; }
        public int ApproverId { get; set; }
        public Employee? Approver { get; set; }
        public int LeaveRequestId { get; set; }
        public LeaveRequest? LeaveRequest { get; set; }
        public Status Status { get; init; } = Status.New;
        public string? Comment { get; set; }
    }
}
