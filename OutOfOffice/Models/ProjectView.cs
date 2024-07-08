using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using OutOfOffice.DbLogic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OutOfOffice.Models
{
    public class ProjectView : IDateRange
    {
        private static readonly DateOnly today = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        public ProjectView() { }
        public ProjectView(IEnumerable<Employee> allEmployees)
        {
            SetOptionsEmployees(allEmployees);
        }
        public void SetOptionsEmployees(IEnumerable<Employee> allEmployees)
        {
            AllEmployees = [];
            foreach (var employee in allEmployees)
            {
                AllEmployees.Add(new(employee.FullName, employee.ID.ToString()));
            }
        }
        public int? ID { get; init; }

        [Required]
        [JsonConverter(typeof(EnumNameConverter<ProjectType>))]
        [Display(Name = "Project type")]
        public ProjectType ProjectType { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start date")]
        public DateOnly StartDate { get; set; } = today;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "End date")]
        public DateOnly EndDate { get; set; } = today;

        [Display(Name = "Project Manager name")]
        public string? ProjectManagerName { get; set; }
        public string? Comment { get; set; }
        public bool IsActive { get; set; }

        [Display(Name = "Involved employees")]
        public List<string>? EmployeesIds { get; set; }
        public Dictionary<int, string>? EmployeesIdsNames { get; set; }

        public List<SelectListItem>? AllEmployees { get; set; }
    }
}
