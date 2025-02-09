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
        [Display(Name = "Тип проєкту")]
        [JsonConverter(typeof(EnumNameConverter<ProjectType>))]
        public ProjectType ProjectType { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Дата старту")]
        public DateOnly StartDate { get; set; } = today;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Дата закінчення")]
        public DateOnly EndDate { get; set; } = today;

        [Display(Name = "Ім'я менеджеру проєкту")]
        public string? ProjectManagerName { get; set; }

        [Display(Name = "Коментар")]
        public string? Comment { get; set; }
        public bool IsActive { get; set; }

        [Display(Name = "Залучені працівники")]
        public List<string>? EmployeesIds { get; set; }
        public Dictionary<int, string>? EmployeesIdsNames { get; set; }

        public List<SelectListItem>? AllEmployees { get; set; }
    }
}
