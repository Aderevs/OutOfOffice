using OutOfOffice.DbLogic;
using System.ComponentModel.DataAnnotations;

namespace OutOfOffice.Models
{
    public class LeaveRequestView
    {
        public int ID { get; init; }
        public Employee? Employee { get; set; }
        public AbsenceReason AbsenceReason { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateOnly StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateOnly EndDate { get; set; }
        public string? Comment { get; set; }
        public Status Status { get; init; }
        public ApprovalRequestView? ApprovalRequest { get; set; }
    }
}
