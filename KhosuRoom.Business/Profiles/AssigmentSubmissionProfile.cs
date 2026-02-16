using AutoMapper;
using KhosuRoom.Business.Dtos.AssignmentDtos;
using KhosuRoom.Business.Dtos.SubmissionDtos;

namespace KhosuRoom.Business.Profiles;

internal class AssigmentSubmissionProfile:Profile
{
    public AssigmentSubmissionProfile()
    {
        CreateMap<AssignmentCreateFormDto, Assignment>()
            .ForMember(d => d.TeacherId, o => o.Ignore())
            .ForMember(d => d.Attachments, o => o.Ignore())
            .ForMember(d => d.Submissions, o => o.Ignore());

        CreateMap<AssignmentUpdateFormDto, Assignment>()
            .ForMember(d => d.TeacherId, o => o.Ignore())
            .ForMember(d => d.Attachments, o => o.Ignore())
            .ForMember(d => d.Submissions, o => o.Ignore());

        CreateMap<Assignment, AssignmentGetDto>()
            .ForMember(d => d.FileUrls, o => o.MapFrom(s => s.Attachments.Select(a => a.FileUrl)))
            .ForMember(d => d.SubmissionCount, o => o.MapFrom(s => s.Submissions.Count));

        CreateMap<Submission, SubmissionGetDto>()
           .ForMember(d => d.FileUrls, o => o.MapFrom(s => s.Attachments.Select(a => a.FileUrl)))
           .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));
        
    }
}
