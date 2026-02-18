using FluentValidation;
using KhosuRoom.Business.Dtos.AttendanceDtos;
using KhosuRoom.Core.Enums;

namespace KhosuRoom.Business.Validators.AttendanceValidators;

public class SaveAttendanceDtoValidator : AbstractValidator<SaveAttendanceDto>
{
    public SaveAttendanceDtoValidator()
    {
        RuleFor(x => x.SessionId).NotEmpty();

        RuleFor(x => x.Students)
            .NotNull()
            .NotEmpty();

        RuleForEach(x => x.Students).ChildRules(s =>
        {
            s.RuleFor(x => x.StudentId).NotEmpty();
            s.RuleFor(x => x.Status)
                .IsInEnum()
                .Must(v => v == AttendanceStatus.Present || v == AttendanceStatus.Absent);
        });
        RuleFor(x => x.Students)
            .Must(list => list.Select(s => s.StudentId).Distinct().Count() == list.Count)
            .WithMessage("Duplicate StudentId is not allowed.");
    }
}