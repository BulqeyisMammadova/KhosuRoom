using AutoMapper;
using KhosuRoom.Business.Dtos.AttendanceDtos;
using KhosuRoom.Core.Entities;

namespace KhosuRoom.Business.Profiles;

public class AttendanceProfile : Profile
{
    public AttendanceProfile()
    {

        CreateMap<CreateAttendanceSessionDto, AttendanceSession>();

       
        CreateMap<AttendanceSession, AttendanceSessionListItemDto>()
            .ForMember(d => d.SessionId, o => o.MapFrom(s => s.Id));
           

      
        CreateMap<AttendanceRecord, AttendanceStudentRowDto>()
            .ForMember(d => d.StudentId, o => o.MapFrom(s => s.StudentId))

           .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))

            .ForMember(d => d.FullName, o => o.MapFrom(s =>
                string.IsNullOrWhiteSpace($"{s.Student.FirstName} {s.Student.LastName}".Trim())
                    ? (s.Student.UserName ?? "Student")
                    : $"{s.Student.FirstName} {s.Student.LastName}".Trim()
            ));

       
    }
}