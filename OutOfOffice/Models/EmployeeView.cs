using OutOfOffice.DbLogic;

namespace OutOfOffice.Models
{
    public class EmployeeView
    {
        public int ID { get; init; }
        public string FullName { get; set; }
        public Subdivision Subdivision { get; set; }
        public Position Position { get; set; }
        public bool IsActive { get; set; }
        public EmployeeView? PeoplePartner { get; set; }
        public int OutOfOfficeBalance { get; set; }
        public byte[]? Photo { get; set; }
        public List<LeaveRequestView>? LeaveRequests { get; set; }
        public List<ApprovalRequestView>? Approvals { get; set; }
        public ProjectView? Project { get; set; }
    }
}
