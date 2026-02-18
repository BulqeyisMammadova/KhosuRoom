using FluentValidation;
using KhosuRoom.Business.Dtos.AttendanceDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KhosuRoom.Business.Validators.AttendanceValidators;

public class CreateAttendanceSessionDtoValidator : AbstractValidator<CreateAttendanceSessionDto>
{
    public CreateAttendanceSessionDtoValidator()
    {
        RuleFor(x => x.GroupId).NotEmpty();
        RuleFor(x => x.Date).Must(d => d != default).WithMessage("Date is required.");
    }
}

