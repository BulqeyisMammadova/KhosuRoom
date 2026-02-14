using FluentValidation;
using KhosuRoom.Business.Dtos.UserDtos;

namespace KhosuRoom.Business.Validators.UserValidators;

public class MeUpdateProfileImageDtoValidator : AbstractValidator<MeUpdateProfileImageDto>
{
    public MeUpdateProfileImageDtoValidator()
    {
        RuleFor(x => x.ProfileImageUrl)
            .NotEmpty()
            .MaximumLength(500);
    }
}
