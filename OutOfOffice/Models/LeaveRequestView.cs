using OutOfOffice.DbLogic;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OutOfOffice.Models
{
    public class LeaveRequestView : IDateRange
    {
        public int ID { get; init; }
        public Employee? Employee { get; set; }

        [Display(Name = "Причина відсутності")]
        [JsonConverter(typeof(EnumNameConverter<AbsenceReason>))]
        public AbsenceReason AbsenceReason { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Початок")]
        public DateOnly StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Кінець")]
        public DateOnly EndDate { get; set; }

        [Display(Name = "Коментар")]
        public string? Comment { get; set; }

        [Display(Name = "Статус")]
        [JsonConverter(typeof(EnumNameConverter<LeaveRequestStatus>))]
        public LeaveRequestStatus Status { get; init; }
        public List<ApprovalRequestView>? ApprovalRequests { get; set; }
    }
}
