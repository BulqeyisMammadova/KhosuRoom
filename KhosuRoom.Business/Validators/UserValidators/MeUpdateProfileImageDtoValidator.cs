using FluentValidation;
using KhosuRoom.Business.Dtos.UserDtos;
using KhosuRoom.Business.Helpers;

namespace KhosuRoom.Business.Validators.UserValidators;

public class MeUpdateProfileImageDtoValidator : AbstractValidator<MeUpdateProfileImageDto>
{
    public MeUpdateProfileImageDtoValidator()
    {
        RuleFor(x => x.ProfileImageUrl)
            .Must(x => x?.CheckSize(2) ?? true).WithMessage("Image is not greate 2")
            .Must(x => x?.CheckType("image") ?? true).WithMessage("Only image");
    }
}
