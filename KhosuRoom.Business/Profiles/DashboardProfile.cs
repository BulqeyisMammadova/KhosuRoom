using AutoMapper;
using KhosuRoom.Business.Dtos.DashboardDtos;

namespace KhosuRoom.Business.Profiles;

public class DashboardProfile : Profile
{
    public DashboardProfile()
    {
        CreateMap<Submission, StudentSubmissionDto>()
            .ForMember(d => d.StudentId, o => o.MapFrom(s => s.StudentId))
            .ForMember(d => d.IsSubmitted, o => o.MapFrom(s => s.SubmittedAt != null))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.SubmittedAt == null ? null : s.Status.ToString()))
            .ForMember(d => d.SubmittedAt, o => o.MapFrom(s => s.SubmittedAt))
            .ForMember(d => d.Grade, o => o.MapFrom(s => s.Grade));
            
    }
}