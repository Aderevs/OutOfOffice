using OutOfOffice.DbLogic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OutOfOffice.Models
{
    public class ApprovalRequestView
    {
        public int ID { get; init; }

        [Display(Name = "Ім'я працівника")]
        public string? EmployeeName {  get; set; }

        [Display(Name = "Ім'я апрувера")]
        public string? ApproverName {  get; set; }

        [Display(Name = "Причина відсутності")]
        [JsonConverter(typeof(EnumNameConverter<AbsenceReason>))]
        public AbsenceReason AbsenceReason { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Дата початку")]
        public DateOnly StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]

        [Display(Name = "Дата закінчення")]
        public DateOnly EndDate { get; set; }

        [Display(Name = "Коментар працівника")]
        public string? LeaveComment { get; set; }

        [Display(Name = "ID запиту на відсутність")]
        public int LeaveID { get; set; }

        [Display(Name = "Статус")]
        [JsonConverter(typeof(EnumNameConverter<LeaveRequestStatus>))]
        public LeaveRequestStatus Status { get; set; }

        [Display(Name = "Коментар апрувера")]
        public string? Comment { get; set; }
    }
}
