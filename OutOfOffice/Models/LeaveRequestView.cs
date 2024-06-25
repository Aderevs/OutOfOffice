using OutOfOffice.DbLogic;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OutOfOffice.Models
{
    public class LeaveRequestView
    {
        public int ID { get; init; }
        public Employee? Employee { get; set; }

        [JsonConverter(typeof(EnumNameConverter<AbsenceReason>))]
        public AbsenceReason AbsenceReason { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateOnly StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateOnly EndDate { get; set; }
        public string? Comment { get; set; }

        [JsonConverter(typeof(EnumNameConverter<Status>))]
        public Status Status { get; init; }
        public ApprovalRequestView? ApprovalRequest { get; set; }
    }
}
