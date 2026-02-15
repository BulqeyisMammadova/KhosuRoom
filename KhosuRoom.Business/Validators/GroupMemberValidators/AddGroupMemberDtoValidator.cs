using FluentValidation;
using KhosuRoom.Business.Dtos.GroupMemberDtos;
using KhosuRoom.Core.Enums;

namespace KhosuRoom.Business.Validators.GroupMemberValidators;

public class AddGroupMemberDtoValidator : AbstractValidator<AddGroupMemberDto>
{
    public AddGroupMemberDtoValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(r => new[] { GroupRole.Teacher.ToString(), GroupRole.Student.ToString() }.Contains(r))
            .WithMessage("Role must be Teacher or Student");
    }
}
