using OutOfOffice.Attributes;

namespace OutOfOffice.DbLogic
{
    //public enum Subdivision
    //{
    //    WebDevelopment,
    //    MobileAndDesktop,
    //    DataScienceAndAnalysis,
    //    IoTAndEmbeddedSystems,
    //    ProjectManagementAndQualityAssurance,
    //    HumanResourceManagement,
    //    CompanySystemsAdministration
    //}
    public enum Subdivision
    {
        [DisplayJson("Сервіс")]
        Service,

        [DisplayJson("Адміністрування та підтримка")]
        AdministrationAndSupport,

        [DisplayJson("Управління людськими ресурсами")]
        HumanRecourceManagement,
        
        [DisplayJson("Маркетинг та продажі")]
        MarketingAndSales
    }
    public enum Position
    {
        [DisplayJson("Працівник")]
        Employee,

        [DisplayJson("HR Менеджер")]
        HRManager,

        [DisplayJson("Менеджер проєктів")]
        ProjectManager,

        [DisplayJson("Адміністратор")]
        Administrator
    }

    //public enum Subdivision
    //{
    //    Сервіс,
    //    IT,
    //    УправлінняЛюдськимиРесурсами,
    //    МаркетингТаПродажі
    //}
    //public enum Position
    //{
    //    Працівник,
    //    HRМенеджер,
    //    МенеджерПроєктів,
    //    Адміністратор
    //}
    public class Employee
    {
        public int ID { get; init; }
        public string PasswordHash { get; set; }
        public Guid Salt { get; set; }
        public string FullName { get; set; }
        public Subdivision Subdivision { get; set; }
        public Position Position { get; set; }
        public bool IsActive { get; set; }
        public int? PeoplePartnerId { get; set; }
        public Employee? PeoplePartner { get; set; }
        public int OutOfOfficeBalance { get; set; }
        public byte[]? Photo { get; set; }
        public List<LeaveRequest>? LeaveRequests { get; set; }
        public List<ApprovalRequest>? Approvals { get; set; }
        public List<Project>? Projects { get; set; }
        public List<Project>? SubordinateProjects { get; set; }
        public List<Employee>? SubordinateEmployees { get; set; }
    }
}
