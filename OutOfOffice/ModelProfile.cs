using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using OutOfOffice.DbLogic;
using OutOfOffice.Models;

namespace OutOfOffice
{
    public class ModelProfile: Profile
    {
        public ModelProfile()
        {
            CreateMap<Employee, EmployeeView>();
            CreateMap<LeaveRequest, LeaveRequestView>();
            CreateMap<ApprovalRequest, ApprovalRequestView>();
            CreateMap<Project, ProjectView>();
            
            CreateMap<EmployeeView, Employee>();
            CreateMap<LeaveRequestView, LeaveRequest>();
            CreateMap<ApprovalRequestView, ApprovalRequest>();
            CreateMap<ProjectView, Project>();

            CreateMap<EmployeeBinding, Employee>()
                .ForMember(dest => dest.PeoplePartnerId, opt => opt.MapFrom(src => 
                    !src.PeoplePartnerId.IsNullOrEmpty() ? int.Parse(src.PeoplePartnerId) : (int?)null));
        }
        
    }
}
