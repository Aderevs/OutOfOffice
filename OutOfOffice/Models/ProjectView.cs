using OutOfOffice.DbLogic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OutOfOffice.Models
{
    public class ProjectView : IDateRange
    {
        public int? ID { get; init; }

        [Required]
        [JsonConverter(typeof(EnumNameConverter<ProjectType>))]
        public ProjectType ProjectType { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateOnly StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateOnly EndDate { get; set; }
        public string? ProjectManagerName { get; set; }
        public string? Comment { get; set; }
        public bool IsActive { get; set; }
        public List<string>? EmployeesNames { get; set; }
    }
}
