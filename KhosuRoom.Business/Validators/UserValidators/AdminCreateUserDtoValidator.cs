using FluentValidation;
using KhosuRoom.Business.Dtos.UserDtos;

namespace KhosuRoom.Business.Validators.UserValidators;

internal class AdminCreateUserDtoValidator: AbstractValidator<AdminCreateUserDto>
{
    public AdminCreateUserDtoValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);

      
    }
}
