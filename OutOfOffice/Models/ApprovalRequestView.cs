using OutOfOffice.DbLogic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OutOfOffice.Models
{
    public class ApprovalRequestView
    {
        public int ID { get; init; }
        public Employee? Approver { get; set; }

        [JsonConverter(typeof(EnumNameConverter<AbsenceReason>))]
        public AbsenceReason AbsenceReason { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateOnly StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateOnly EndDate { get; set; }
        public string? LeaveComment { get; set; }

        [JsonConverter(typeof(EnumNameConverter<Status>))]
        public Status Status { get; set; }
        public string? Comment { get; set; }
    }
}
