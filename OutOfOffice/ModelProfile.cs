using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using OutOfOffice.DbLogic;
using OutOfOffice.Models;

namespace OutOfOffice
{
    public class ModelProfile : Profile
    {
        public ModelProfile()
        {
            CreateMap<Employee, EmployeeView>();
            CreateMap<LeaveRequest, LeaveRequestView>();
            CreateMap<ApprovalRequest, ApprovalRequestView>()
                .ForMember(dest => dest.AbsenceReason, opt => opt.MapFrom(src => src.LeaveRequest.AbsenceReason))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.LeaveRequest.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.LeaveRequest.EndDate))
                .ForMember(dest => dest.LeaveComment, opt => opt.MapFrom(src => src.LeaveRequest.Comment));

            CreateMap<Project, ProjectView>();

            CreateMap<EmployeeView, Employee>();
            CreateMap<LeaveRequestView, LeaveRequest>();
            CreateMap<ApprovalRequestView, ApprovalRequest>();
            CreateMap<ProjectView, Project>();

            CreateMap<EmployeeCreateBinding, Employee>()
                .ForMember(dest => dest.PeoplePartnerId, opt => opt.MapFrom(src =>
                    !src.PeoplePartnerId.IsNullOrEmpty() ? int.Parse(src.PeoplePartnerId) : (int?)null))
                .ForMember(dest => dest.Photo, opt => opt.Ignore());
            CreateMap<Employee, EmployeeEditBinding>()
                .ForMember(dest => dest.Photo, opt => opt.Ignore())
                .ForMember(dest => dest.HasPhoto, opt => opt.MapFrom(src =>
                    src.Photo != null));
        }

    }
}
