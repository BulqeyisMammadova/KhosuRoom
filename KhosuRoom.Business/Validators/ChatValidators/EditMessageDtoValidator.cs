using FluentValidation;
using KhosuRoom.Business.Dtos.ChatDtos;

namespace KhosuRoom.Business.Validators.ChatValidators;

public class EditMessageDtoValidator : AbstractValidator<EditMessageDto>
{
    public EditMessageDtoValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .MaximumLength(2000);
    }
}