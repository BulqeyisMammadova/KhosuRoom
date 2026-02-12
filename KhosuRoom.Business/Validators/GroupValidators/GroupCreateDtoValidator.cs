using FluentValidation;
using KhosuRoom.Business.Dtos.GroupDtos;

namespace KhosuRoom.Business.Validators.GroupValidators;

public class GroupCreateDtoValidator : AbstractValidator<GroupCreateDto>
{
    public GroupCreateDtoValidator()
    {
        RuleFor(x => x.Name)
             .NotEmpty().WithMessage("Group name is required.")
             .MaximumLength(100).WithMessage("Group name must not exceed 100 characters.")
             .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d]+$")
             .WithMessage("Group name must contain at least one uppercase letter, one lowercase letter and one digit.");


    }
}
