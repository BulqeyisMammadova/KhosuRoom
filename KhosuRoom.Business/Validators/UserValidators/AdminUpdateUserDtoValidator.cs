using FluentValidation;
using KhosuRoom.Business.Dtos.UserDtos;

namespace KhosuRoom.Business.Validators.UserValidators;

public class AdminUpdateUserDtoValidator : AbstractValidator<AdminUpdateUserDto>
{
    public AdminUpdateUserDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
    }
}
