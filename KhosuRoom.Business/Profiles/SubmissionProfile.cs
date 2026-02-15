using AutoMapper;
using KhosuRoom.Business.Dtos.SubmissionDtos;

namespace KhosuRoom.Business.Profiles;

internal class SubmissionProfile : Profile
{
    public SubmissionProfile()
    {
        CreateMap<Submission, SubmissionGetDto>()
            .ForMember(d => d.StudentFullName,
                o => o.MapFrom(s => (s.Student!.FirstName + " " + s.Student!.LastName).Trim()))
            .ForMember(d => d.Attachments,
                o => o.MapFrom(s => s.SubmissionAttachments));

        CreateMap<SubmissionAttachment, SubmissionAttachmentItemDto>();
    }
}
