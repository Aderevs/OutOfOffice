using OutOfOffice.DbLogic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OutOfOffice.Models
{
    public class EmployeeView
    {
        public int ID { get; init; }
        public string FullName { get; set; }

        [JsonConverter(typeof(EnumNameConverter<Subdivision>))]
        public Subdivision Subdivision { get; set; }

        [JsonConverter(typeof(EnumNameConverter<Position>))]
        public Position Position { get; set; }
        public bool IsActive { get; set; }
        public EmployeeView? PeoplePartner { get; set; }
        public int OutOfOfficeBalance { get; set; }
        public byte[]? Photo { get; set; }
        public List<LeaveRequestView>? LeaveRequests { get; set; }
        public List<ApprovalRequestView>? Approvals { get; set; }
    }
}
