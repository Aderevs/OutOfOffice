namespace OutOfOffice.DbLogic
{
    public enum ProjectType
    {
        WebDevelopment,
        MobileAppDevelopment,
        DesktopApplications,
        DataAnalysisAndVisualization,
        MachineLearning,
        InternetOfThings,
        Robotics
    }
    public class Project
    {
        public int ID { get; init; }
        public ProjectType ProjectType { get;set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int ProjectManagerId {  get; set; }
        public Employee? ProjectManager { get; set; }
        public string? Comment { get; set; }
        public bool IsActive {  get; set; }
        public List<Employee> Employees { get; set; }
    }
}
