using FluentValidation;
using KhosuRoom.Business.Dtos.UserDtos;

namespace KhosuRoom.Business.Validators.UserValidators;

public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(6);
    }
}