using AutoMapper;
using KhosuRoom.Business.Dtos.AttendanceDtos;

namespace KhosuRoom.Business.Profiles;

public class AttendanceProfile : Profile
{
    public AttendanceProfile()
    {
        CreateMap<CreateAttendanceSessionDto, AttendanceSession>();
        CreateMap<AttendanceRecord, AttendanceStudentRowDto>()
            .ForMember(d => d.FullName, o => o.MapFrom(s =>string.IsNullOrWhiteSpace($"{s.Student.FirstName} {s.Student.LastName}".Trim())
                    ? (s.Student.UserName ?? "Student")
                    : $"{s.Student.FirstName} {s.Student.LastName}".Trim()));
    }
}
