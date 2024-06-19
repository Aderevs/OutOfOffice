namespace OutOfOffice.DbLogic
{
    public enum Subdivision
    {

    }
    public enum Position
    {

    }
    public class Employee
    {
        public int ID { get; init; }
        public string FullName { get; set; }
        public Subdivision Subdivision { get; set; }
        public Position Position { get; set; }
        public bool Status { get; set; }
        public int PeoplePartnerId { get; set; }
        public Employee? PeoplePartner { get; set; }
        public int OutOfOfficeBalance { get; set; }
        public byte[]? Photo { get; set; }
        public List<LeaveRequest>? LeaveRequests { get; set; }
        public List<ApprovalRequest>? Approvals { get; set; }
        public Project? Project { get; set; }
    }
}
