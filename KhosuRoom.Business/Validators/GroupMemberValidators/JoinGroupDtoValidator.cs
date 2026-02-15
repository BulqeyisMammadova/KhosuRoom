using FluentValidation;
using KhosuRoom.Business.Dtos.GroupMemberDtos;

namespace KhosuRoom.Business.Validators.GroupMemberValidators;

public class JoinGroupDtoValidator : AbstractValidator<JoinGroupDto>
{
    public JoinGroupDtoValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(20);
    }
}