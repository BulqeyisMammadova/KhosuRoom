using AutoMapper;
using KhosuRoom.Business.Dtos.AssignmentDtos;

namespace KhosuRoom.Business.Profiles;

internal class AssignmentProfile : Profile
{
    public AssignmentProfile()
    {
        CreateMap<AssignmentCreateDto, Assignment>();
        CreateMap<AssignmentUpdateDto, Assignment>();
        CreateMap<Assignment, AssignmentGetDto>().ForMember(d => d.TeacherFullName,
                o => o.MapFrom(s => (s.Teacher!.FirstName + " " + s.Teacher!.LastName).Trim()))
            .ForMember(d => d.Attachments,
                o => o.MapFrom(s => s.Attachments));
        CreateMap<AssignmentAttachment, AssignmentAttachmentItemDto>();
    }
}
