using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using OutOfOffice.DbLogic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OutOfOffice.Models
{
    public class ProjectView : IDateRange
    {
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

        [Display(Name = "Involved employees")]
        public List<string>? EmployeesIds { get; set; }
        public Dictionary<int, string>? EmployeesIdsNames { get; set; }

        public List<SelectListItem>? AllEmployees { get; set; }
    }
}
