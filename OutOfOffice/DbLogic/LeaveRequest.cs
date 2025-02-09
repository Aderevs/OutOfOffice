using OutOfOffice.Attributes;

namespace OutOfOffice.DbLogic
{
    
    public enum AbsenceReason
    {
        [DisplayJson("Хвороба")]
        Disease,

        [DisplayJson("Вітпустка")]
        Vacation,

        [DisplayJson("Сімецні обставини")]
        FamilyCircumstances,

        [DisplayJson("Освіта")]
        Education,

        [DisplayJson("Інше в коментарях")]
        OtherInComment
    }
    public enum LeaveRequestStatus
    {
        [DisplayJson("Новий")]
        New,

        [DisplayJson("Відправлений")]
        Submit,

        [DisplayJson("Cкасований")]
        Canceled
    }
    public class LeaveRequest
    {
        public int ID { get; init; }
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        public AbsenceReason AbsenceReason { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string? Comment { get; set; }
        public LeaveRequestStatus Status { get; set; } = LeaveRequestStatus.New;
        public List<ApprovalRequest>? ApprovalRequests { get; set; }
    }
}
