using OutOfOffice.DbLogic;
using System.Text.Json.Serialization;

namespace OutOfOffice.Models
{
    public class ProjectView
    {
        public int ID { get; init; }
        [JsonConverter(typeof(EnumNameConverter<ProjectType>))]
        public ProjectType ProjectType { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int ProjectManagerId { get; set; }
        public EmployeeView? ProjectManager { get; set; }
        public string? Comment { get; set; }
        public bool IsActive { get; set; }
        public List<EmployeeView>? Employees { get; set; }
    }
}
